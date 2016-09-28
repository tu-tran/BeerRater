namespace BeerRater.Providers
{
    /// <summary>
    /// The input resolver.
    /// </summary>
    internal interface IInputProvider
    {
        /// <summary>
        /// Determines whether the specified arguments is compatible.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>
        ///   <c>true</c> if the specified arguments is compatible; otherwise, <c>false</c>.
        /// </returns>
        bool IsCompatible(params string[] args);

        /// <summary>
        /// Gets the session.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The session.</returns>
        QuerySession Get(params string[] args);
    }
}