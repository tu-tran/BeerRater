namespace BeerRater.Console.Processors
{
    using Data;
    using Providers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Utils;

    /// <summary>
    /// The reference referencePrice query.
    /// </summary>
    internal sealed class ReferencePriceQuery : Multitask
    {
        /// <summary>
        /// Updates the reference prices.
        /// </summary>
        /// <param name="infos">The infos.</param>
        public void UpdateReferencePrices(IList<BeerInfo> infos)
        {
            this.Queue.Start((m, i) => ResolveReferencePrice(m, i, infos), infos);
        }

        /// <summary>
        /// Resolves the reference referencePrice.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="index">The index.</param>
        /// <param name="references">The references.</param>
        private static void ResolveReferencePrice(BeerInfo info, int index, IList<BeerInfo> references)
        {
            $"[{info.DataSource}] {index}. Resolve reference price for {info.Name}".Output();
            var match = references.FirstOrDefault(i => string.Equals(i.Name, info.Name, StringComparison.OrdinalIgnoreCase));
            if (match != null && match.ReferencePrices != null)
            {
                foreach (var price in match.ReferencePrices)
                {
                    info.AddPrice(price);
                }
            }
            else
            {
                ReferencePriceResolver.Instance.UpdateReferencePrice(info);
            }
        }
    }
}
