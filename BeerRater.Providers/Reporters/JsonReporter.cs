namespace BeerRater.Providers.Reporters
{
    using Data;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// The <see cref="JsonReporter"/> generates the JSON report.
    /// </summary>
    public class JsonReporter : IReporter
    {
        /// <summary>
        /// Generates the reports based on the specified infos.
        /// </summary>
        /// <param name="infos">The infos.</param>
        /// <param name="basePath">The base path.</param>
        /// <param name="reportName">Name of the report.</param>
        public void Generate(IList<BeerInfo> infos, string basePath, string reportName)
        {
            var target = Path.Combine(basePath, "JSON", reportName + ".json");
            Directory.CreateDirectory(Path.GetDirectoryName(target) ?? string.Empty);
            File.WriteAllText(target, JsonConvert.SerializeObject(infos, Formatting.Indented));
        }
    }
}
