namespace BeerRater.Utils
{
    public static class LogExtensions
    {
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
    }
}
