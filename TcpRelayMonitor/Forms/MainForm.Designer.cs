namespace TcpRelayMonitor.Forms;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;
    private AntdUI.Input txtListenIp = null!;
    private AntdUI.Input txtListenPort = null!;
    private AntdUI.Input txtForwardIp = null!;
    private AntdUI.Input txtForwardPort = null!;
    private AntdUI.Button btnStart = null!;
    private AntdUI.Button btnStop = null!;
    private AntdUI.Button btnClearLog = null!;
    private DataGridView dgvClients = null!;
    private RichTextBox rtbLog = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        txtListenIp = new AntdUI.Input { PlaceholderText = "监听IP", Location = new Point(20, 20), Size = new Size(150, 32) };
        txtListenPort = new AntdUI.Input { PlaceholderText = "监听端口", Location = new Point(180, 20), Size = new Size(100, 32) };
        txtForwardIp = new AntdUI.Input { PlaceholderText = "转发IP", Location = new Point(290, 20), Size = new Size(150, 32) };
        txtForwardPort = new AntdUI.Input { PlaceholderText = "转发端口", Location = new Point(450, 20), Size = new Size(100, 32) };
        btnStart = new AntdUI.Button { Text = "启动监听", Location = new Point(570, 20), Size = new Size(100, 32) };
        btnStop = new AntdUI.Button { Text = "停止监听", Location = new Point(680, 20), Size = new Size(100, 32) };
        btnClearLog = new AntdUI.Button { Text = "清空日志", Location = new Point(790, 20), Size = new Size(100, 32) };
        dgvClients = new DataGridView { Location = new Point(20, 70), Size = new Size(870, 250), ReadOnly = true, AutoGenerateColumns = true };
        rtbLog = new RichTextBox { Location = new Point(20, 330), Size = new Size(870, 250), ReadOnly = true };

        btnStart.Click += btnStart_Click;
        btnStop.Click += btnStop_Click;
        btnClearLog.Click += btnClearLog_Click;

        Controls.Add(txtListenIp);
        Controls.Add(txtListenPort);
        Controls.Add(txtForwardIp);
        Controls.Add(txtForwardPort);
        Controls.Add(btnStart);
        Controls.Add(btnStop);
        Controls.Add(btnClearLog);
        Controls.Add(dgvClients);
        Controls.Add(rtbLog);

        Text = "TcpRelayMonitor";
        ClientSize = new Size(920, 600);
        StartPosition = FormStartPosition.CenterScreen;
    }
}
