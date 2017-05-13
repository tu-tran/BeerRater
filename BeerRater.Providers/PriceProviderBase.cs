namespace BeerRater.Providers
{
    using System;

    using BeerRater.Data;
    using BeerRater.Utils;

    /// <summary>
    /// The <see cref="PriceProviderBase"/> class.
    /// </summary>
    public abstract class PriceProviderBase : IPriceProvider
    {
        /// <summary>
        /// Gets price.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns>The beer price.</returns>
        public BeerPrice GetPrice(BeerInfo info)
        {
            var beerName = info.Name;
            var price = string.IsNullOrEmpty(beerName) ? null : this.GetPrice(beerName);
            if (price == null)
            {
                var storeBeerName = info.NameOnStore;
                if (!string.IsNullOrEmpty(storeBeerName) && string.Compare(beerName, storeBeerName, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    price = this.GetPrice(storeBeerName);
                }
            }

            return price;
        }

        /// <summary>
        /// Gets price.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The beer price.</returns>
        public abstract BeerPrice GetPrice(string name);
    }
}
