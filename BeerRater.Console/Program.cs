namespace BeerRater.Console
{
    using System;
    using System.Configuration;

    using CommandLine;

    using Providers;

    using Utils;

    /// <summary>
    /// The beer rater.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Main app.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void Main(string[] args)
        {
            try
            {
                System.AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
                var parserResult = CommandLine.Parser.Default.ParseArguments<AppParameters>(args);
                var appParams = parserResult.Tag == ParserResultType.Parsed ? ((Parsed<AppParameters>)parserResult).Value : new AppParameters();
                appParams.Initialize(ConfigurationManager.AppSettings);

                if (appParams.ThreadsCount.HasValue && appParams.ThreadsCount.Value > 0)
                {
                    Multitask.PoolSize = appParams.ThreadsCount.Value;
                }

                var providers = InputProviderResolver.Get(args);
                if (providers == null || providers.Count < 1)
                {
                    "Invalid command line arguments".OutputError();
                    return;
                }

                new AppProcessor(providers, appParams, args).Execute();
            }
            catch (Exception ex)
            {
                "======================================================================".Output();
                ex.OutputError();
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Currents domain on unhandled exception.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs eventArgs)
        {
            var message = sender?.ToString() ?? "Error: ";
            var exception = eventArgs.ExceptionObject as Exception;
            if (exception == null)
            {
                "FATAL ERROR: Application crashing".OutputError();
            }
            else
            {
                exception.OutputError(message);
            }
        }
    }
}