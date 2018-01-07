namespace BeerRater.Tests.Providers
{
    using BeerRater.Providers.Process;
    using BeerRater.Providers.Reporters;

    /// <summary>
    /// The test reporter.
    /// </summary>
    public sealed class TestReporter : IReporter
    {
        /// <summary>
        /// Gets the last session.
        /// </summary>
        public QuerySession LastSession { get; private set; }

        /// <summary>
        /// Generates the reports based on the specified infos.
        /// </summary>
        /// <param name="session">The session.</param>
        public void Generate(QuerySession session)
        {
            this.LastSession = session;
        }
    }
}
