using System.Runtime.InteropServices;

namespace PdfCompressor.Windows.Notifications;

public static class WindowsNotifier
{
    private const string Title = "PdfCompressor";

    // MessageBox via user32.dll evita dependência de WinForms.
    private const uint MB_OK = 0x00000000;
    private const uint MB_ICONINFORMATION = 0x00000040;
    private const uint MB_ICONERROR = 0x00000010;
    private const uint MB_SETFOREGROUND = 0x00010000;
    private const uint MB_TOPMOST = 0x00040000;

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

    public static void Success(double reductionPercent) =>
        Show($"PDF comprimido com sucesso.\nRedução: {reductionPercent:0.##}%", isError: false);

    public static void NotReduced() =>
        Show("O PDF já parece estar otimizado.\nNenhum novo arquivo foi mantido.", isError: false);

    public static void Error() =>
        Show("Não foi possível comprimir o PDF.", isError: true);

    private static void Show(string message, bool isError)
    {
        var icon = isError ? MB_ICONERROR : MB_ICONINFORMATION;
        MessageBox(IntPtr.Zero, message, Title, MB_OK | icon | MB_SETFOREGROUND | MB_TOPMOST);
    }
}
