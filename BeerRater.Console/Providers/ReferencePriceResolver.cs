namespace BeerRater.Console.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using BeerRater.Providers;
    using BeerRater.Providers.Pricings;

    using Data;

    using Utils;

    /// <summary>
    /// The price reference comparer.
    /// </summary>
    internal sealed class ReferencePriceResolver
    {
        public static readonly ReferencePriceResolver Instance = new ReferencePriceResolver();

        /// <summary>
        /// The providers.
        /// </summary>
        private readonly List<IPriceProvider> providers = new List<IPriceProvider>();

        /// <summary>
        /// Initializes the <see cref="ReferencePriceResolver"/> class.
        /// </summary>
        private ReferencePriceResolver()
        {
            this.providers.AddRange(TypeExtensions.GetLoadedTypes<IPriceProvider>());
        }

        /// <summary>
        /// Updates reference price.
        /// </summary>
        /// <param name="info">The information.</param>
        public void UpdateReferencePrice(BeerInfo info)
        {
            foreach (var priceProvider in this.providers)
            {
                var price = priceProvider.GetPrice(info);
                if (price != null)
                {
                    info.AddPrice(price);
                    return;
                }
            }
        }
    }
}
