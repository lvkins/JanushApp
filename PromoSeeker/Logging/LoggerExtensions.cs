using System.Runtime.CompilerServices;

namespace PromoSeeker
{
    /// <summary>
    /// The extension methods for the <see cref="ILogger"/>.
    /// </summary>
    internal static class LoggerExtensions
    {
        /// <summary>
        /// A shortcut method for logging a trace message.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="origin">The method/function this message was logged in.</param>
        /// <param name="filePath">The code filename that this message was logged from.</param>
        /// <param name="lineNumber">The line of code in the filename this message was logged from.</param>
        public static void Trace(this ILogger logger, string message, [CallerMemberName] string origin = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) => logger.Log(message, LogLevel.Trace, origin, filePath, lineNumber);

        /// <summary>
        /// A shortcut method for logging a debug message.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="origin">The method/function this message was logged in.</param>
        /// <param name="filePath">The code filename that this message was logged from.</param>
        /// <param name="lineNumber">The line of code in the filename this message was logged from.</param>
        public static void Debug(this ILogger logger, string message, [CallerMemberName] string origin = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) => logger.Log(message, LogLevel.Debug, origin, filePath, lineNumber);

        /// <summary>
        /// A shortcut method for logging a verbose message.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="origin">The method/function this message was logged in.</param>
        /// <param name="filePath">The code filename that this message was logged from.</param>
        /// <param name="lineNumber">The line of code in the filename this message was logged from.</param>
        public static void Verbose(this ILogger logger, string message, [CallerMemberName] string origin = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) => logger.Log(message, LogLevel.Verbose, origin, filePath, lineNumber);

        /// <summary>
        /// A shortcut method for logging a info message.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="origin">The method/function this message was logged in.</param>
        /// <param name="filePath">The code filename that this message was logged from.</param>
        /// <param name="lineNumber">The line of code in the filename this message was logged from.</param>
        public static void Info(this ILogger logger, string message, [CallerMemberName] string origin = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) => logger.Log(message, LogLevel.Info, origin, filePath, lineNumber);

        /// <summary>
        /// A shortcut method for logging a warning message.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="origin">The method/function this message was logged in.</param>
        /// <param name="filePath">The code filename that this message was logged from.</param>
        /// <param name="lineNumber">The line of code in the filename this message was logged from.</param>
        public static void Warning(this ILogger logger, string message, [CallerMemberName] string origin = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) => logger.Log(message, LogLevel.Warning, origin, filePath, lineNumber);

        /// <summary>
        /// A shortcut method for logging a error message.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="origin">The method/function this message was logged in.</param>
        /// <param name="filePath">The code filename that this message was logged from.</param>
        /// <param name="lineNumber">The line of code in the filename this message was logged from.</param>
        public static void Error(this ILogger logger, string message, [CallerMemberName] string origin = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) => logger.Log(message, LogLevel.Error, origin, filePath, lineNumber);

        /// <summary>
        /// A shortcut method for logging a fatal message.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="origin">The method/function this message was logged in.</param>
        /// <param name="filePath">The code filename that this message was logged from.</param>
        /// <param name="lineNumber">The line of code in the filename this message was logged from.</param>
        public static void Fatal(this ILogger logger, string message, [CallerMemberName] string origin = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) => logger.Log(message, LogLevel.Fatal, origin, filePath, lineNumber);
    }
}
