using System;
using BeerRater.Data;

namespace BeerRater.Providers.Pricings
{
    /// <summary>
    ///     The <see cref="PriceProviderBase" /> class.
    /// </summary>
    public abstract class PriceProviderBase : ProviderBase, IPriceProvider
    {
        /// <summary>
        ///     Updates the specified information.
        /// </summary>
        /// <param name="info">The information.</param>
        protected override void DoUpdate(BeerInfo info)
        {
            var beerName = info.Name;
            var price = string.IsNullOrEmpty(beerName) ? null : GetPrice(beerName);
            if (price == null)
            {
                var storeBeerName = info.NameOnStore;
                if (!string.IsNullOrEmpty(storeBeerName) &&
                    string.Compare(beerName, storeBeerName, StringComparison.OrdinalIgnoreCase) != 0)
                    price = GetPrice(storeBeerName);
            }

            if (price != null) info.AddPrice(price);
        }

        /// <summary>
        ///     Gets price.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The beer price.</returns>
        protected abstract ReferencePrice GetPrice(string name);
    }
}