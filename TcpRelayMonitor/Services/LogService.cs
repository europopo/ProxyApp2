using System.Collections.Concurrent;
using TcpRelayMonitor.Models;

namespace TcpRelayMonitor.Services;

/// <summary>
/// 日志服务，负责线程安全写入与批量消费。
/// </summary>
public sealed class LogService
{
    private readonly ConcurrentQueue<LogItem> _queue = new();

    /// <summary>日志新增事件。</summary>
    public event Action? OnLogEnqueued;

    /// <summary>
    /// 写入信息日志。
    /// </summary>
    public void Info(string message) => Enqueue(new LogItem { Level = "INFO", Message = message });

    /// <summary>
    /// 写入错误日志。
    /// </summary>
    public void Error(string message, Exception? ex = null) => Enqueue(new LogItem { Level = "ERROR", Message = message, Exception = ex?.ToString() });

    /// <summary>
    /// 入队日志。
    /// </summary>
    public void Enqueue(LogItem item)
    {
        _queue.Enqueue(item);
        OnLogEnqueued?.Invoke();
    }

    /// <summary>
    /// 批量拉取待显示日志。
    /// </summary>
    public List<LogItem> Drain(int maxItems = 500)
    {
        var list = new List<LogItem>(maxItems);
        for (var i = 0; i < maxItems && _queue.TryDequeue(out var item); i++)
        {
            list.Add(item);
        }

        return list;
    }
}
