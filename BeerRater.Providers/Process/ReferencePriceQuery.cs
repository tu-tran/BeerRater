using System;
using System.Collections.Generic;
using System.Linq;
using BeerRater.Data;
using BeerRater.Providers.Pricings;

namespace BeerRater.Providers.Process
{
    /// <summary>
    ///     The reference referencePrice query.
    /// </summary>
    internal sealed class ReferencePriceQuery : Multitask
    {
        /// <summary>
        ///     The providers.
        /// </summary>
        private readonly IEnumerable<IPriceProvider> providers;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReferencePriceQuery" /> class.
        /// </summary>
        /// <param name="providers">The providers.</param>
        public ReferencePriceQuery(IEnumerable<IPriceProvider> providers)
        {
            this.providers = providers;
        }

        /// <summary>
        ///     Updates the reference prices.
        /// </summary>
        /// <param name="infos">The infos.</param>
        public void UpdateReferencePrices(IReadOnlyList<BeerInfo> infos)
        {
            Queue.Start((m, i) => ResolveReferencePrice(m, i, infos), infos);
        }

        /// <summary>
        ///     Resolves the reference referencePrice.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="index">The index.</param>
        /// <param name="references">The references.</param>
        private void ResolveReferencePrice(BeerInfo info, int index, IReadOnlyList<BeerInfo> references)
        {
            Output($"[{info.DataSource}] {index}. Resolve reference price for {info.Name}");
            var match = references.FirstOrDefault(i =>
                string.Equals(i.Name, info.Name, StringComparison.OrdinalIgnoreCase));
            if (match != null && match.ReferencePrices != null)
                foreach (var price in match.ReferencePrices)
                    info.AddPrice(price);
            else
                foreach (var provider in providers)
                    provider.Update(info);
        }
    }
}