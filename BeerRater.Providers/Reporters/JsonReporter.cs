namespace BeerRater.Providers.Reporters
{
    using Data;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.IO;

    using Process;

    /// <summary>
    /// The <see cref="JsonReporter"/> generates the JSON report.
    /// </summary>
    public class JsonReporter : IReporter
    {
        /// <inheritdoc />
        public void Generate(QuerySession session)
        {
            var basePath = Path.GetDirectoryName(session.Name);
            var target = Path.Combine(basePath, "JSON", session.Name + ".json");
            Directory.CreateDirectory(Path.GetDirectoryName(target) ?? string.Empty);
            File.WriteAllText(target, JsonConvert.SerializeObject(session, Formatting.Indented));
        }
    }
}
