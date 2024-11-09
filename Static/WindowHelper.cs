﻿namespace StalkerModdingHelper.Static;

public static class WindowHelper
{
    public static void BringWindowToForeground(Process process)
    {
        IntPtr handle = process.MainWindowHandle;
        if (IsIconic(handle))
        {
            ShowWindow(handle, SW_RESTORE);
        }

        SetForegroundWindow(handle);
    }

    const int SW_RESTORE = 9;

    [System.Runtime.InteropServices.DllImport("User32.dll")]
    private static extern bool SetForegroundWindow(IntPtr handle);
    [System.Runtime.InteropServices.DllImport("User32.dll")]
    private static extern bool ShowWindow(IntPtr handle, int nCmdShow);
    [System.Runtime.InteropServices.DllImport("User32.dll")]
    private static extern bool IsIconic(IntPtr handle);
}