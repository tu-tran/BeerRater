namespace BeerRater.Console
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;

    using BeerRater.Data;
    using BeerRater.Providers;
    using BeerRater.Utils;

    using CommandLine;

    using Processors;

    using Providers;

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

                var comparer = new CustomEqualityComparer<BeerInfo>((a, b) => string.Compare(a.NameOnStore, b.NameOnStore, StringComparison.OrdinalIgnoreCase) == 0);
                var date = DateTime.UtcNow;
                foreach (var provider in providers)
                {
                    var beerInfos = provider.GetBeerMeta(args);
                    var session = new QuerySession($"{provider.Name}_{date:yyyyMMdd_hhmmss}", beerInfos.Distinct(comparer));
                    $"Query contains {beerInfos.Count} beer name(s)".Output();

                    if (appParams.IsRated ?? false)
                    {
                        beerInfos = new RateQuery().Query(beerInfos);
                        if (beerInfos == null)
                        {
                            "There is no data to process.".OutputError();
                            return;
                        }

                        beerInfos = beerInfos.OrderByDescending(r => r.Overall).ThenByDescending(r => r.WeightedAverage).ThenBy(r => r.Price).ThenBy(r => r.Name).ToList();
                    }

                    if (appParams.IsPriceCompared ?? false)
                    {
                        "Querying the reference prices...".Output();
                        new ReferencePriceQuery().UpdateReferencePrices(beerInfos);
                    }

                    var fileName = Path.GetFileNameWithoutExtension(session.FilePath);
                    var basePath = Path.GetDirectoryName(session.FilePath);
                    Reporter.Instance.Generate(beerInfos, basePath, fileName);
                }

            }
            catch (Exception ex)
            {
                "======================================================================".Output();
                $"FATAL ERROR: {ex}".OutputError();
                Console.ReadKey();
            }
        }
    }
}