namespace TcpRelayMonitor.Forms;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;
    private AntdUI.PageHeader header = null!;
    private AntdUI.Panel panelConfig = null!;
    private AntdUI.Label lblListenIp = null!;
    private AntdUI.Label lblListenPort = null!;
    private AntdUI.Label lblForwardIp = null!;
    private AntdUI.Label lblForwardPort = null!;
    private AntdUI.Input txtListenIp = null!;
    private AntdUI.Input txtListenPort = null!;
    private AntdUI.Input txtForwardIp = null!;
    private AntdUI.Input txtForwardPort = null!;
    private AntdUI.Button btnStart = null!;
    private AntdUI.Button btnStop = null!;
    private AntdUI.Button btnClearLog = null!;
    private AntdUI.Button btnMinimize = null!;
    private AntdUI.Button btnClose = null!;
    private TabControl tabMain = null!;
    private TabPage tabClients = null!;
    private TabPage tabLogs = null!;
    private AntdUI.Table tableClients = null!;
    private AntdUI.Label lblListenStatus = null!;
    private RichTextBox rtbLog = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        header = new AntdUI.PageHeader { Text = "TcpRelayMonitor", Description = "Element风格中继监听控制台", Location = new Point(12, 8), Size = new Size(760, 44) };
        btnMinimize = new AntdUI.Button { Text = "最小化", Type = AntdUI.TTypeMini.Info, Location = new Point(780, 14), Size = new Size(90, 30) };
        btnClose = new AntdUI.Button { Text = "关闭", Type = AntdUI.TTypeMini.Error, Location = new Point(880, 14), Size = new Size(90, 30) };

        panelConfig = new AntdUI.Panel { Location = new Point(20, 60), Size = new Size(950, 90), Radius = 8 };
        lblListenIp = new AntdUI.Label { Text = "监听IP", Location = new Point(16, 14), Size = new Size(60, 24) };
        txtListenIp = new AntdUI.Input { Location = new Point(78, 10), Size = new Size(160, 30), PlaceholderText = "0.0.0.0" };
        lblListenPort = new AntdUI.Label { Text = "监听端口", Location = new Point(250, 14), Size = new Size(80, 24) };
        txtListenPort = new AntdUI.Input { Location = new Point(325, 10), Size = new Size(100, 30), PlaceholderText = "5000" };
        lblForwardIp = new AntdUI.Label { Text = "转发IP", Location = new Point(440, 14), Size = new Size(70, 24) };
        txtForwardIp = new AntdUI.Input { Location = new Point(500, 10), Size = new Size(160, 30), PlaceholderText = "127.0.0.1" };
        lblForwardPort = new AntdUI.Label { Text = "转发端口", Location = new Point(675, 14), Size = new Size(80, 24) };
        txtForwardPort = new AntdUI.Input { Location = new Point(750, 10), Size = new Size(100, 30), PlaceholderText = "6000" };

        btnStart = new AntdUI.Button { Text = "启动监听", Type = AntdUI.TTypeMini.Success, Location = new Point(16, 48), Size = new Size(130, 38), Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold) };
        btnStop = new AntdUI.Button { Text = "停止监听", Type = AntdUI.TTypeMini.Warn, Location = new Point(152, 48), Size = new Size(130, 38), Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold) };
        btnClearLog = new AntdUI.Button { Text = "清空日志", Location = new Point(288, 48), Size = new Size(130, 38), Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold) };

        tabMain = new TabControl { Location = new Point(20, 162), Size = new Size(950, 450) };
        tabClients = new TabPage { Text = "PLC连接" };
        tabLogs = new TabPage { Text = "运行日志" };

        lblListenStatus = new AntdUI.Label { Text = "监听状态：未监听", ForeColor = Color.Gray, Location = new Point(380, 53), Size = new Size(260, 28), Font = new Font("Microsoft YaHei UI", 11F, FontStyle.Bold) };

        tableClients = new AntdUI.Table { Dock = DockStyle.Fill, Font = new Font("Microsoft YaHei UI", 10F) };
        rtbLog = new RichTextBox { Dock = DockStyle.Fill, ReadOnly = true, Font = new Font("Consolas", 11F) };

        btnStart.Click += btnStart_Click;
        btnStop.Click += btnStop_Click;
        btnClearLog.Click += btnClearLog_Click;
        btnMinimize.Click += btnMinimize_Click;
        btnClose.Click += btnClose_Click;

        panelConfig.Controls.Add(lblListenIp);
        panelConfig.Controls.Add(txtListenIp);
        panelConfig.Controls.Add(lblListenPort);
        panelConfig.Controls.Add(txtListenPort);
        panelConfig.Controls.Add(lblForwardIp);
        panelConfig.Controls.Add(txtForwardIp);
        panelConfig.Controls.Add(lblForwardPort);
        panelConfig.Controls.Add(txtForwardPort);
        panelConfig.Controls.Add(btnStart);
        panelConfig.Controls.Add(btnStop);
        panelConfig.Controls.Add(btnClearLog);

        tabClients.Controls.Add(tableClients);
        tabLogs.Controls.Add(rtbLog);
        tabMain.TabPages.Add(tabClients);
        tabMain.TabPages.Add(tabLogs);

        Controls.Add(header);
        Controls.Add(btnMinimize);
        Controls.Add(btnClose);
        Controls.Add(panelConfig);
        Controls.Add(lblListenStatus);
        Controls.Add(tabMain);

        Text = "TcpRelayMonitor";
        ClientSize = new Size(1000, 630);
        MinimumSize = new Size(1000, 630);
        StartPosition = FormStartPosition.CenterScreen;
    }
}
