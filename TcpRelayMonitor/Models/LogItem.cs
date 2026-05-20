namespace TcpRelayMonitor.Models;

/// <summary>
/// 日志项模型。
/// </summary>
public sealed class LogItem
{
    /// <summary>时间。</summary>
    public DateTime Time { get; init; } = DateTime.Now;

    /// <summary>级别。</summary>
    public string Level { get; init; } = "INFO";

    /// <summary>消息内容。</summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>异常信息。</summary>
    public string? Exception { get; init; }

    /// <summary>格式化输出。</summary>
    public override string ToString() => $"[{Time:yyyy-MM-dd HH:mm:ss.fff}] [{Level}] {Message}{(string.IsNullOrWhiteSpace(Exception) ? string.Empty : " | " + Exception)}";
}
