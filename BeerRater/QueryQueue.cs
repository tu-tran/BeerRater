namespace BeerRater
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;

    internal sealed class QueryQueue
    {
        /// <summary>
        /// The maximum threads.
        /// </summary>
        private readonly int maxThreads;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryQueue"/> class.
        /// </summary>
        /// <param name="maxThreads">The maximum threads.</param>
        public QueryQueue(int maxThreads = 20)
        {
            this.maxThreads = maxThreads < 1 ? 1 : maxThreads;
        }

        /// <summary>
        /// Queries the specified metas.
        /// </summary>
        /// <param name="metas">The metas.</param>
        /// <returns>The beer infos.</returns>
        public List<BeerInfo> Query(ICollection<BeerMeta> metas)
        {
            if (metas == null)
            {
                Console.Error.WriteLine("Invalid input arguments. Please specify the file name containing the tab-seperated beer names (and prices)");
                return null;
            }

            var result = new List<BeerInfo>();
            var c = 0;
            var tasks = new List<Task>();
            foreach (var meta in metas)
            {
                c++;
                var task = Task.Factory.StartNew(
                    () =>
                    {
                        var price = meta.Price.HasValue ? meta.Price.ToString() : "-";
                        BeerInfo info;
                        try
                        {
                            info = Crawler.Query(meta.Name);
                            Console.WriteLine($"{result.Count}. {info}");
                        }
                        catch (Exception ex)
                        {
                            info = new BeerInfo
                            {
                                Name = meta.Name,
                                Calories = "-",
                                WeightedAverage = "-",
                                ABV = "-",
                                Ratings = "-",
                                Overall = "-",
                                Price = price
                            };

                            var error = $"{result.Count}. Failed to resolve {meta.Name}";
                            Console.Error.WriteLine(error);
                            Debug.WriteLine(error);
                        }

                        info.Price = price;
                        info.ProductUrl = meta.ProductUrl;
                        result.Add(info);
                    });

                tasks.Add(task);
                if ((tasks.Count == this.maxThreads) || (c == metas.Count))
                {
                    Task.WaitAll(tasks.ToArray());
                    tasks.Clear();
                }
            }

            Task.WaitAll(tasks.ToArray());
            return result;
        }
    }
}
