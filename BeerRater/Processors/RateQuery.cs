namespace BeerRater.Processors
{
    using System;
    using System.Collections.Generic;

    using BeerRater.Data;
    using BeerRater.Providers;
    using BeerRater.Utils;

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
            this.Queue.Start((m,i) => Query(m, result), metas);
            return result;
        }

        /// <summary>
        /// Queries the specified meta.
        /// </summary>
        /// <param name="meta">The meta.</param>
        /// <param name="result">The output result.</param>
        private static void Query(BeerMeta meta, List<BeerInfo> result)
        {
            var price = (meta.Price.HasValue ? meta.Price.ToString() : "").ToDouble();
            var info = RateBeerInfoProvider.Query(meta.Name);
            info.NameOnStore = meta.Name;
            info.Price = price;
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
