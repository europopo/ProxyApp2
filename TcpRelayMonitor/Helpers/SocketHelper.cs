using System.Net;

namespace TcpRelayMonitor.Helpers;

/// <summary>
/// Socket辅助工具。
/// </summary>
public static class SocketHelper
{
    /// <summary>
    /// 将IP字符串解析为地址。
    /// </summary>
    public static IPAddress ParseIp(string ip) => IPAddress.TryParse(ip, out var address) ? address : throw new ArgumentException($"非法IP地址: {ip}");
}
