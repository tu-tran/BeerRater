﻿namespace BeerRater.Console.Processors
{
    using System.Collections.Generic;

    using Data;

    internal sealed class QuerySession : List<BeerInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuerySession" /> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="data">The data.</param>
        public QuerySession(string filePath, IEnumerable<BeerInfo> data) : base(data)
        {
            this.FilePath = filePath;
        }

        /// <summary>
        /// Gets the file path.
        /// </summary>
        public string FilePath { get; private set; }
    }
}
