using System.Net.Sockets;

namespace TcpRelayMonitor.Services;

/// <summary>
/// TCP转发服务，负责建立目标连接并发送数据。
/// </summary>
public sealed class TcpForwardService
{
    private readonly LogService _logService;

    public TcpForwardService(LogService logService)
    {
        _logService = logService;
    }

    /// <summary>
    /// 创建到目标服务端的连接。
    /// </summary>
    public async Task<TcpClient> ConnectAsync(string ip, int port, CancellationToken cancellationToken)
    {
        var client = new TcpClient();
        await client.ConnectAsync(ip, port, cancellationToken).ConfigureAwait(false);
        _logService.Info($"已连接转发目标 {ip}:{port}");
        return client;
    }

    /// <summary>
    /// 异步发送原始字节。
    /// </summary>
    public async Task SendAsync(NetworkStream stream, byte[] buffer, int length, CancellationToken cancellationToken)
    {
        await stream.WriteAsync(buffer.AsMemory(0, length), cancellationToken).ConfigureAwait(false);
    }
}
