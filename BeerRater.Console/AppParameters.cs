using System.Collections.Generic;
using System.Configuration;

namespace BeerRater.Console
{
    using System;
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
        /// The property flags.
        /// </summary>
        private static readonly BindingFlags PropertyFlags = BindingFlags.Instance | BindingFlags.Public;

        /// <summary>
        /// The threads count.
        /// </summary>
        private int threadsCount = 0;

        /// <summary>
        /// Gets the application settings
        /// </summary>
        private KeyValueConfigurationCollection appSettings
        {
            get { return this.configFile.AppSettings.Settings; }
        }

        /// <summary>
        /// The configuration file.
        /// </summary>
        private Configuration configFile;

        /// <inheritdoc />
        [Option('p', Default = false, HelpText = "Whether to gets the price comparison", Required = false)]
        public bool IsPriceCompared { get; set; }

        /// <inheritdoc />
        [Option('r', Default = true, HelpText = "Whether to gets the beer review", Required = false)]
        public bool IsRated { get; set; }

        /// <inheritdoc />
        [Option('t', Default = 0, HelpText = "Threads count for multi-threading processing", Required = false, Hidden = true)]
        public int ThreadsCount
        {
            get => this.threadsCount > 0 ? this.threadsCount : Environment.ProcessorCount;
            set => this.threadsCount = value;
        }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        public IEnumerable<PropertyInfo> Parameters
        {
            get
            {
                return this.GetType().GetProperties(PropertyFlags).Where(p => p.CanWrite && p.GetCustomAttribute<OptionAttribute>() != null);
            }
        }

        /// <inheritdoc />
        public void Initialize()
        {
            this.configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            foreach (var propertyInfo in this.Parameters)
            {
                try
                {
                    var appValue = this.appSettings[propertyInfo.Name].Value;
                    var dataType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
                    var convertedValue = Convert.ChangeType(appValue, dataType);
                    propertyInfo.SetValue(this, convertedValue);
                }
                catch (Exception e)
                {
                    Trace.TraceError($"Failed to load app config for {propertyInfo.Name}: {e.Message}");
                }
            }
        }

        /// <inheritdoc />
        public void Save()
        {
            foreach (var propertyInfo in this.Parameters.Where(p => p.GetCustomAttribute<OptionAttribute>() != null && !p.GetCustomAttribute<OptionAttribute>().Hidden))
            {
                try
                {
                    var currentValue = propertyInfo.GetValue(this);
                    this.appSettings[propertyInfo.Name].Value = currentValue?.ToString();
                }
                catch (Exception e)
                {
                    Trace.TraceError($"Failed to save app config for {propertyInfo.Name}: {e.Message}");
                }
            }

            this.configFile.Save(ConfigurationSaveMode.Modified);
        }
    }
}
