using HtmlAgilityPack;

namespace BeerRater.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using BeerRater.Data;
    using BeerRater.Processors;
    using BeerRater.Utils;

    /// <summary>
    /// The SuperAlkolProvider input resolver.
    /// </summary>
    internal class SuperAlkoInputProvider : Multitask, IInputProvider
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
            "Retrieving beer lists from SuperAlko...".Output();
            var date = DateTime.Now;
            var result = new List<BeerMeta>();
            var url = "http://m.viinarannasta.ee/range-of-products/1";
            var referrer = "http://m.viinarannasta.ee/";
            var countryIndex = url.GetDocument(referrer).DocumentNode;
            var nodes = countryIndex.SelectNodes("//section/div/h4//a");
            this.Queue.Start((n,i) => this.GetBeerForCountry(n, result, url), nodes);
            return new QuerySession($"SuperAlko_{date:yyyyMMdd_hhmmss}", result);
        }

        /// <summary>
        /// Gets the beers for country.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="result">The result.</param>
        /// <param name="baseUrl">The URL.</param>
        private void GetBeerForCountry(HtmlNode node, List<BeerMeta> result, string baseUrl)
        {
            var countryUrl = "http://m.viinarannasta.ee/" + node.GetAttributeValue("href", "");
            lock (result)
            {
                result.AddRange(this.GetBeers(countryUrl, baseUrl));
            }
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
                var name = beerNode.InnerText.TrimDecoded();
                var beerName = name.ExtractBeerName();
                var priceText = dataNodes[2].ChildNodes[0].InnerText.TrimDecoded().Replace(',', '.');
                double parsedPrice;
                double? price = null;
                if (double.TryParse(priceText, out parsedPrice))
                {
                    price = parsedPrice;
                }

                string imageUrl = null;
                var imageNode = node.SelectSingleNode("./div/a/img");
                if (imageNode != null)
                {
                    imageUrl = imageNode.GetAttributeValue("src", String.Empty).Replace("-thumb-", "-");
                    if (!imageUrl.UrlExists(referrer))
                    {
                        imageUrl = null;
                    }
                }

                var uri = new Uri(url);
                var beerUrl = $"{uri.Scheme}://{uri.Host}/{beerNode.GetAttributeValue("href", "")}";
                Trace.WriteLine($"SuperAlko: [{name}] -> [{beerName}]   {price}");
                result.Add(new BeerMeta(beerName, beerUrl, imageUrl, price));
            }

            return result;
        }
    }
}