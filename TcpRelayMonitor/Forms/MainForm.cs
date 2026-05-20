using System.Text;
using TcpRelayMonitor.Helpers;
using TcpRelayMonitor.ViewModels;

namespace TcpRelayMonitor.Forms;

/// <summary>
/// 主窗口。
/// </summary>
public partial class MainForm : AntdUI.Window
{
    private readonly MainViewModel _viewModel;
    private readonly System.Windows.Forms.Timer _logTimer;
    private readonly NotifyIcon _notifyIcon;

    public MainForm()
    {
        InitializeComponent();
        _viewModel = new MainViewModel();
        BindData();

        _logTimer = new System.Windows.Forms.Timer { Interval = 200 };
        _logTimer.Tick += (_, _) => FlushLogs();
        _logTimer.Start();

        _notifyIcon = new NotifyIcon
        {
            Text = "TcpRelayMonitor",
            Visible = true,
            Icon = SystemIcons.Application
        };
        _notifyIcon.DoubleClick += (_, _) => RestoreFromTray();

        Resize += MainForm_Resize;
        FormClosing += MainForm_FormClosing;
    }

    private void BindData()
    {
        txtListenIp.DataBindings.Add("Text", _viewModel.Config, nameof(_viewModel.Config.ListenIp));
        txtListenPort.DataBindings.Add("Text", _viewModel.Config, nameof(_viewModel.Config.ListenPort));
        txtForwardIp.DataBindings.Add("Text", _viewModel.Config, nameof(_viewModel.Config.ForwardIp));
        txtForwardPort.DataBindings.Add("Text", _viewModel.Config, nameof(_viewModel.Config.ForwardPort));
        dgvClients.DataSource = _viewModel.Clients;
    }

    private async void btnStart_Click(object sender, EventArgs e) => await _viewModel.StartAsync();

    private async void btnStop_Click(object sender, EventArgs e) => await _viewModel.StopAsync();

    private void btnClearLog_Click(object sender, EventArgs e)
    {
        _viewModel.ClearLogs();
        rtbLog.Clear();
    }

    private void FlushLogs()
    {
        var batch = _viewModel.DrainLogs();
        if (batch.Count == 0)
        {
            return;
        }

        UIInvokeHelper.SafeInvoke(this, () =>
        {
            var sb = new StringBuilder();
            foreach (var item in batch)
            {
                sb.AppendLine(item.ToString());
            }

            rtbLog.AppendText(sb.ToString());
            while (rtbLog.Lines.Length > _viewModel.Config.MaxLogCount)
            {
                var lines = rtbLog.Lines.Skip(1).ToArray();
                rtbLog.Lines = lines;
            }

            rtbLog.SelectionStart = rtbLog.TextLength;
            rtbLog.ScrollToCaret();
        });
    }

    private void MainForm_Resize(object? sender, EventArgs e)
    {
        if (WindowState == FormWindowState.Minimized)
        {
            Hide();
            _notifyIcon.ShowBalloonTip(1500, "TcpRelayMonitor", "程序已最小化到托盘", ToolTipIcon.Info);
        }
    }

    private void RestoreFromTray()
    {
        Show();
        WindowState = FormWindowState.Normal;
        Activate();
    }

    private async void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        _logTimer.Stop();
        await _viewModel.StopAsync();
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
        _viewModel.Dispose();
    }
}
