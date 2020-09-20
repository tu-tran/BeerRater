using BeerRater.Providers.Process;
using BeerRater.Providers.Reporters;

namespace BeerRater.Tests.Providers
{
    /// <summary>
    ///     The test reporter.
    /// </summary>
    public sealed class TestReporter : IReporter
    {
        /// <summary>
        ///     Gets the last session.
        /// </summary>
        public QuerySession LastSession { get; private set; }

        /// <summary>
        ///     Generates the reports based on the specified infos.
        /// </summary>
        /// <param name="session">The session.</param>
        public void Generate(QuerySession session)
        {
            LastSession = session;
        }
    }
}