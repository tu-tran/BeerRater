namespace BeerRater.Console.Processors
{
    using System.Collections.Generic;
    using System.Diagnostics;

    using Data;

    using Providers;

    using Utils;

    /// <summary>
    /// The reference price query.
    /// </summary>
    internal sealed class ReferencePriceQuery : Multitask
    {
        /// <summary>
        /// Updates the reference prices.
        /// </summary>
        /// <param name="infos">The infos.</param>
        public void UpdateReferencePrices(IList<BeerInfo> infos)
        {
            this.Queue.Start(ResolveReferencePrice, infos);
        }

        /// <summary>
        /// Resolves the reference price.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="index">The index.</param>
        private static void ResolveReferencePrice(BeerInfo info, int index)
        {
            ReferencePriceResolver.UpdateReferencePrice(info);
            $"{index}. {info.Name} - [{info.Price}] <-> [{info.ReferencePrice}]".Output();
            if (string.IsNullOrEmpty(info.ReferencePriceUrl))
            {
                Trace.TraceError($"Failed to resolve reference price for [{info.Name}]");
            }
        }
    }
}
