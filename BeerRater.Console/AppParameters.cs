using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using BeerRater.Providers.Process;
using CommandLine;

namespace BeerRater.Console
{
    /// <summary>
    ///     The <see cref="AppParameters" /> class.
    /// </summary>
    public class AppParameters : IAppParameters
    {
        /// <summary>
        ///     The property flags.
        /// </summary>
        private static readonly BindingFlags PropertyFlags = BindingFlags.Instance | BindingFlags.Public;

        /// <summary>
        ///     The configuration file.
        /// </summary>
        private Configuration configFile;

        /// <summary>
        ///     The threads count.
        /// </summary>
        private int threadsCount;

        /// <summary>
        ///     Gets the application settings
        /// </summary>
        private KeyValueConfigurationCollection appSettings => configFile.AppSettings.Settings;

        /// <summary>
        ///     Gets the parameters.
        /// </summary>
        public IEnumerable<PropertyInfo> Parameters
        {
            get
            {
                return GetType().GetProperties(PropertyFlags)
                    .Where(p => p.CanWrite && p.GetCustomAttribute<OptionAttribute>() != null);
            }
        }

        /// <inheritdoc />
        [Option('p', Default = false, HelpText = "Whether to gets the price comparison", Required = false)]
        public bool IsPriceCompared { get; set; }

        /// <inheritdoc />
        [Option('r', Default = true, HelpText = "Whether to gets the beer review", Required = false)]
        public bool IsRated { get; set; }

        /// <inheritdoc />
        [Option('t', Default = 0, HelpText = "Threads count for multi-threading processing", Required = false,
            Hidden = true)]
        public int ThreadsCount
        {
            get => threadsCount > 0 ? threadsCount : Environment.ProcessorCount;
            set => threadsCount = value;
        }

        /// <inheritdoc />
        public void Initialize()
        {
            configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            foreach (var propertyInfo in Parameters)
                try
                {
                    var appValue = appSettings[propertyInfo.Name].Value;
                    var dataType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
                    var convertedValue = Convert.ChangeType(appValue, dataType);
                    propertyInfo.SetValue(this, convertedValue);
                }
                catch (Exception e)
                {
                    Trace.TraceError($"Failed to load app config for {propertyInfo.Name}: {e.Message}");
                }
        }

        /// <inheritdoc />
        public void Save()
        {
            foreach (var propertyInfo in Parameters.Where(p =>
                p.GetCustomAttribute<OptionAttribute>() != null && !p.GetCustomAttribute<OptionAttribute>().Hidden))
                try
                {
                    var currentValue = propertyInfo.GetValue(this);
                    appSettings[propertyInfo.Name].Value = currentValue?.ToString();
                }
                catch (Exception e)
                {
                    Trace.TraceError($"Failed to save app config for {propertyInfo.Name}: {e.Message}");
                }

            configFile.Save(ConfigurationSaveMode.Modified);
        }
    }
}