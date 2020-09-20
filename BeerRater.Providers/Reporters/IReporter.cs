using BeerRater.Providers.Process;

namespace BeerRater.Providers.Reporters
{
    /// <summary>
    ///     The <see cref="IReporter" /> interfaces the reporter.
    /// </summary>
    public interface IReporter
    {
        /// <summary>
        ///     Generates the reports based on the specified infos.
        /// </summary>
        /// <param name="session">The session.</param>
        void Generate(QuerySession session);
    }
}