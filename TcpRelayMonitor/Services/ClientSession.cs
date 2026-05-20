using System.Net;
using System.Net.Sockets;
using TcpRelayMonitor.Models;

namespace TcpRelayMonitor.Services;

/// <summary>
/// 客户端会话，负责单个PLC连接转发生命周期。
/// </summary>
public sealed class ClientSession : IAsyncDisposable
{
    private readonly TcpClient _plcClient;
    private readonly TcpClient _forwardClient;
    private readonly TcpForwardService _forwardService;
    private readonly LogService _logService;

    public ClientSession(TcpClient plcClient, TcpClient forwardClient, TcpForwardService forwardService, LogService logService)
    {
        _plcClient = plcClient;
        _forwardClient = forwardClient;
        _forwardService = forwardService;
        _logService = logService;

        var ep = (IPEndPoint)_plcClient.Client.RemoteEndPoint!;
        Info = new PlcClientInfo
        {
            PlcIp = ep.Address.ToString(),
            PlcPort = ep.Port,
            Status = "已连接"
        };
    }

    /// <summary>当前会话状态。</summary>
    public PlcClientInfo Info { get; }

    /// <summary>
    /// 启动转发循环。
    /// </summary>
    public async Task RunAsync(CancellationToken token)
    {
        var readStream = _plcClient.GetStream();
        var writeStream = _forwardClient.GetStream();
        var buffer = new byte[8192];

        while (!token.IsCancellationRequested)
        {
            var read = await readStream.ReadAsync(buffer, token).ConfigureAwait(false);
            if (read == 0)
            {
                break;
            }

            Info.ReceivedBytes += read;
            _logService.Info($"[{Info.SessionId}] 接收 {read} 字节");

            await _forwardService.SendAsync(writeStream, buffer, read, token).ConfigureAwait(false);
            Info.SentBytes += read;
            _logService.Info($"[{Info.SessionId}] 转发成功，发送 {read} 字节");
        }
    }

    public ValueTask DisposeAsync()
    {
        Info.Status = "已断开";
        _plcClient.Dispose();
        _forwardClient.Dispose();
        return ValueTask.CompletedTask;
    }
}
