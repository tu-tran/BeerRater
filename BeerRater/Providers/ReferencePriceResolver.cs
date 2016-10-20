namespace BeerRater.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using BeerRater.Data;

    /// <summary>
    /// The price reference comparer.
    /// </summary>
    internal sealed class ReferencePriceResolver
    {
        /// <summary>
        /// The providers.
        /// </summary>
        private static readonly List<IPriceProvider> Providers = new List<IPriceProvider>();

        /// <summary>
        /// Initializes the <see cref="ReferencePriceResolver"/> class.
        /// </summary>
        static ReferencePriceResolver()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(asm => asm.GetTypes().Where(t => typeof(IPriceProvider).IsAssignableFrom(t)));
            foreach (var type in types)
            {
                try
                {
                    var instance = (IPriceProvider)Activator.CreateInstance(type);
                    Providers.Add(instance);
                }
                catch (Exception ex)
                {
                    Trace.TraceError($"Failed to instantiate {type}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Updates reference price.
        /// </summary>
        /// <param name="info">The information.</param>
        public static void UpdateReferencePrice(BeerInfo info)
        {
            foreach (var priceProvider in Providers)
            {
                var price = priceProvider.GetPrice(info);
                if (price != null)
                {
                    info.ReferencePrice = price.Price;
                    info.ReferencePriceUrl = price.Reference;
                    return;
                }
            }
        }
    }
}
