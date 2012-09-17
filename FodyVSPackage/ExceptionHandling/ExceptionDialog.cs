using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

public class ExceptionDialog
{

    [DllImport("user32.dll")]
    static extern IntPtr GetActiveWindow();

    public void HandleException(Exception exception)
    {
        var model = new ExceptionWindowModel
                        {
                            ExceptionText = exception.ExceptionHierarchyToString(),
                        };
        var window = new ExceptionWindow(model);
        new WindowInteropHelper(window)
            {
                Owner = GetActiveWindow()
            };
        window.ShowDialog();
    }
}