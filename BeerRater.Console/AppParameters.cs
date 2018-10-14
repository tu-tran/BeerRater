namespace BeerRater.Console
{
    using System;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    using BeerRater.Providers.Process;

    using CommandLine;

    /// <summary>
    /// The <see cref="AppParameters"/> class.
    /// </summary>
    public class AppParameters : IAppParameters
    {
        /// <summary>
        /// Gets or sets a value indicating whether to gets the price comparison.
        /// </summary>
        [Option('p', Default = false, HelpText = "Whether to gets the price comparison", Required = false)]
        public bool? IsPriceCompared { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to gets the beer review.
        /// </summary>
        [Option('r', Default = true, HelpText = "Whether to gets the beer review", Required = false)]
        public bool? IsRated { get; set; }

        /// <summary>
        /// Gets or sets the thread counts.
        /// </summary>
        [Option('t', Default = null, HelpText = "Threads count for multi-threading processing", Required = false)]
        public int? ThreadsCount { get; set; }

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
                var dataType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
                try
                {
                    var convertedValue = Convert.ChangeType(appValue, dataType);
                    propertyInfo.SetValue(this, convertedValue);
                }
                catch (Exception e)
                {
                    Trace.TraceError($"Failed to load app config for {propertyInfo.Name} [{appValue}] of type {dataType}: {e.Message}");
                }
            }
        }
    }
}
