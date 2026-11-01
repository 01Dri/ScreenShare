using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using Avalonia;
using Avalonia.Controls;

namespace ScreenShare.App.Native;

[SupportedOSPlatform("windows")]
internal class Windows : OS.IBackend
{
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool PathFindOnPath([In, Out] StringBuilder pszFile, [In] string[] ppszOtherDirs);

    public void SetupApp(AppBuilder builder)
    {
        //
    }

    public void SetupWindow(Window window)
    {
        window.ExtendClientAreaToDecorationsHint = true;
        window.BorderThickness = new Thickness(1);
        window.Padding = new Thickness(0);
    }

    public OS.Directories GetOrCreateDirectories()
    {
        var dirs = new OS.Directories();
        var execFile = Environment.ProcessPath;
        var portableDir = Path.Combine(Path.GetDirectoryName(execFile)!, "data");
        if (Directory.Exists(portableDir))
            dirs.CacheDir = portableDir;
        else
        {
            var appDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "ScreenShare");
            if (!Directory.Exists(appDataDir))
                Directory.CreateDirectory(appDataDir);
            dirs.CacheDir = appDataDir;
        }

        return dirs;
    }
}