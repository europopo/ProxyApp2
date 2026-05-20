using System.IO;
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
    /// 启动转发循环：PLC -> 目标服务端，再将目标返回数据回写PLC。
    /// </summary>
    public async Task RunAsync(CancellationToken token)
    {
        var plcStream = _plcClient.GetStream();
        var forwardStream = _forwardClient.GetStream();
        var plcBuffer = new byte[8192];
        var responseBuffer = new byte[8192];

        while (!token.IsCancellationRequested)
        {
            var read = await plcStream.ReadAsync(plcBuffer, token).ConfigureAwait(false);
            if (read == 0)
            {
                break;
            }

            Info.ReceivedBytes += read;
            _logService.Info($"[{Info.SessionId}] PLC接收 {read} 字节");

            try
            {
                await _forwardService.SendAsync(forwardStream, plcBuffer, read, token).ConfigureAwait(false);
                _logService.Info($"[{Info.SessionId}] 转发到目标成功，发送 {read} 字节");
            }
            catch (IOException ex) when (ex.InnerException is SocketException socketEx && socketEx.SocketErrorCode == SocketError.ConnectionReset)
            {
                _logService.Info($"[{Info.SessionId}] 目标连接已关闭（10054），会话结束");
                break;
            }

            var responseRead = await forwardStream.ReadAsync(responseBuffer, token).ConfigureAwait(false);
            if (responseRead == 0)
            {
                _logService.Info($"[{Info.SessionId}] 目标服务端主动断开");
                break;
            }

            await plcStream.WriteAsync(responseBuffer.AsMemory(0, responseRead), token).ConfigureAwait(false);
            Info.SentBytes += responseRead;
            _logService.Info($"[{Info.SessionId}] 回写PLC成功，发送 {responseRead} 字节");
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
