using System.Collections.ObjectModel;
using System.ComponentModel;
using TcpRelayMonitor.Config;
using TcpRelayMonitor.Models;
using TcpRelayMonitor.Services;

namespace TcpRelayMonitor.ViewModels;

/// <summary>
/// 主界面视图模型。
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
        _relayService.OnClientConnected += info => Clients.Add(info);
        _relayService.OnClientDisconnected += sessionId =>
        {
            var item = Clients.FirstOrDefault(x => x.SessionId == sessionId);
            if (item is not null)
            {
                item.Status = "已断开";
            }
        };
    }

    public AppConfig Config { get; }
    public BindingList<PlcClientInfo> Clients { get; } = new();
    public ObservableCollection<LogItem> Logs { get; } = new();
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

    public void ClearLogs() => Logs.Clear();

    public void Dispose()
    {
        _runCts?.Cancel();
        _runCts?.Dispose();
    }
}
