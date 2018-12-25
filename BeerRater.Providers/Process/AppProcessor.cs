namespace BeerRater.Providers.Process
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using BeerRater.Utils;

    using Data;

    using Inputs;

    using Pricings;

    using Ratings;

    using Reporters;

    /// <summary>
    /// The <see cref="AppProcessor"/> class processes the requests.
    /// </summary>
    public sealed class AppProcessor : Multitask
    {
        /// <summary>
        /// The beer name comparer.
        /// </summary>
        private static readonly CustomEqualityComparer<BeerInfo> BeerNameComparer =
            new CustomEqualityComparer<BeerInfo>((a, b) =>
                string.Compare(a.NameOnStore, b.NameOnStore, StringComparison.OrdinalIgnoreCase) == 0);

        /// <summary>
        /// The application parameters.
        /// </summary>
        private readonly IAppParameters appParams;

        /// <summary>
        /// The arguments.
        /// </summary>
        private readonly string[] args;

        /// <summary>
        /// Gets or sets the input providers resolver.
        /// </summary>
        public IResolver<IInputProvider> InputResolver { get; set; }

        /// <summary>
        /// Gets or sets the inputs resolver.
        /// </summary>
        public IResolver<IReporter> ReporterResolver { get; set; }

        /// <summary>
        /// Gets or sets the rater resolver.
        /// </summary>
        public IResolver<IRatingProvider> RaterResolver { get; set; }

        /// <summary>
        /// Gets or sets the pricer resolver.
        /// </summary>
        public IResolver<IPriceProvider> PricerResolver { get; set; }

        /// <summary>
        /// Gets the start date.
        /// </summary>
        public DateTime StartDate { get; private set; }

        /// <inheritdoc />
        public AppProcessor(IAppParameters appParams, string[] args)
        {
            this.appParams = appParams;
            this.args = args;
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        public void Execute()
        {
            Debug.Assert(this.InputResolver != null, $"Invalid {nameof(this.InputResolver)}");
            Debug.Assert(this.ReporterResolver != null, $"Invalid {nameof(this.ReporterResolver)}");
            Debug.Assert(this.RaterResolver != null, $"Invalid {nameof(this.RaterResolver)}");
            Debug.Assert(this.PricerResolver != null, $"Invalid {nameof(this.PricerResolver)}");

            this.StartDate = DateTime.UtcNow;

            Multitask.PoolSize = this.appParams.ThreadsCount;

            var rateQuery = new RateQuery(this.RaterResolver.Resolve(this.appParams, this.args));
            var priceQuery = new ReferencePriceQuery(this.PricerResolver.Resolve(this.appParams, this.args));
            this.Queue.Start((p, i) => this.GetBeerFromProvider(p, rateQuery, priceQuery),
                this.InputResolver.Resolve(this.appParams, this.args));
        }

        /// <summary>
        /// Gets beer from provider.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="rateQuery">The rate query.</param>
        /// <param name="priceQuery">The price query.</param>
        private void GetBeerFromProvider(IInputProvider provider, RateQuery rateQuery, ReferencePriceQuery priceQuery)
        {
            var beerInfos = provider.GetBeerMeta(this.args);
            var sessionName = $"{provider.Name}_{this.StartDate:yyyyMMdd_hhmmss}";
            var session = new QuerySession(sessionName, beerInfos.Distinct(BeerNameComparer));
            this.Output($"{provider.Name} contains {session.Count} beer name(s)");

            this.GenerateReports(session);
            var tasks = new List<Task>(2);
            var finalReport = false;
            if (this.appParams.IsRated)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    this.Output($"[{provider.Name}] Querying the beer ratings...");
                    rateQuery.Query(session);
                }));
                finalReport = true;
            }

            if (this.appParams.IsPriceCompared)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    this.Output($"[{provider.Name}] Querying the reference prices...");
                    priceQuery.UpdateReferencePrices(beerInfos);
                }));
                finalReport = true;
            }

            Task.WaitAll(tasks.ToArray());
            if (finalReport)
            {
                session.Name = sessionName;
                this.GenerateReports(session);
            }
        }

        /// <summary>
        /// Generates the reports.
        /// </summary>
        /// <param name="session">The session.</param>
        private void GenerateReports(QuerySession session)
        {
            foreach (var reporter in this.ReporterResolver.Resolve(this.appParams, this.args))
            {
                reporter.Generate(session);
            }
        }
    }
}