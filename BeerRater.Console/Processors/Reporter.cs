﻿namespace BeerRater.Console.Processors
{
    using BeerRater.Providers.Reporters;
    using Data;
    using System.Collections.Generic;
    using Utils;

    /// <summary>
    /// Reporter.
    /// </summary>
    internal class Reporter
    {
        /// <summary>
        /// The instance
        /// </summary>
        public static readonly Reporter Instance = new Reporter();

        /// <summary>
        /// The reporters
        /// </summary>
        private readonly IList<IReporter> reporters;

        /// <summary>
        /// Initializes a new instance of the <see cref="Reporter" /> class.
        /// </summary>
        private Reporter()
        {
            this.reporters = TypeExtensions.GetLoadedTypes<IReporter>();
        }

        /// <summary>
        /// Generates the reports based on the specified infos.
        /// </summary>
        /// <param name="infos">The infos.</param>
        /// <param name="basePath">The base path.</param>
        /// <param name="name">The name.</param>
        public void Generate(List<BeerInfo> infos, string basePath, string name)
        {
            foreach (var reporter in this.reporters)
            {
                reporter.Generate(infos, basePath, name);
            }
        }
    }
}