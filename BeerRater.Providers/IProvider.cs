using BeerRater.Data;

namespace BeerRater.Providers
{
    /// <summary>
    ///     The <see cref="IProvider" /> interfaces the beer data provider.
    /// </summary>
    public interface IProvider
    {
        /// <summary>
        ///     Gets the name.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Updates the specified information.
        /// </summary>
        /// <param name="info">The information.</param>
        void Update(BeerInfo info);
    }
}