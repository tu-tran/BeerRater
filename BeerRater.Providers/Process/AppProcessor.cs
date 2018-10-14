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
            Debug.Assert(InputResolver != null, $"Invalid {nameof(InputResolver)}");
            Debug.Assert(ReporterResolver != null, $"Invalid {nameof(ReporterResolver)}");
            Debug.Assert(RaterResolver != null, $"Invalid {nameof(RaterResolver)}");
            Debug.Assert(PricerResolver != null, $"Invalid {nameof(PricerResolver)}");

            StartDate = DateTime.UtcNow;
            var rateQuery = new RateQuery(RaterResolver.Resolve(args));
            var priceQuery = new ReferencePriceQuery(PricerResolver.Resolve(args));
            Queue.Start((p, i) => GetBeerFromProvider(p, rateQuery, priceQuery),
                InputResolver.Resolve(args));
        }

        /// <summary>
        /// Gets beer from provider.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="rateQuery">The rate query.</param>
        /// <param name="priceQuery">The price query.</param>
        private void GetBeerFromProvider(IInputProvider provider, RateQuery rateQuery, ReferencePriceQuery priceQuery)
        {
            var beerInfos = provider.GetBeerMeta(args);
            var sessionName = $"{provider.Name}_{StartDate:yyyyMMdd_hhmmss}";
            var session = new QuerySession(sessionName, beerInfos.Distinct(BeerNameComparer));
            Output($"{provider.Name} contains {session.Count} beer name(s)");

            session.Name = sessionName + ".Original";
            GenerateReports(session);
            var tasks = new List<Task>(2);
            var finalReport = false;
            if (appParams.IsRated ?? false)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    Output($"[{provider.Name}] Querying the beer ratings...");
                    rateQuery.Query(session);
                }));
                finalReport = true;
            }

            if (appParams.IsPriceCompared ?? false)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    Output($"[{provider.Name}] Querying the reference prices...");
                    priceQuery.UpdateReferencePrices(beerInfos);
                }));
                finalReport = true;
            }

            Task.WaitAll(tasks.ToArray());
            if (finalReport)
            {
                session.Name = sessionName;
                GenerateReports(session);
            }
        }

        private void GenerateReports(QuerySession session)
        {
            foreach (var reporter in ReporterResolver.Resolve(args))
            {
                reporter.Generate(session);
            }
        }
    }
}