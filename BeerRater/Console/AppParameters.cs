namespace BeerRater.Console
{
    using CommandLine;

    /// <summary>
    /// The <see cref="AppParameters"/> class.
    /// </summary>
    public class AppParameters
    {
        /// <summary>
        /// Gets or sets a value indicating whether to skip the price comparison.
        /// </summary>
        [Option('p', Default = false, HelpText = "Whether to skip the price comparison", Required = false)]
        public bool SkipPriceCompare { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to skip the beer review.
        /// </summary>
        [Option('r', Default = false, HelpText = "Whether to skip the beer review", Required = false)]
        public bool SkipRating { get; set; }
    }
}
