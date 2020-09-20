using System;
using BeerRater.Console.Providers;
using BeerRater.Providers;
using BeerRater.Providers.Pricings;
using BeerRater.Providers.Process;
using BeerRater.Providers.Ratings;
using BeerRater.Providers.Reporters;
using BeerRater.Utils;
using CommandLine;

namespace BeerRater.Console
{
    /// <summary>
    ///     The beer rater.
    /// </summary>
    public class Program
    {
        /// <summary>
        ///     The logger.
        /// </summary>
        private static readonly ILogger Logger = LogUtil.GetLogger();

        /// <summary>
        ///     Main app.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void Main(string[] args)
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
                var parserResult = Parser.Default.ParseArguments<AppParameters>(args);
                var appParams = parserResult.Tag == ParserResultType.Parsed
                    ? ((Parsed<AppParameters>) parserResult).Value
                    : new AppParameters();
                appParams.Initialize();

                GetApp(appParams, args).Execute();
            }
            catch (Exception ex)
            {
                Logger.Error("======================================================================");
                Logger.Error("Error: ", ex);
                System.Console.ReadKey();
            }
        }

        /// <summary>
        ///     Gets application.
        /// </summary>
        /// <param name="appParams">The application parameters.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>
        ///     The application processor.
        /// </returns>
        public static AppProcessor GetApp(IAppParameters appParams, params string[] args)
        {
            return new AppProcessor(appParams, args)
            {
                InputResolver = new ConsoleInputProviderResolver(),
                ReporterResolver = new ResolverList<IReporter>(AggregateReporter<IReporter>.Instance),
                PricerResolver = new ReflectionResolver<IPriceProvider>(),
                RaterResolver = new ReflectionResolver<IRatingProvider>()
            };
        }

        /// <summary>
        ///     Currents domain on unhandled exception.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="UnhandledExceptionEventArgs" /> instance containing the event data.</param>
        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs eventArgs)
        {
            var message = sender?.ToString() ?? "Error: ";
            var exception = eventArgs.ExceptionObject as Exception;
            if (exception == null)
                Logger.Error("FATAL ERROR: Application crashing");
            else
                Logger.Error("Error: " + message);
        }
    }
}