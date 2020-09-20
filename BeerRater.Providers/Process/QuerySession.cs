using System.Collections.Generic;
using BeerRater.Data;

namespace BeerRater.Providers.Process
{
    public sealed class QuerySession : List<BeerInfo>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="QuerySession" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="data">The data.</param>
        public QuerySession(string name, IEnumerable<BeerInfo> data) : base(data)
        {
            Name = name;
        }

        /// <summary>
        ///     Gets the file path.
        /// </summary>
        public string Name { get; set; }
    }
}