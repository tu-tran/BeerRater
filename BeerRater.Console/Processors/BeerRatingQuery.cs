namespace BeerRater.Console.Processors
{
    using Data;
    using Providers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Utils;

    /// <summary>
    /// The query of beer rate.
    /// </summary>
    internal sealed class RateQuery : Multitask
    {
        /// <summary>
        /// Queries the specified metas.
        /// </summary>
        /// <param name="metas">The metas.</param>
        /// <returns>The beer infos.</returns>
        public void Query(IList<BeerInfo> metas)
        {
            this.Queue.Start((m, i) => ResolveRating(m, metas), metas);
        }

        /// <summary>
        /// Resolves rating.
        /// </summary>
        /// <param name="info">The beer information.</param>
        /// <param name="references">The references.</param>
        private static void ResolveRating(BeerInfo info, IList<BeerInfo> references)
        {
            var match = references.FirstOrDefault(i => string.Equals(i.Name, info.Name, StringComparison.OrdinalIgnoreCase));
            if (match != null && !string.IsNullOrEmpty(match.ReviewUrl))
            {
                info.Overall = match.Overall;
                info.ABV = match.ABV;
                info.Calories = match.Calories;
                info.Ratings = match.Ratings;
                info.ReviewUrl = match.ReviewUrl;
            }
            else
            {
                RatingsResolver.Instance.Query(info);
            }
        }
    }
}
