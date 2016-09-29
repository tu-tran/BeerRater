namespace BeerRater
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using BeerRater.Providers;

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
                    Console.Error.WriteLine("Invalid arguments");
                    return;
                }

                var metas = provider.Get(args);
                Console.WriteLine($"Query contains {metas.Count} beer name(s)");

                var infos = new QueryQueue().Query(metas);
                if (infos == null)
                {
                    Console.Error.WriteLine("There is no data to process.");
                    return;
                }

                infos = infos.OrderByDescending(r => Parse(r.Overall)).ThenByDescending(r => Parse(r.WeightedAverage)).ThenBy(r => Parse(r.Price)).ThenBy(r => r.Name).ToList();
                var fileName = Path.GetFileNameWithoutExtension(metas.FilePath);
                var basePath = Path.GetDirectoryName(metas.FilePath);
                new Reporter(fileName, basePath).Generate(infos);

            }
            catch (Exception ex)
            {
                Console.WriteLine("======================================================================");
                var error = $"FATAL ERROR: {ex}";
                Console.Error.WriteLine(error);
                Trace.TraceError(error);
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Parses the specified target to double.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>The parsed double.</returns>
        private static double? Parse(string target)
        {
            double result;
            return double.TryParse(target, out result) ? result : (double?)null;
        }
    }
}