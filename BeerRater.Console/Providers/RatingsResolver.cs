namespace BeerRater.Console.Providers
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using BeerRater.Providers.Ratings;

    using Data;

    using Utils;

    /// <summary>
    /// Reporter.
    /// </summary>
    internal class RatingsResolver
    {
        /// <summary>
        /// The instance
        /// </summary>
        public static readonly RatingsResolver Instance = new RatingsResolver();

        /// <summary>
        /// The resolvers
        /// </summary>
        private readonly Dictionary<IRatingProvider, int> resolvers = new Dictionary<IRatingProvider, int>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RatingsResolver" /> class.
        /// </summary>
        private RatingsResolver()
        {
            var providers = TypeExtensions.GetLoadedTypes<IRatingProvider>();
            foreach (var provider in providers)
            {
                this.resolvers.Add(provider, 0);
            }
        }

        /// <summary>
        /// Queries the specified beer name.
        /// </summary>
        /// <param name="beerName">Name of the beer.</param>
        /// <returns></returns>
        public BeerInfo Query(string beerName)
        {
            IRatingProvider[] providers;
            lock (this)
            {
                providers = this.resolvers.OrderBy(r => r.Value).Select(p => p.Key).ToArray();
            }

            BeerInfo candidate = null;
            foreach (var provider in providers)
            {
                Trace.WriteLine($"Getting ratings from {provider}");
                lock (this)
                {
                    this.resolvers[provider]++;
                }

                var info = provider.Query(beerName);

                lock (this)
                {
                    this.resolvers[provider]--;
                }

                if (info != null)
                {
                    candidate = info;
                    if (info.Overall > 0.0)
                    {
                        break;
                    }
                }
            }

            return candidate;
        }
    }
}
