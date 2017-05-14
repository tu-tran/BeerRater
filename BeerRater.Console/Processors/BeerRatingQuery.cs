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
        public List<BeerInfo> Query(IList<BeerInfo> metas)
        {
            var result = new List<BeerInfo>();
            this.Queue.Start((m, i) => Query(m, result), metas);
            return result;
        }

        /// <summary>
        /// Queries the specified info.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="result">The output result.</param>
        private static void Query(BeerInfo info, List<BeerInfo> result)
        {
            var match = result.FirstOrDefault(i => string.Equals(i.Name, info.Name, StringComparison.OrdinalIgnoreCase));
            BeerInfo resultInfo;
            if (match != null)
            {
                resultInfo = match.Clone();
            }
            else
            {
                resultInfo = RatingsResolver.Instance.Query(info.Name) ?? info;
            }

            resultInfo.NameOnStore = info.NameOnStore;
            resultInfo.Price = (info.Price.HasValue ? info.Price.ToString() : string.Empty).ToDouble();
            resultInfo.ProductUrl = info.ProductUrl;
            resultInfo.ImageUrl = info.ImageUrl;
            lock (result)
            {
                result.Add(resultInfo);
                $"{result.Count}. {resultInfo}".Output();
            }
        }
    }
}
