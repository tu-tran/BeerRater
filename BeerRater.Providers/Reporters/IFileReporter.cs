namespace BeerRater.Providers.Reporters
{
    using System.Collections.Generic;

    using Data;

    /// <summary>
    /// The <see cref="IFileReporter"/> interfaces the file reporter.
    /// </summary>
    public interface IFileReporter : IReporter
    {
        /// <summary>
        /// Generates the reports based on the specified infos.
        /// </summary>
        /// <param name="infos">The infos.</param>
        /// <param name="basePath">The base path.</param>
        /// <param name="reportName">Name of the report.</param>
        void Generate(IList<BeerInfo> infos, string basePath, string reportName);
    }
}