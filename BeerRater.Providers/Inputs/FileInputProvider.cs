using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BeerRater.Data;
using BeerRater.Utils;

namespace BeerRater.Providers.Inputs
{
    /// <summary>
    ///     The file input resolver.
    /// </summary>
    internal class FileInputProvider : BaseObject, IInputProvider
    {
        /// <summary>
        ///     Gets the name.
        /// </summary>
        public string Name => "File input";

        /// <summary>
        ///     Determines whether the specified arguments is compatible.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>
        ///     <c>true</c> if the specified arguments is compatible; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCompatible(params string[] args)
        {
            return args != null && args.Length == 1 && File.Exists(args[0]);
        }

        /// <summary>
        ///     Gets the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public IReadOnlyList<BeerInfo> GetBeerMeta(params string[] args)
        {
            var fileName = args[0];
            Output($"Processing [{fileName}]...");
            var result = new List<BeerInfo>();
            using (var reader = File.OpenText(fileName))
            {
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    string name = null;
                    double? price = null;
                    var metas = line.Split(new[] {'\t'}, StringSplitOptions.RemoveEmptyEntries);
                    if (metas.Length > 0)
                    {
                        name = metas[0].Trim();
                        if (metas.Length > 1) price = metas[1].ToDouble();
                    }

                    if (!string.IsNullOrEmpty(name)) result.Add(new BeerInfo(name, name, null, null, price, Name));
                }
            }

            return result.OrderBy(m => m.Name).ThenBy(m => m.TotalPrice).ToList();
        }
    }
}