using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using TcpRelayMonitor.Helpers;
using TcpRelayMonitor.Models;

namespace TcpRelayMonitor.Services;

/// <summary>
/// TCP中继监听服务，负责接入、分发和会话管理。
/// </summary>
public sealed class TcpRelayService : IAsyncDisposable
{
    private readonly LogService _logService;
    private readonly TcpForwardService _forwardService;
    private readonly ConcurrentDictionary<string, (ClientSession Session, CancellationTokenSource Cts)> _sessions = new();
    private TcpListener? _listener;
    private CancellationTokenSource? _listenerCts;

    public TcpRelayService(LogService logService, TcpForwardService forwardService)
    {
        _logService = logService;
        _forwardService = forwardService;
    }

    public event Action<PlcClientInfo>? OnClientConnected;
    public event Action<PlcClientInfo>? OnClientUpdated;
    public event Action<string>? OnClientDisconnected;

    public bool IsRunning => _listener is not null;

    public async Task StartAsync(string listenIp, int listenPort, string forwardIp, int forwardPort, CancellationToken token)
    {
        if (_listener is not null)
        {
            return;
        }

        _listenerCts = CancellationTokenSource.CreateLinkedTokenSource(token);
        _listener = new TcpListener(SocketHelper.ParseIp(listenIp), listenPort);
        _listener.Start();
        _logService.Info($"开始监听 {listenIp}:{listenPort}");

        _ = Task.Run(() => AcceptLoopAsync(forwardIp, forwardPort, _listenerCts.Token), _listenerCts.Token);
        await Task.CompletedTask;
    }

    private async Task AcceptLoopAsync(string forwardIp, int forwardPort, CancellationToken token)
    {
        while (!token.IsCancellationRequested && _listener is not null)
        {
            try
            {
                var plcClient = await _listener.AcceptTcpClientAsync(token).ConfigureAwait(false);
                var forwardClient = await _forwardService.ConnectAsync(forwardIp, forwardPort, token).ConfigureAwait(false);
                var session = new ClientSession(plcClient, forwardClient, _forwardService, _logService);
                var sessionCts = CancellationTokenSource.CreateLinkedTokenSource(token);
                _sessions[session.Info.SessionId] = (session, sessionCts);
                OnClientConnected?.Invoke(session.Info);
                _logService.Info($"PLC接入 {session.Info.PlcIp}:{session.Info.PlcPort}, 会话 {session.Info.SessionId}");

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await session.RunAsync(sessionCts.Token).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _logService.Error($"会话 {session.Info.SessionId} 转发失败", ex);
                    }
                    finally
                    {
                        await RemoveSessionAsync(session.Info.SessionId).ConfigureAwait(false);
                    }
                }, sessionCts.Token);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logService.Error("接受客户端连接失败", ex);
            }
        }
    }

    public async Task StopAsync()
    {
        _listenerCts?.Cancel();
        _listener?.Stop();
        _listener = null;
        _logService.Info("停止监听请求已发出");

        foreach (var key in _sessions.Keys)
        {
            await RemoveSessionAsync(key).ConfigureAwait(false);
        }

        _listenerCts?.Dispose();
        _listenerCts = null;
    }

    private async Task RemoveSessionAsync(string sessionId)
    {
        if (_sessions.TryRemove(sessionId, out var entry))
        {
            entry.Cts.Cancel();
            await entry.Session.DisposeAsync().ConfigureAwait(false);
            entry.Cts.Dispose();
            OnClientDisconnected?.Invoke(sessionId);
            _logService.Info($"会话 {sessionId} 已断开并清理");
        }
    }

    public ValueTask DisposeAsync() => new(StopAsync());
}
