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
                var infos = new QueryQueue().Query(metas);
                if (infos == null)
                {
                    Console.Error.WriteLine("There is no data to process.");
                    return;
                }

                infos = infos.OrderByDescending(r => Parse(r.OVERALL)).ThenByDescending(r => Parse(r.WEIGHTED_AVG)).ThenBy(r => Parse(r.PRICE)).ThenBy(r => r.NAME).ToList();
                var fileName = Path.GetFileNameWithoutExtension(metas.FilePath);
                var basePath = Path.GetDirectoryName(metas.FilePath);
                new Reporter(fileName, basePath).Generate(infos);

            }
            catch (Exception ex)
            {
                var error = $"FATAL ERROR: {ex}";
                Console.Error.WriteLine(error);
                Trace.TraceError(error);
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