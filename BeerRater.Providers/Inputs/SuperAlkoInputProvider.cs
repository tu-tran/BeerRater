using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

using BeerRater.Data;
using BeerRater.Providers.Utils;
using BeerRater.Utils;

using HtmlAgilityPack;

namespace BeerRater.Providers.Inputs
{
    /// <summary>
    ///     The SuperAlkolProvider input resolver.
    /// </summary>
    public class SuperAlkoInputProvider : Multitask, IInputProvider
    {
        /// <summary>
        ///     Gets the name.
        /// </summary>
        public string Name => "SuperAlko";

        /// <summary>
        ///     Determines whether the specified arguments is compatible.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>
        ///     <c>true</c> if the specified arguments is compatible; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCompatible(params string[] args)
        {
            return true;
        }

        /// <summary>
        ///     Gets the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public IReadOnlyList<BeerInfo> GetBeerMeta(params string[] args)
        {
            Output("Retrieving beer lists from SuperAlko...");
            var result = new List<BeerInfo>();
            var url = "https://m.viinarannasta.ee/range-of-products/1";
            var referrer = "https://m.viinarannasta.ee/";
            var countryIndex = url.GetDocument(referrer).DocumentNode;
            var nodes = countryIndex.SelectNodes("//section/div/h4//a");
            Queue.Start((n, i) => GetBeerForCountry(n, result, url), new List<HtmlNode>(nodes));
            return result;
        }

        /// <summary>
        ///     Extracts the name of the beer.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The beer name.</returns>
        public static string ExtractBeerName(string input)
        {
            input = input ?? string.Empty;
            var regex = Regex.Match(input,
                @"(?<Name>.+?)( ?\(\w.+\))? \(?(?<Abv>\d+[,\.]?\d? ?%)\)? ?(?<Volume>(\d+x)?\d[,\.]?\d+? ?cl)?");
            var result = regex.Success ? regex.Groups["Name"].Value : input;
            result = result.Replace("A.Le Coq", "A. Le Coq");
            if (result.EndsWith(" beer", StringComparison.OrdinalIgnoreCase))
                result = result.Substring(0, result.Length - " beer".Length);

            return result.Trim();
        }

        /// <summary>
        ///     Gets the beers for country.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="result">The result.</param>
        /// <param name="baseUrl">The URL.</param>
        private void GetBeerForCountry(HtmlNode node, List<BeerInfo> result, string baseUrl)
        {
            var countryUrl = "https://m.viinarannasta.ee/" + node.GetAttributeValue("href", "");
            lock (result)
            {
                result.AddRange(GetBeers(countryUrl, baseUrl));
            }
        }

        /// <summary>
        ///     Gets the beers.
        /// </summary>
        /// <param name="url">A value indicating whether to.</param>
        /// <param name="referrer">The referrer.</param>
        /// <returns>The beer metas.</returns>
        private List<BeerInfo> GetBeers(string url, string referrer)
        {
            var document = url.GetDocument(referrer).DocumentNode;
            var nodes = document.SelectNodes("//section/div[@class='table']/div");
            var result = new List<BeerInfo>();
            foreach (var node in nodes)
            {
                BeerInfo info = null;
                try
                {
                    var dataNodes = node.SelectNodes("./div/span");
                    if (dataNodes == null || dataNodes.Count < 2) continue;

                    var beerNode = dataNodes[0].SelectSingleNode("./a");
                    var nameOnStore = beerNode.InnerText.TrimDecoded();
                    var priceText = dataNodes[2].ChildNodes[0].InnerText.TrimDecoded().Replace(',', '.');
                    var price = priceText.ToDouble();
                    string imageUrl = null;
                    var imageNode = node.SelectSingleNode("./div/a/img");
                    if (imageNode != null)
                    {
                        imageUrl = imageNode.GetAttributeValue("src", string.Empty).Replace("-thumb-", "-");
                        if (!imageUrl.UrlExists(referrer)) imageUrl = null;
                    }

                    var uri = new Uri(url);
                    var beerUrl = $"{uri.Scheme}://{uri.Host}/{beerNode.GetAttributeValue("href", "")}";
                    var beerName = ExtractBeerName(nameOnStore);
                    info = new BeerInfo(beerName, nameOnStore, beerUrl, imageUrl, price, Name);
                    result.Add(info);

                    var bDoc = beerUrl.GetDocument(referrer).DocumentNode;
                    var depositNode = bDoc.SelectNodes("((//div[@class='table']//table)[1]//tr)[last()]/td")?.LastOrDefault();
                    if (depositNode != null)
                    {
                        info.Deposit = depositNode.InnerText.TrimDecoded().Split(' ')[0]?.ToDouble();
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }

                if (info != null)
                {
                    Trace.WriteLine($"{Name}: [{info.NameOnStore}] -> {info.Price}{(info.Deposit.HasValue ? " (Deposit " + info.Deposit + ")" : "")}");
                }
            }

            return result;
        }
    }
}