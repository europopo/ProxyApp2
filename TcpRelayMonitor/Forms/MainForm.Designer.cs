namespace TcpRelayMonitor.Forms;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;
    private AntdUI.PageHeader header = null!;
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
    private TabControl tabMain = null!;
    private TabPage tabClients = null!;
    private TabPage tabLogs = null!;
    private DataGridView dgvClients = null!;
    private RichTextBox rtbLog = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        header = new AntdUI.PageHeader { Text = "TCP中继监听器", Location = new Point(12, 8), Size = new Size(980, 40) };

        lblListenIp = new AntdUI.Label { Text = "监听IP", Location = new Point(20, 62), Size = new Size(70, 24) };
        txtListenIp = new AntdUI.Input { Location = new Point(90, 58), Size = new Size(150, 30) };

        lblListenPort = new AntdUI.Label { Text = "监听端口", Location = new Point(250, 62), Size = new Size(80, 24) };
        txtListenPort = new AntdUI.Input { Location = new Point(325, 58), Size = new Size(100, 30) };

        lblForwardIp = new AntdUI.Label { Text = "转发IP", Location = new Point(440, 62), Size = new Size(70, 24) };
        txtForwardIp = new AntdUI.Input { Location = new Point(505, 58), Size = new Size(150, 30) };

        lblForwardPort = new AntdUI.Label { Text = "转发端口", Location = new Point(665, 62), Size = new Size(80, 24) };
        txtForwardPort = new AntdUI.Input { Location = new Point(740, 58), Size = new Size(100, 30) };

        btnStart = new AntdUI.Button { Text = "启动监听", Type = AntdUI.TTypeMini.Success, Location = new Point(20, 98), Size = new Size(100, 32) };
        btnStop = new AntdUI.Button { Text = "停止监听", Type = AntdUI.TTypeMini.Error, Location = new Point(130, 98), Size = new Size(100, 32) };
        btnClearLog = new AntdUI.Button { Text = "清空日志", Location = new Point(240, 98), Size = new Size(100, 32) };

        tabMain = new TabControl { Location = new Point(20, 140), Size = new Size(960, 470) };
        tabClients = new TabPage { Text = "PLC连接" };
        tabLogs = new TabPage { Text = "运行日志" };

        dgvClients = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoGenerateColumns = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false
        };

        rtbLog = new RichTextBox { Dock = DockStyle.Fill, ReadOnly = true, Font = new Font("Consolas", 10F) };

        btnStart.Click += btnStart_Click;
        btnStop.Click += btnStop_Click;
        btnClearLog.Click += btnClearLog_Click;

        tabClients.Controls.Add(dgvClients);
        tabLogs.Controls.Add(rtbLog);
        tabMain.TabPages.Add(tabClients);
        tabMain.TabPages.Add(tabLogs);

        Controls.Add(header);
        Controls.Add(lblListenIp);
        Controls.Add(txtListenIp);
        Controls.Add(lblListenPort);
        Controls.Add(txtListenPort);
        Controls.Add(lblForwardIp);
        Controls.Add(txtForwardIp);
        Controls.Add(lblForwardPort);
        Controls.Add(txtForwardPort);
        Controls.Add(btnStart);
        Controls.Add(btnStop);
        Controls.Add(btnClearLog);
        Controls.Add(tabMain);

        Text = "TcpRelayMonitor";
        ClientSize = new Size(1000, 630);
        MinimumSize = new Size(1000, 630);
        StartPosition = FormStartPosition.CenterScreen;
    }
}
