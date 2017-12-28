namespace BeerRater.Console
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using BeerRater.Providers.Inputs;

    using Data;

    using Processors;

    using Utils;

    /// <summary>
    /// The <see cref="AppProcessor"/> class processes the requests.
    /// </summary>
    internal sealed class AppProcessor : Multitask
    {
        /// <summary>
        /// The beer name comparer.
        /// </summary>
        private static readonly CustomEqualityComparer<BeerInfo> BeerNameComparer =
            new CustomEqualityComparer<BeerInfo>((a, b) => string.Compare(a.NameOnStore, b.NameOnStore, StringComparison.OrdinalIgnoreCase) == 0);

        /// <summary>
        /// The application parameters.
        /// </summary>
        private readonly AppParameters appParams;

        /// <summary>
        /// The arguments.
        /// </summary>
        private readonly string[] args;

        /// <summary>
        /// The rate query.
        /// </summary>
        private readonly RateQuery rateQuery = new RateQuery();

        /// <summary>
        /// The price query.
        /// </summary>
        private readonly ReferencePriceQuery priceQuery = new ReferencePriceQuery();

        /// <summary>
        /// Gets the providers.
        /// </summary>
        public IList<IInputProvider> Providers { get; }

        /// <summary>
        /// Gets the start date.
        /// </summary>
        public DateTime StartDate { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppProcessor"/> class.
        /// </summary>
        /// <param name="providers">The providers.</param>
        /// <param name="appParams">The application parameters.</param>
        /// <param name="args">The arguments.</param>
        public AppProcessor(IList<IInputProvider> providers, AppParameters appParams, string[] args)
        {
            this.Providers = providers;
            this.appParams = appParams;
            this.args = args;
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        public void Execute()
        {
            this.StartDate = DateTime.UtcNow;
            this.Queue.Start((p, i) => this.GetBeerFromProvider(p), this.Providers);
        }

        /// <summary>
        /// Gets beer from provider.
        /// </summary>
        /// <param name="provider">The provider.</param>
        private void GetBeerFromProvider(IInputProvider provider)
        {
            var beerInfos = provider.GetBeerMeta(this.args);
            var session = new QuerySession($"{provider.Name}_{this.StartDate:yyyyMMdd_hhmmss}", beerInfos.Distinct(BeerNameComparer));
            $"{provider.Name} contains {beerInfos.Count} beer name(s)".Output();

            var tasks = new List<Task>(2);
            if (this.appParams.IsRated ?? false)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    $"[{provider.Name}] Querying the beer ratings...".Output();
                    this.rateQuery.Query(beerInfos);
                }));
            }

            if (this.appParams.IsPriceCompared ?? false)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    $"[{provider.Name}] Querying the reference prices...".Output();
                    this.priceQuery.UpdateReferencePrices(beerInfos);
                }));
            }

            Task.WaitAll(tasks.ToArray());

            var fileName = Path.GetFileNameWithoutExtension(session.FilePath);
            var basePath = Path.GetDirectoryName(session.FilePath);
            Reporter.Instance.Generate(beerInfos, basePath, fileName);
        }
    }
}