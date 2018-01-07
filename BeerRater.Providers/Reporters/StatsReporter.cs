namespace BeerRater.Providers.Reporters
{
    using Process;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// The <see cref="StatsReporter"/> generates the statistics report.
    /// </summary>
    public class StatsReporter : IReporter
    {
        /// <inheritdoc />
        public void Generate(QuerySession session)
        {
            var basePath = Path.GetDirectoryName(session.Name);
            var target = Path.Combine(basePath, "Stats", session.Name + ".stats");
            Directory.CreateDirectory(Path.GetDirectoryName(target) ?? string.Empty);
            var rated = session.Count(i => i.Ratings > 0.0);
            var unrated = session.Count - rated;
            using (var writer = new StreamWriter(target))
            {
                writer.WriteLine($"Rated: {rated} - Unrated: {unrated} - Total: {session.Count}");
            }
        }
    }
}