using System;
using System.Windows.Forms;
using TcpRelayMonitor.Forms;

namespace TcpRelayMonitor;

/// <summary>
/// 程序入口。
/// </summary>
internal static class Program
{
    /// <summary>
    /// 主入口方法。
    /// </summary>
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
