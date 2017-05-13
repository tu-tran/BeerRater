namespace BeerRater.Providers.Reporters
{
    using Data;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// The <see cref="StatReporter"/> generates the statistics report.
    /// </summary>
    public class StatReporter : IReporter
    {
        /// <summary>
        /// Generates the reports based on the specified infos.
        /// </summary>
        /// <param name="infos">The infos.</param>
        /// <param name="basePath">The base path.</param>
        /// <param name="reportName">Name of the report.</param>
        public void Generate(IList<BeerInfo> infos, string basePath, string reportName)
        {
            var target = Path.Combine(basePath, reportName) + ".stats";
            var rated = infos.Count(i => i.Ratings > 0.0);
            var unrated = infos.Count - rated;
            using (var writer = new StreamWriter(target))
            {
                writer.WriteLine($"Rated: {rated} - Unrated: {unrated} - Total: {infos.Count}");
            }
        }
    }
}
