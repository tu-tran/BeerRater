namespace BeerRater.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;

    /// <summary>
    /// The SuperAlkolProvider input resolver.
    /// </summary>
    internal class SuperAlkoProvider : IInputProvider
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
            return args == null || args.Length < 1;
        }

        /// <summary>
        /// Gets the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The beer metas.</returns>
        public QuerySession Get(params string[] args)
        {
            Console.WriteLine("Retrieving beer lists from SuperAlko...");
            var date = DateTime.Now;
            var result = new List<BeerMeta>();
            var url = "http://m.viinarannasta.ee/range-of-products/1";
            var referrer = "http://m.viinarannasta.ee/";
            var countryIndex = url.GetDocument(referrer).DocumentNode;
            var nodes = countryIndex.SelectNodes("//section/div/h4//a");
            var tasks = new Task[nodes.Count];
            for (var i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                var countryUrl = "http://m.viinarannasta.ee/" + node.GetAttributeValue("href", "");
                tasks[i] = Task.Run(() => result.AddRange(this.GetBeers(countryUrl, url)));
            }

            Task.WaitAll(tasks);
            return new QuerySession($"SuperAlko_{date:yyyyMMdd_hhmmss}", result);
        }

        /// <summary>
        /// Gets the beers.
        /// </summary>
        /// <param name="url">A value indicating whether to.</param>
        /// <param name="referrer">The referrer.</param>
        /// <returns>The beer metas.</returns>
        private List<BeerMeta> GetBeers(string url, string referrer)
        {
            var document = url.GetDocument(referrer).DocumentNode;
            var nodes = document.SelectNodes("//section/div[@class='table']/div");
            var result = new List<BeerMeta>();
            foreach (var node in nodes)
            {
                var dataNodes = node.SelectNodes("./div/span");
                if (dataNodes == null || dataNodes.Count < 2)
                {
                    continue;
                }

                var beerNode = dataNodes[0].SelectSingleNode("./a");
                var name = beerNode.InnerText.Decode();
                var beerName = name.ExtractBeerName();
                var priceText = dataNodes[2].ChildNodes[0].InnerText.Decode().Trim().Replace(',', '.');
                double parsedPrice;
                double? price = null;
                if (double.TryParse(priceText, out parsedPrice))
                {
                    price = parsedPrice;
                }

                var uri = new Uri(url);
                var beerUrl = uri.Host + "/" + beerNode.GetAttributeValue("href", "");
                Trace.WriteLine($"SuperAlko: [{name}] -> [{beerName}]   {price}");
                result.Add(new BeerMeta(beerName, beerUrl, price));
            }

            return result;
        }
    }
}