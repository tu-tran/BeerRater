namespace BeerRater.Providers
{
    using BeerRater.Data;
    using BeerRater.Processors;
    using BeerRater.Utils;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// The file input resolver.
    /// </summary>
    internal class FileInputProvider : IInputProvider
    {
        /// <summary>
        /// Determines whether the specified arguments is compatible.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>
        ///   <c>true</c> if the specified arguments is compatible; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCompatible(params string[] args)
        {
            return args != null && args.Length == 1 && File.Exists(args[0]);
        }

        /// <summary>
        /// Gets the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The beer metas.</returns>
        public QuerySession Get(params string[] args)
        {
            var fileName = args[0];
            $"Processing [{fileName}]...".Output();
            var result = new List<BeerMeta>();
            using (var reader = File.OpenText(fileName))
            {
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    string name = null;
                    double? price = null;
                    var metas = line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (metas.Length > 0)
                    {
                        name = metas[0].ExtractBeerName().Trim();
                        if (metas.Length > 1)
                        {
                            double temp;
                            if (double.TryParse(metas[1], out temp))
                                price = temp;
                        }
                    }

                    if (!string.IsNullOrEmpty(name))
                    {
                        result.Add(new BeerMeta(name, null, null, price));
                    }
                }
            }

            return new QuerySession(fileName, result.OrderBy(m => m.Name).ThenBy(m => m.Price));
        }
    }
}