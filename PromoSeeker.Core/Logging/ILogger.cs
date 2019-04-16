using System.Runtime.CompilerServices;

namespace PromoSeeker.Core
{
    public interface ILogger
    {
        /// <summary>
        /// Checks if the given <paramref name="logLevel"/> is enabled for logging.
        /// </summary>
        /// <param name="logLevel">The log level to check.</param>
        /// <returns><see langword="true"/> if enabled, otherwise <see langword="false"/>.</returns>
        bool IsEnabled(LogLevel logLevel);

        /// <summary>
        /// Handles a single log message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="logLevel">The severity of the log message.</param>
        /// <param name="origin">The method/function this message was logged in.</param>
        /// <param name="filePath">The code filename that this message was logged from.</param>
        /// <param name="lineNumber">The line of code in the filename this message was logged from.</param>
        void Log(string message, LogLevel logLevel, [CallerMemberName] string origin = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0);
    }
}
