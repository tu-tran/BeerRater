﻿using System;
using System.Collections.Generic;
using System.Linq;
using BeerRater.Data;
using BeerRater.Providers.Ratings;

namespace BeerRater.Providers.Process
{
    /// <summary>
    ///     The query of beer rate.
    /// </summary>
    internal sealed class RateQuery : Multitask
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RateQuery" /> class.
        /// </summary>
        /// <param name="raters">The raters.</param>
        public RateQuery(IReadOnlyList<IRatingProvider> raters)
        {
            Resolver = new RatingsResolver(raters);
        }

        /// <summary>
        ///     Gets the raters.
        /// </summary>
        public RatingsResolver Resolver { get; }

        /// <summary>
        ///     Queries the specified metas.
        /// </summary>
        /// <param name="metas">The metas.</param>
        /// <returns>The beer infos.</returns>
        public void Query(IReadOnlyList<BeerInfo> metas)
        {
            Queue.Start((m, i) => ResolveRating(m, metas), metas);
        }

        /// <summary>
        ///     Resolves rating.
        /// </summary>
        /// <param name="info">The beer information.</param>
        /// <param name="references">The references.</param>
        private void ResolveRating(BeerInfo info, IReadOnlyList<BeerInfo> references)
        {
            var match = references.FirstOrDefault(i =>
                i != info &&
                (string.Equals(i.Name, info.Name, StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(i.NameOnStore, info.NameOnStore, StringComparison.OrdinalIgnoreCase)));

            if (!string.IsNullOrEmpty(match?.ReviewUrl))
            {
                info.Overall = match.Overall;
                info.ABV = match.ABV;
                info.Calories = match.Calories;
                info.Ratings = match.Ratings;
                info.ReviewUrl = match.ReviewUrl;
            }
            else
            {
                Resolver.Query(info);
            }
        }
    }
}