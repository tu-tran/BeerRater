namespace BeerRater.Providers.Pricings
{
    using Data;

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
        ReferencePrice GetPrice(BeerInfo info);
    }
}