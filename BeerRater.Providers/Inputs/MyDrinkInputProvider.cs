namespace BeerRater.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BeerRater.Data;
    using BeerRater.Processors;
    using BeerRater.Utils;

    /// <summary>
    /// The MyDrinkInputProvider input resolver.
    /// </summary>
    internal class MyDrinkInputProvider : Multitask, IInputProvider
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
            return true;
        }

        /// <summary>
        /// Gets the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The beer metas.</returns>
        public QuerySession Get(params string[] args)
        {
            this.Output("Retrieving beer lists from MyDrink.ee...");
            var date = DateTime.Now;
            var result = new List<BeerMeta>();
            var entries = new[]
                              {
                                  "http://mydrink.ee/olu-siider/olu?limit=all", "http://mydrink.ee/olu-siider/craft-beer?limit=all", "http://mydrink.ee/olu-siider/combimix?limit=all",
                                  "http://mydrink.ee/olu-siider/komplekteeritud-tooted?limit=all"
                              };
            this.Queue.Start((n, i) => this.GetBeerFromPage(n, result), entries);
            return new QuerySession($"MyDrink_{date:yyyyMMdd_hhmmss}", result);
        }

        /// <summary>
        /// Gets beer from page.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="result">The result.</param>
        private void GetBeerFromPage(string url, List<BeerMeta> result)
        {
            var document = url.GetDocument("http://mydrink.ee/").DocumentNode;
            var nodes = document.SelectNodes("//li[@class='item']");
            foreach (var node in nodes)
            {
                var beerLinkNode = node.SelectSingleNode(".//a");
                if (beerLinkNode == null)
                {
                    continue;
                }

                var beerUrl = beerLinkNode.GetAttributeValue("href", "").TrimDecoded();
                var imageNode = beerLinkNode.SelectSingleNode(".//img");
                if (imageNode == null)
                {
                    continue;
                }

                var beerName = imageNode.GetAttributeValue("alt", "").TrimDecoded();
                if (beerName.ToLowerInvariant().Contains("cider"))
                {
                    continue;
                }

                var imageUrl = imageNode.GetAttributeValue("src", "").TrimDecoded();
                var priceNode = node.SelectSingleNode(".//span[@class='price']");
                if (priceNode == null)
                {
                    continue;
                }

                double price;
                var priceStr = new string(priceNode.InnerText.TrimDecoded().Where(c => char.IsDigit(c) || c == '.' || c == ',').ToArray()).Replace(',' ,'.');
                if (!double.TryParse(priceStr, out price))
                {
                    continue;
                }

                result.Add(new BeerMeta(beerName, beerUrl, imageUrl, price));
            }
        }
    }
}