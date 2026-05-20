namespace TcpRelayMonitor.Config;

/// <summary>
/// 应用配置对象。
/// </summary>
public sealed class AppConfig
{
    /// <summary>监听IP。</summary>
    public string ListenIp { get; set; } = "0.0.0.0";

    /// <summary>监听端口。</summary>
    public int ListenPort { get; set; } = 5000;

    /// <summary>转发目标IP。</summary>
    public string ForwardIp { get; set; } = "127.0.0.1";

    /// <summary>转发目标端口。</summary>
    public int ForwardPort { get; set; } = 6000;

    /// <summary>UI日志最大保留条数。</summary>
    public int MaxLogCount { get; set; } = 2000;
}
