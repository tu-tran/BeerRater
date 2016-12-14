namespace BeerRater.Console
{
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reflection;

    using CommandLine;

    /// <summary>
    /// The <see cref="AppParameters"/> class.
    /// </summary>
    public class AppParameters
    {
        /// <summary>
        /// Gets or sets a value indicating whether to gets the price comparison.
        /// </summary>
        [Option('p', Default = null, HelpText = "Whether to gets the price comparison", Required = false)]
        public bool? IsPriceCompared { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to gets the beer review.
        /// </summary>
        [Option('r', Default = null, HelpText = "Whether to gets the beer review", Required = false)]
        public bool? IsRated { get; set; }

        /// <summary>
        /// Initializes based on the specified application settings.
        /// </summary>
        /// <param name="appSettings">The application settings.</param>
        public void Initialize(NameValueCollection appSettings)
        {
            var properties = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.CanWrite && p.GetCustomAttribute<OptionAttribute>() != null);
            foreach (var propertyInfo in properties)
            {
                var appValue = appSettings.Get(propertyInfo.Name);
                if (propertyInfo.PropertyType == typeof(bool?))
                {
                    bool boolValue;
                    if (bool.TryParse(appValue, out boolValue))
                    {
                        propertyInfo.SetValue(this, boolValue);
                    }
                }
            }
        }
    }
}
