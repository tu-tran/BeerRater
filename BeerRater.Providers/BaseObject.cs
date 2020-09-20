using System;
using BeerRater.Utils;

namespace BeerRater.Providers
{
    /// <summary>
    ///     The base object.
    /// </summary>
    public abstract class BaseObject
    {
        /// <summary>
        ///     The this.logger.
        /// </summary>
        protected readonly ILogger logger;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseObject" /> class.
        /// </summary>
        protected BaseObject()
        {
            logger = LogUtil.GetLogger(GetType().Name);
        }

        /// <summary>
        ///     Writes the specified input to output.
        /// </summary>
        /// <param name="input">The input.</param>
        protected void Output(string input)
        {
            logger.Info(input);
        }

        /// <summary>
        ///     Writes the specified input to error output.
        /// </summary>
        /// <param name="input">The input.</param>
        protected void OutputError(string input)
        {
            logger.Error(input);
        }

        /// <summary>
        ///     Writes the specified input to error output.
        /// </summary>
        /// <param name="exception">The input.</param>
        protected void OutputError(Exception exception)
        {
            logger.Error("Error: ", exception);
        }

        /// <summary>
        ///     Writes the specified input to error output.
        /// </summary>
        /// <param name="exception">The input.</param>
        /// <param name="message">The message.</param>
        protected void OutputError(Exception exception, string message)
        {
            logger.Error(message, exception);
        }
    }
}