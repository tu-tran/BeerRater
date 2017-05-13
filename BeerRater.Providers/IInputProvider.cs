namespace BeerRater.Providers
{
    using System.Collections.Generic;

    using Data;

    /// <summary>
    /// The input resolver.
    /// </summary>
    public interface IInputProvider
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Determines whether the specified arguments is compatible.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>
        ///   <c>true</c> if the specified arguments is compatible; otherwise, <c>false</c>.
        /// </returns>
        bool IsCompatible(params string[] args);

        /// <summary>
        /// Gets the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        IList<BeerMeta> GetBeerMeta(params string[] args);
    }
}