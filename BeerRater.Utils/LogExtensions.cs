namespace BeerRater.Utils
{
    using System;

    /// <summary>
    /// The <see cref="LogExtensions"/> class provides extension methods for logging.
    /// </summary>
    public static class LogExtensions
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILogger Logger;

        static LogExtensions()
        {
            Logger = LogUtil.GetLogger();
        }

        /// <summary>
        /// Writes the specified input to output.
        /// </summary>
        /// <param name="input">The input.</param>
        public static void Output(this string input)
        {
            Logger.Info(input);
        }

        /// <summary>
        /// Writes the specified input to error output.
        /// </summary>
        /// <param name="input">The input.</param>
        public static void OutputError(this string input)
        {
            Logger.Error(input);
        }

        /// <summary>
        /// Writes the specified input to error output.
        /// </summary>
        /// <param name="exception">The input.</param>
        public static void OutputError(this Exception exception)
        {
            Logger.Error("Error: ", exception);
        }

        /// <summary>
        /// Writes the specified input to error output.
        /// </summary>
        /// <param name="exception">The input.</param>
        /// <param name="message">The message.</param>
        public static void OutputError(this Exception exception, string message)
        {
            Logger.Error(message, exception);
        }
    }
}
