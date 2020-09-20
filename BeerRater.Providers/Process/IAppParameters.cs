namespace BeerRater.Providers.Process
{
    /// <summary>
    ///     The <see cref="IAppParameters" /> interfaces the app parameters.
    /// </summary>
    public interface IAppParameters
    {
        /// <summary>
        ///     Gets or sets a value indicating whether to gets the price comparison.
        /// </summary>
        bool IsPriceCompared { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to gets the beer review.
        /// </summary>
        bool IsRated { get; set; }

        /// <summary>
        ///     Gets or sets the thread counts.
        /// </summary>
        int ThreadsCount { get; set; }

        /// <summary>
        ///     Initializes based on the specified application settings.
        /// </summary>
        void Initialize();

        /// <summary>
        ///     Saves this instance.
        /// </summary>
        void Save();
    }
}