namespace BeerRater.Console
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using BeerRater.Data;
    using BeerRater.Processors;
    using BeerRater.Providers;
    using BeerRater.Utils;

    using CommandLine;

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
                var providers = InputProviderResolver.Get(args);
                if (providers == null || providers.Count < 1)
                {
                    "Invalid arguments".OutputError();
                    return;
                }

                var parserResult = CommandLine.Parser.Default.ParseArguments<AppParameters>(args);
                var appParams = parserResult.Tag == ParserResultType.Parsed ? ((Parsed<AppParameters>)parserResult).Value : new AppParameters();
                var comparer = new CustomEqualityComparer<BeerMeta>((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase) == 0);
                foreach (var provider in providers)
                {
                    var metas = provider.Get(args);
                    metas = new QuerySession(metas.FilePath, metas.Distinct(comparer));
                    $"Query contains {metas.Count} beer name(s)".Output();
                    List<BeerInfo> infos;

                    if (!appParams.SkipRating)
                    {
                        infos = new RateQuery().Query(metas);
                        if (infos == null)
                        {
                            "There is no data to process.".OutputError();
                            return;
                        }

                        infos = infos.OrderByDescending(r => r.Overall).ThenByDescending(r => r.WeightedAverage).ThenBy(r => r.Price).ThenBy(r => r.Name).ToList();
                    }
                    else
                    {
                        infos = metas.Select(m => m.ToInfo()).OrderBy(i => i.Name).ToList();
                    }

                    if (!appParams.SkipPriceCompare)
                    {
                        "Querying the reference prices...".Output();
                        new ReferencePriceQuery().UpdateReferencePrices(infos);
                    }

                    var fileName = Path.GetFileNameWithoutExtension(metas.FilePath);
                    var basePath = Path.GetDirectoryName(metas.FilePath);
                    new Reporter(fileName, basePath).Generate(infos);
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