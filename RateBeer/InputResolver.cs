namespace BeerRater
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal static class InputResolver
    {
        /// <summary>
        /// Gets the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The beer metas.</returns>
        public static QuerySession Get(params string[] args)
        {
            if ((args == null) || (args.Length < 1) || !File.Exists(args[0]))
                return null;

            var fileName = args[0];
            Console.WriteLine($"Processing [{fileName}]...");
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
                        name = metas[0].Trim();
                        if (metas.Length > 1)
                        {
                            double temp;
                            if (double.TryParse(metas[1], out temp))
                                price = temp;
                        }
                    }

                    if (!string.IsNullOrEmpty(name))
                        result.Add(new BeerMeta(name, price));
                }
            }

            Console.WriteLine($"Query contains {result.Count} rows");
            return new QuerySession(fileName, result.OrderBy(m => m.Name).ThenBy(m => m.Price));
        }
    }
}