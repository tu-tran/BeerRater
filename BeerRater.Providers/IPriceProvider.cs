using BeerRater.Data;

namespace BeerRater.Providers
{
    /// <summary>
    /// The <see cref="IPriceProvider"/> interfaces the beer price provider.
    /// </summary>
    public interface IPriceProvider
    {
        /// <summary>
        /// Gets price.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns>The beer price.</returns>
        BeerPrice GetPrice(BeerInfo info);
    }
}