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
                    "Invalid arguments".OutputError();
                    return;
                }

                new AppProcessor(providers, appParams, args).Execute();
            }
            catch (Exception ex)
            {
                "======================================================================".Output();
                $"FATAL ERROR: {ex}".OutputError();
                System.Console.ReadKey();
            }
        }
    }
}