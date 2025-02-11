using StardewModdingAPI;

namespace BotFramework
{
    /// <summary>
    /// Static method of printing to <see cref="IMonitor"/>.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Static reference to <see cref="IMonitor"/>.
        /// </summary>
        private static IMonitor? Monitor;

        /// <summary>
        /// Sets static reference to <see cref="IMonitor"/>.
        /// </summary>
        /// <param name="monitor"><see cref="IMonitor"/> reference.</param>
        public static void SetMonitor(IMonitor monitor)
        {
            if (Logger.Monitor == null)
            {
                Logger.Monitor = monitor;
            }
        }

        /// <summary>
        /// Prints to a given <see cref="LogLevel"/> of <see cref="IMonitor"/>.
        /// </summary>
        /// <param name="message">Message to be printed.</param>
        /// <param name="level"><see cref="LogLevel"/> to print to.</param>
        public static void Log(string message, LogLevel level = LogLevel.Debug)
        {
            Logger.Monitor?.Log(message, level);
        }

        /// <summary>
        /// Prints to <see cref="LogLevel.Debug"/> of <see cref="IMonitor"/>.
        /// </summary>
        /// <param name="message">Message to be printed.</param>
        public static void Debug(string message)
        {
            Logger.Log(message, LogLevel.Debug);
        }

        /// <summary>
        /// Prints to <see cref="LogLevel.Info"/> of <see cref="IMonitor"/>.
        /// </summary>
        /// <param name="message">Message to be printed.</param>
        public static void Info(string message)
        {
            Logger.Log(message, LogLevel.Info);
        }

        /// <summary>
        /// Prints to <see cref="LogLevel.Trace"/> of <see cref="IMonitor"/>.
        /// </summary>
        /// <param name="message">Message to be printed.</param>
        public static void Trace(string message)
        {
            Logger.Log(message, LogLevel.Trace);
        }
    }
}
