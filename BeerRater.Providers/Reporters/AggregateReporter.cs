namespace BeerRater.Providers.Reporters
{
    using System.Collections.Generic;
    using System.Linq;

    using BeerRater.Utils;

    using Process;

    /// <summary>
    /// AggregateReporter.
    /// </summary>
    public class AggregateReporter<TReporter> : IReporter where TReporter : IReporter
    {
        /// <summary>
        /// The instance
        /// </summary>
        public static readonly AggregateReporter<TReporter> Instance = new AggregateReporter<TReporter>();

        /// <summary>
        /// The reporters
        /// </summary>
        private readonly IReadOnlyList<TReporter> reporters;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateReporter{TReporter}" /> class.
        /// </summary>
        public AggregateReporter()
        {
            this.reporters = TypeExtensions.GetLoadedTypes<TReporter>();
        }

        /// <summary>
        /// Generates the reports based on the specified infos.
        /// </summary>
        /// <param name="session"></param>
        public void Generate(QuerySession session)
        {
            var orderedSession = new QuerySession(session.Name,
                session.OrderByDescending(r => r.Overall).ThenByDescending(r => r.WeightedAverage).ThenBy(r => r.Price).ThenBy(r => r.Name));

            foreach (var reporter in this.reporters)
            {
                reporter.Generate(orderedSession);
            }
        }
    }
}
