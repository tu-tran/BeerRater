namespace BeerRater.Console.Providers
{
    using System.Collections.Generic;

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
        private readonly IList<IRatingProvider> resolvers;

        /// <summary>
        /// Initializes a new instance of the <see cref="RatingsResolver" /> class.
        /// </summary>
        private RatingsResolver()
        {
            this.resolvers = TypeExtensions.GetLoadedTypes<IRatingProvider>();
        }

        /// <summary>
        /// Queries the specified beer name.
        /// </summary>
        /// <param name="beerName">Name of the beer.</param>
        /// <returns></returns>
        public BeerInfo Query(string beerName)
        {
            BeerInfo candidate = null;
            foreach (var provider in this.resolvers)
            {
                var info = provider.Query(beerName);
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
