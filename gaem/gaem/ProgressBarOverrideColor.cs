using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

public class myProgBar : ProgressBar
{
    [DllImport("uxtheme.dll")]
    private static extern int SetWindowTheme(IntPtr hWnd, string appname, string idlist);

    protected override void OnHandleCreated(EventArgs e)
    {
        SetWindowTheme(Handle, "", "");
        base.OnHandleCreated(e);
    }
}
