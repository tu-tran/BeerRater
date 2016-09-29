using System;
using System.Collections.Generic;

namespace BeerRater
{
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
            this.Queue.Start(m => Query(m, result), metas);
            return result;
        }

        /// <summary>
        /// Queries the specified meta.
        /// </summary>
        /// <param name="meta">The meta.</param>
        /// <param name="result">The output result.</param>
        private static void Query(BeerMeta meta, List<BeerInfo> result)
        {
            var price = meta.Price.HasValue ? meta.Price.ToString() : "-";
            var info = Crawler.Query(meta.Name);
            info.Price = price;
            info.ProductUrl = meta.ProductUrl;
            lock (result)
            {
                result.Add(info);
                Console.WriteLine($"{result.Count}. {info}");
            }
        }
    }
}
