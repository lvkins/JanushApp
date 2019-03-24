using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace PromoSeeker
{
    /// <summary>
    /// The severity of the log message.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// All the details. 
        /// </summary>
        Trace = 1,

        /// <summary>
        /// Developer-specific messages.
        /// </summary>
        Debug = 2,

        /// <summary>
        /// Verbose messages.
        /// </summary>
        Verbose = 3,

        /// <summary>
        /// Informative messages.
        /// </summary>
        Info = 4,

        /// <summary>
        /// Warning messages.
        /// </summary>
        Warning = 5,

        /// <summary>
        /// Failure messages.
        /// </summary>
        Error = 6,

        /// <summary>
        /// Catastrophic messages.
        /// </summary>
        Fatal = 7,
    }

    /// <summary>
    /// The configuration for the <see cref="Logger"/>.
    /// </summary>
    public class LoggerConfiguration
    {
        /// <summary>
        /// The <see cref="DateTimeFormat"/> for use in the <see cref="DateTime"/> to properly format the date.
        /// </summary>
        public string DateFormat = "yyyy-MM-dd hh:mm:ss.fff";

        /// <summary>
        /// The minimum log level of a log to be processed.
        /// </summary>
        public LogLevel LogLevel { get; set; } = LogLevel.Trace;

        /// <summary>
        /// Whether to write the message to the debug listeners using <see cref="System.Diagnostics.Debug.WriteLine(string)"/>.
        /// </summary>
        public bool PrintDebug { get; set; } = true;

        /// <summary>
        /// If the log message should contain the time it's being logged.
        /// </summary>
        public bool PrefixLogTime { get; set; } = true;

        /// <summary>
        /// If the log message should contain the log level.
        /// </summary>
        public bool PrefixLogLevel { get; set; } = true;

        /// <summary>
        /// Whether if details like the member name, line number should be added to the log line.
        /// </summary>
        public bool AppendCallerDetails { get; set; } = true;
    }

    /// <summary>
    /// A simple class for processing logs.
    /// </summary>
    public class Logger : ILogger
    {
        #region Private Members

        /// <summary>
        /// The path of the log file.
        /// </summary>
        private readonly string _filePath;

        /// <summary>
        /// The directory where the log file should be put.
        /// </summary>
        private readonly string _fileDir;

        /// <summary>
        /// The configuration of this logging instance.
        /// </summary>
        private readonly LoggerConfiguration _configuration;

        /// <summary>
        /// A lock for the thread safe log file access.
        /// </summary>
        private readonly object _loggerLock = new object();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="filePath">The path of the log file.</param>
        /// <param name="configuration">The configuration to use for logging.</param>
        public Logger(string filePath, LoggerConfiguration configuration = null)
        {
            _filePath = filePath;
            _fileDir = Path.GetDirectoryName(filePath);
            _configuration = configuration ?? new LoggerConfiguration();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks if the given <paramref name="logLevel"/> is enabled for logging.
        /// </summary>
        /// <param name="logLevel">The log level to check.</param>
        /// <returns><see langword="true"/> if enabled, otherwise <see langword="false"/>.</returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _configuration.LogLevel;
        }

        /// <summary>
        /// Handles a single log message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="logLevel">The severity of the log message.</param>
        /// <param name="origin">The method/function this message was logged in.</param>
        /// <param name="filePath">The code filename that this message was logged from.</param>
        /// <param name="lineNumber">The line of code in the filename this message was logged from.</param>
        public void Log(string message, LogLevel logLevel, [CallerMemberName] string origin = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            // If the log level is too low...
            if (!IsEnabled(logLevel))
            {
                // Do nothing
                return;
            }

            // Prepare the output
            var output = new StringBuilder();

            // If the log time should be a part of the message...
            if (_configuration.PrefixLogTime)
            {
                output.Append("[" + DateTime.Now.ToString(_configuration.DateFormat) + "] ");
            }

            // If the log level should be a part of the message...
            if (_configuration.PrefixLogLevel)
            {
                output.Append("<" + logLevel + "> ");
            }

            // Append message itself
            output.Append(message);

            // If the caller details should be appended...
            if (_configuration.AppendCallerDetails)
            {
                output.Append(" > " + Path.GetFileName(filePath));
                output.Append("::" + origin + "()");
                output.Append(":" + lineNumber);
            }

            // Append line terminator
            output.AppendLine();

            // Lock for thread safety and single access
            lock (_loggerLock)
            {
                // If the log directory doesn't exist...
                if (!Directory.Exists(_fileDir))
                {
                    // Create log directory
                    Directory.CreateDirectory(_fileDir);
                }

                // Write the message to file
                using (var writer = new StreamWriter(_filePath, true))
                {
                    writer.Write(output);
                }

                // If debug info should be printed...
                if (_configuration.PrintDebug)
                {
                    Debug.WriteLine("> " + output);
                }
            }
        }

        #endregion
    }
}
