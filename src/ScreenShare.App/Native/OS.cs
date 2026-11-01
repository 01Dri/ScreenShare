using Avalonia;
using Avalonia.Controls;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace ScreenShare.App.Native;

public static partial class OS
{
    public class Directories
    {
        public string CacheDir { get; set; } = string.Empty;
    }

    public interface IBackend
    {
        void SetupApp(AppBuilder builder);
        void SetupWindow(Window window);
        Directories GetOrCreateDirectories();
    }

    public static Directories BasicDirectories { get; private set; } = new();

    static OS()
    {
        if (OperatingSystem.IsWindows())
            _backend = new Windows();
    }

    public static void SetupBasicDirectories()
        => BasicDirectories = _backend.GetOrCreateDirectories();

    public static void SetupApp(AppBuilder builder)
        => _backend.SetupApp(builder);

    public static void SetupForWindow(Window window)
        => _backend.SetupWindow(window);

    public static void LogException(Exception ex)
    {
        if (ex == null)
            return;
        var crashDir = Path.Combine(BasicDirectories.CacheDir, "crashes");
        if (!Directory.Exists(crashDir))
            Directory.CreateDirectory(crashDir);

        var time = DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss");
        var file = Path.Combine(crashDir, $"{time}.log");
        using var writer = new StreamWriter(file);
        writer.WriteLine($"Crash::: {ex.GetType().FullName}: {ex.Message}");
        writer.WriteLine();
        writer.WriteLine("----------------------------");
        writer.WriteLine($"Version: {Assembly.GetExecutingAssembly().GetName().Version}");
        writer.WriteLine($"OS: {Environment.OSVersion}");
        writer.WriteLine($"Framework: {AppDomain.CurrentDomain.SetupInformation.TargetFrameworkName}");
        writer.WriteLine($"Source: {ex.Source}");
        writer.WriteLine($"Thread Name: {Thread.CurrentThread.Name ?? "Unnamed"}");
        writer.WriteLine($"App Start Time: {Process.GetCurrentProcess().StartTime}");
        writer.WriteLine($"Exception Time: {DateTime.UtcNow}");
        writer.WriteLine($"Memory Usage: {Process.GetCurrentProcess().PrivateMemorySize64 / 1024 / 1024} MB");
        writer.WriteLine("----------------------------");
        writer.WriteLine();
        writer.WriteLine(ex);
        writer.Flush();
    }

    private static IBackend _backend = null;
}