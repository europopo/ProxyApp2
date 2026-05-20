using System.ComponentModel;
using System.Text;
using TcpRelayMonitor.Helpers;
using TcpRelayMonitor.Models;
using TcpRelayMonitor.ViewModels;

namespace TcpRelayMonitor.Forms;

/// <summary>
/// 主窗口。
/// </summary>
public partial class MainForm : AntdUI.Window
{
    private readonly MainViewModel _viewModel;
    private readonly BindingList<PlcClientInfo> _clients = new();
    private readonly System.Windows.Forms.Timer _logTimer;
    private readonly NotifyIcon _notifyIcon;

    public MainForm()
    {
        InitializeComponent();
        _viewModel = new MainViewModel();
        BindData();
        BindEvents();

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
        RefreshClientTable();
        SetListenStatus("未监听", Color.Gray);
    }

    private void BindEvents()
    {
        _viewModel.ClientConnected += info => UIInvokeHelper.SafeInvoke(this, () =>
        {
            _clients.Add(info);
            RefreshClientTable();
        });

        _viewModel.ClientDisconnected += sessionId => UIInvokeHelper.SafeInvoke(this, () =>
        {
            var client = _clients.FirstOrDefault(x => x.SessionId == sessionId);
            if (client is not null)
            {
                client.Status = "已断开";
            }

            RefreshClientTable();
        });
    }

    private async void btnStart_Click(object sender, EventArgs e)
    {
        try
        {
            await _viewModel.StartAsync();
            SetListenStatus("监听中", Color.SeaGreen);
        }
        catch (Exception ex)
        {
            SetListenStatus("错误", Color.Firebrick);
            rtbLog.AppendText($"启动失败: {ex.Message}{Environment.NewLine}");
        }
    }

    private async void btnStop_Click(object sender, EventArgs e)
    {
        await _viewModel.StopAsync();
        SetListenStatus("未监听", Color.Gray);
    }

    private void btnClearLog_Click(object sender, EventArgs e) => rtbLog.Clear();

    private void btnMinimize_Click(object? sender, EventArgs e) => WindowState = FormWindowState.Minimized;

    private void btnClose_Click(object? sender, EventArgs e) => Close();

    private void SetListenStatus(string status, Color color)
    {
        lblListenStatus.Text = status;
        lblListenStatus.ForeColor = color;
    }

    private void RefreshClientTable()
    {
        tableClients.DataSource = _clients.Select(x => new
        {
            PLC_IP = x.PlcIp,
            PLC_Port = x.PlcPort,
            连接时间 = x.ConnectedAt,
            接收字节 = x.ReceivedBytes,
            发送字节 = x.SentBytes,
            状态 = x.Status
        }).ToList();
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
                rtbLog.Lines = rtbLog.Lines.Skip(1).ToArray();
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
            _notifyIcon.ShowBalloonTip(1000, "TcpRelayMonitor", "程序已最小化到托盘", ToolTipIcon.Info);
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
