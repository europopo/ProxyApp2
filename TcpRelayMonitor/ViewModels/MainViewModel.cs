using System.ComponentModel;
using TcpRelayMonitor.Config;
using TcpRelayMonitor.Models;
using TcpRelayMonitor.Services;

namespace TcpRelayMonitor.ViewModels;

/// <summary>
/// 主界面视图模型，负责配置、服务调用与事件转发。
/// </summary>
public sealed class MainViewModel : INotifyPropertyChanged, IDisposable
{
    private readonly LogService _logService;
    private readonly TcpForwardService _forwardService;
    private readonly TcpRelayService _relayService;
    private CancellationTokenSource? _runCts;

    public MainViewModel()
    {
        Config = new AppConfig();
        _logService = new LogService();
        _forwardService = new TcpForwardService(_logService);
        _relayService = new TcpRelayService(_logService, _forwardService);
        _relayService.OnClientConnected += info => ClientConnected?.Invoke(info);
        _relayService.OnClientDisconnected += sessionId => ClientDisconnected?.Invoke(sessionId);
    }

    /// <summary>客户端接入事件。</summary>
    public event Action<PlcClientInfo>? ClientConnected;

    /// <summary>客户端断开事件。</summary>
    public event Action<string>? ClientDisconnected;

    public AppConfig Config { get; }
    public event PropertyChangedEventHandler? PropertyChanged;

    public async Task StartAsync()
    {
        _runCts = new CancellationTokenSource();
        await _relayService.StartAsync(Config.ListenIp, Config.ListenPort, Config.ForwardIp, Config.ForwardPort, _runCts.Token).ConfigureAwait(false);
    }

    public async Task StopAsync()
    {
        _runCts?.Cancel();
        await _relayService.StopAsync().ConfigureAwait(false);
        _runCts?.Dispose();
        _runCts = null;
    }

    public List<LogItem> DrainLogs() => _logService.Drain();

    public void Dispose()
    {
        _runCts?.Cancel();
        _runCts?.Dispose();
    }
}
