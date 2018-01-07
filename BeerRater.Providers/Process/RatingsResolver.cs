namespace BeerRater.Providers.Process
{
    using System.Collections.Generic;
    using System.Linq;

    using BeerRater.Utils;

    using Data;

    using Ratings;

    using BaseObject = Providers.BaseObject;

    /// <summary>
    /// Reporter.
    /// </summary>
    public class RatingsResolver : BaseObject
    {
        /// <summary>
        /// The resolvers.
        /// </summary>
        private readonly Dictionary<IRatingProvider, int> resolvers = new Dictionary<IRatingProvider, int>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RatingsResolver" /> class.
        /// </summary>
        public RatingsResolver(IEnumerable<IRatingProvider> providers)
        {
            foreach (var provider in providers)
            {
                this.resolvers.Add(provider, 0);
            }
        }

        /// <summary>
        /// Queries the specified beer information.
        /// </summary>
        /// <param name="beerInfo">The beer information.</param>
        public void Query(BeerInfo beerInfo)
        {
            IRatingProvider[] providers;
            lock (this)
            {
                providers = this.resolvers.OrderBy(r => r.Value).Select(p => p.Key).ToArray();
            }

            foreach (var provider in providers)
            {
                lock (this)
                {
                    this.resolvers[provider]++;
                }

                provider.Query(beerInfo);

                lock (this)
                {
                    this.resolvers[provider]--;
                }

                if (beerInfo.Overall.HasValue && beerInfo.Overall.Value > double.Epsilon)
                {
                    break;
                }
            }

            this.Output($"[{beerInfo.DataSource}] {beerInfo.Name} - {beerInfo.Overall}");
        }
    }
}
