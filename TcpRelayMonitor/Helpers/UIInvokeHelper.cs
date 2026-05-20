using System.Windows.Forms;

namespace TcpRelayMonitor.Helpers;

/// <summary>
/// UI线程调用辅助工具。
/// </summary>
public static class UIInvokeHelper
{
    /// <summary>
    /// 安全执行UI更新。
    /// </summary>
    public static void SafeInvoke(Control control, Action action)
    {
        if (control.IsDisposed)
        {
            return;
        }

        if (control.InvokeRequired)
        {
            control.BeginInvoke(action);
            return;
        }

        action();
    }
}
