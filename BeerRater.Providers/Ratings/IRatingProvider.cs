using BeerRater.Data;

namespace BeerRater.Providers.Ratings
{
    /// <summary>
    ///     The <see cref="IRatingProvider" /> provides the rating.
    /// </summary>
    public interface IRatingProvider
    {
        /// <summary>
        ///     Queries the specified beer name.
        /// </summary>
        /// <param name="beerName">Name of the beer.</param>
        void Query(BeerInfo beerName);
    }
}