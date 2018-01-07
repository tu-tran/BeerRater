namespace BeerRater.Providers.Process
{
    using System.Collections.Specialized;

    /// <summary>
    /// The <see cref="IAppParameters"/> interfaces the app parameters.
    /// </summary>
    public interface IAppParameters
    {
        /// <summary>
        /// Gets or sets a value indicating whether to gets the price comparison.
        /// </summary>
        bool? IsPriceCompared { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to gets the beer review.
        /// </summary>
        bool? IsRated { get; set; }

        /// <summary>
        /// Gets or sets the thread counts.
        /// </summary>
        int? ThreadsCount { get; set; }

        /// <summary>
        /// Initializes based on the specified application settings.
        /// </summary>
        /// <param name="appSettings">The application settings.</param>
        void Initialize(NameValueCollection appSettings);
    }
}
