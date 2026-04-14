namespace WaterBot.Framework;

/// <summary>
/// Static logging wrapper. Delegates to a configured log action (typically SMAPI's IMonitor).
/// Safe to call even when no monitor is configured (e.g., in unit tests) — calls are silently ignored.
/// </summary>
internal static class Logger
{
    private static Action<string, int>? _logAction;

    // Log level constants matching SMAPI's LogLevel enum values
    // Avoids a direct dependency on StardewModdingAPI in this file,
    // so the JIT doesn't try to load the assembly in test contexts.
    internal const int LevelTrace = 0;
    internal const int LevelDebug = 1;
    internal const int LevelInfo = 2;
    internal const int LevelWarn = 3;
    internal const int LevelError = 4;

    /// <summary>
    /// Configure the logging backend. Call once during mod entry.
    /// The action receives (message, logLevel) where logLevel matches SMAPI's LogLevel enum.
    /// </summary>
    public static void SetLogAction(Action<string, int> logAction)
    {
        _logAction ??= logAction;
    }

    /// <summary>Log a message at the specified level.</summary>
    public static void Log(string message, int level = LevelDebug)
    {
        _logAction?.Invoke(message, level);
    }

    /// <summary>Log a debug message.</summary>
    public static void Debug(string message) => Log(message, LevelDebug);

    /// <summary>Log an info message.</summary>
    public static void Info(string message) => Log(message, LevelInfo);

    /// <summary>Log a trace message.</summary>
    public static void Trace(string message) => Log(message, LevelTrace);

    /// <summary>Log a warning message.</summary>
    public static void Warn(string message) => Log(message, LevelWarn);
}
