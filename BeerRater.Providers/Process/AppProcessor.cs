namespace BeerRater.Providers.Process
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using BeerRater.Utils;

    using Data;

    using Inputs;

    using Newtonsoft.Json.Serialization;

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
            new CustomEqualityComparer<BeerInfo>((a, b) => string.Compare(a.NameOnStore, b.NameOnStore, StringComparison.OrdinalIgnoreCase) == 0);

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
        public IResolver<IInputProvider> InputerResolver { get; set; }

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
            Debug.Assert(this.InputerResolver != null, $"Invalid {nameof(this.InputerResolver)}");
            Debug.Assert(this.ReporterResolver != null, $"Invalid {nameof(this.ReporterResolver)}");
            Debug.Assert(this.RaterResolver != null, $"Invalid {nameof(this.RaterResolver)}");
            Debug.Assert(this.PricerResolver != null, $"Invalid {nameof(this.PricerResolver)}");

            this.StartDate = DateTime.UtcNow;
            var rateQuery = new RateQuery(this.RaterResolver.Resolve(this.args));
            var priceQuery = new ReferencePriceQuery(this.PricerResolver.Resolve(this.args));
            this.Queue.Start((p, i) => this.GetBeerFromProvider(p, rateQuery, priceQuery), this.InputerResolver.Resolve(this.args));
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
            var session = new QuerySession($"{provider.Name}_{this.StartDate:yyyyMMdd_hhmmss}", beerInfos.Distinct(BeerNameComparer));
            this.Output($"{provider.Name} contains {session.Count} beer name(s)");

            var tasks = new List<Task>(2);
            if (this.appParams.IsRated ?? false)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    this.Output($"[{provider.Name}] Querying the beer ratings...");
                    rateQuery.Query(session);
                }));
            }

            if (this.appParams.IsPriceCompared ?? false)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    this.Output($"[{provider.Name}] Querying the reference prices...");
                    priceQuery.UpdateReferencePrices(beerInfos);
                }));
            }

            Task.WaitAll(tasks.ToArray());

            foreach (var reporter in this.ReporterResolver.Resolve(this.args))
            {
                reporter.Generate(session);
            }
        }
    }
}