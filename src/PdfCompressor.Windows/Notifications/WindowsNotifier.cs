using System.Runtime.InteropServices;

namespace PdfCompressor.Windows.Notifications;

public static class WindowsNotifier
{
    // MessageBox via user32.dll evita dependência de WinForms.
    private const uint MB_OK = 0x00000000;
    private const uint MB_ICONINFORMATION = 0x00000040;
    private const uint MB_ICONERROR = 0x00000010;
    private const uint MB_SETFOREGROUND = 0x00010000;
    private const uint MB_TOPMOST = 0x00040000;

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

    public static void ShowInformation(string title, string message) =>
        Show(title, message, isError: false);

    public static void ShowError(string title, string message) =>
        Show(title, message, isError: true);

    private static void Show(string title, string message, bool isError)
    {
        var icon = isError ? MB_ICONERROR : MB_ICONINFORMATION;
        MessageBox(IntPtr.Zero, message, title, MB_OK | icon | MB_SETFOREGROUND | MB_TOPMOST);
    }
}
