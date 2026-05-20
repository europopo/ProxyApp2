using System.ComponentModel;

namespace TcpRelayMonitor.Models;

/// <summary>
/// PLC客户端状态信息模型。
/// </summary>
public sealed class PlcClientInfo : INotifyPropertyChanged
{
    private long _receivedBytes;
    private long _sentBytes;
    private string _status = "已连接";

    /// <summary>唯一会话ID。</summary>
    public string SessionId { get; init; } = Guid.NewGuid().ToString("N");

    /// <summary>PLC远端IP。</summary>
    public string PlcIp { get; init; } = string.Empty;

    /// <summary>PLC远端端口。</summary>
    public int PlcPort { get; init; }

    /// <summary>连接建立时间。</summary>
    public DateTime ConnectedAt { get; init; } = DateTime.Now;

    /// <summary>已接收字节。</summary>
    public long ReceivedBytes { get => _receivedBytes; set { _receivedBytes = value; OnPropertyChanged(nameof(ReceivedBytes)); } }

    /// <summary>已发送字节。</summary>
    public long SentBytes { get => _sentBytes; set { _sentBytes = value; OnPropertyChanged(nameof(SentBytes)); } }

    /// <summary>连接状态。</summary>
    public string Status { get => _status; set { _status = value; OnPropertyChanged(nameof(Status)); } }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
