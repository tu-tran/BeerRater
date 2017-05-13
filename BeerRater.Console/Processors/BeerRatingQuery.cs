namespace BeerRater.Console.Processors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BeerRater.Providers.Ratings;

    using Data;

    using Providers;

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
        public List<BeerInfo> Query(IList<BeerMeta> metas)
        {
            var result = new List<BeerInfo>();
            this.Queue.Start((m, i) => Query(m, result), metas);
            return result;
        }

        /// <summary>
        /// Queries the specified meta.
        /// </summary>
        /// <param name="meta">The meta.</param>
        /// <param name="result">The output result.</param>
        private static void Query(BeerMeta meta, List<BeerInfo> result)
        {
            var match = result.FirstOrDefault(i => string.Equals(i.Name, meta.Name, StringComparison.OrdinalIgnoreCase));
            BeerInfo info;
            if (match != null)
            {
                info = match.Clone();
            }
            else
            {
                info = RatingsResolver.Instance.Query(meta.Name) ?? meta.ToInfo();
            }

            info.NameOnStore = meta.NameOnStore;
            info.Price = (meta.Price.HasValue ? meta.Price.ToString() : string.Empty).ToDouble();
            info.ProductUrl = meta.ProductUrl;
            info.ImageUrl = meta.ImageUrl;
            lock (result)
            {
                result.Add(info);
                $"{result.Count}. {info}".Output();
            }
        }
    }
}
