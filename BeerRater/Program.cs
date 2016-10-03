namespace BeerRater
{
    using System;
    using System.IO;
    using System.Linq;

    using BeerRater.Processors;
    using BeerRater.Providers;
    using BeerRater.Utils;

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
                var provider = InputProviderResolver.Get(args);
                if (provider == null)
                {
                    "Invalid arguments".OutputError();
                    return;
                }

                var metas = provider.Get(args);
                $"Query contains {metas.Count} beer name(s)".Output();

                var infos = new RateQuery().Query(metas);
                if (infos == null)
                {
                    "There is no data to process.".OutputError();
                    return;
                }

                infos = infos.OrderByDescending(r => r.Overall).ThenByDescending(r => r.WeightedAverage).ThenBy(r => r.Price).ThenBy(r => r.Name).ToList();
                "Querying the reference prices...".Output();
                new ReferencePriceQuery().UpdateReferencePrices(infos);

                var fileName = Path.GetFileNameWithoutExtension(metas.FilePath);
                var basePath = Path.GetDirectoryName(metas.FilePath);
                new Reporter(fileName, basePath).Generate(infos);

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