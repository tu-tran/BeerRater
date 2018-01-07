namespace BeerRater.Providers.Reporters
{
    using Process;

    /// <summary>
    /// The <see cref="IReporter"/> interfaces the reporter.
    /// </summary>
    public interface IReporter
    {
        /// <summary>
        /// Generates the reports based on the specified infos.
        /// </summary>
        /// <param name="session">The session.</param>
        void Generate(QuerySession session);
    }
}