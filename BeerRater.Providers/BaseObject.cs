namespace BeerRater.Providers
{
    using BeerRater.Utils;

    using System;

    /// <summary>
    /// The base object.
    /// </summary>
    public abstract class BaseObject
    {
        /// <summary>
        /// The this.logger.
        /// </summary>
        protected readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseObject"/> class.
        /// </summary>
        protected BaseObject()
        {
            this.logger = LogUtil.GetLogger(this.GetType().Name);
        }

        /// <summary>
        /// Writes the specified input to output.
        /// </summary>
        /// <param name="input">The input.</param>
        protected void Output(string input)
        {
            this.logger.Info(input);
        }

        /// <summary>
        /// Writes the specified input to error output.
        /// </summary>
        /// <param name="input">The input.</param>
        protected void OutputError(string input)
        {
            this.logger.Error(input);
        }

        /// <summary>
        /// Writes the specified input to error output.
        /// </summary>
        /// <param name="exception">The input.</param>
        protected void OutputError(Exception exception)
        {
            this.logger.Error("Error: ", exception);
        }

        /// <summary>
        /// Writes the specified input to error output.
        /// </summary>
        /// <param name="exception">The input.</param>
        /// <param name="message">The message.</param>
        protected void OutputError(Exception exception, string message)
        {
            this.logger.Error(message, exception);
        }
    }
}