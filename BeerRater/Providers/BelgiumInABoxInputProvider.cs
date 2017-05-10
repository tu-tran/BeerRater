﻿namespace BeerRater.Providers
{
    using BeerRater.Data;
    using BeerRater.Processors;
    using BeerRater.Utils;
    using HtmlAgilityPack;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The BelgiumInABoxInputProvider input resolver.
    /// </summary>
    internal class BelgiumInABoxInputProvider : Multitask, IInputProvider
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name => "BelgiumInABox";

        /// <summary>
        /// The query size.
        /// </summary>
        private const int QuerySize = 200;

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
            $"Retrieving beer lists from {this.Name}...".Output();
            var date = DateTime.Now;
            var url = "https://belgiuminabox.com/shop/470-beer";
            var referrer = "";
            var text = url.GetDocument(referrer).DocumentNode.InnerText;
            var totalMatch = Regex.Match(text, @"Showing .+? of (?<Total>\d+) items");
            double totalCount = int.MaxValue;
            if (totalMatch.Success)
            {
                double.TryParse(totalMatch.Groups["Total"].Value.TrimDecoded(), out totalCount);
            }

            var pagination = (int)Math.Ceiling(totalCount / QuerySize);
            var pages = Enumerable.Range(1, pagination);
            var result = new List<BeerMeta>();
            this.Queue.Start<int>((n, i) => this.GetBeerOnPage(result, n), pages.ToList());
            return new QuerySession($"{this.Name}{date:yyyyMMdd_hhmmss}", result);
        }

        /// <summary>
        /// Gets the beer on page.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="pageIndex">Index of the page.</param>
        private void GetBeerOnPage(List<BeerMeta> result, int pageIndex)
        {
            var epochTime = DateTime.UtcNow - new DateTime(1970, 1, 1);
            var url = "https://belgiuminabox.com/shop/modules/blocklayered/blocklayered-ajax.php?id_category_layered=470&layered_weight_slider=0.29_29&layered_price_slider=0_330&orderby=name&orderway=asc" +
                $"&n={QuerySize}true&selected_filters=/page-{pageIndex}&_={epochTime}";
            var referrer = "https://belgiuminabox.com/shop/470-beer";
            var documentText = url.GetRestResponse(referrer);

            JObject documentObject = JObject.Parse(documentText);
            var productList = documentObject.Children().FirstOrDefault(j => j.Path == "productList");
            if (productList == null)
            {
                return;
            }

            var products = productList.FirstOrDefault() as JValue;
            if (products == null)
            {
                return;
            }

            var document = new HtmlDocument();
            document.LoadHtml(products.Value.ToString());
            var pageResult = new List<BeerMeta>();
            var nodes = document.DocumentNode.SelectNodes("//div[@class='product-container']");
            if(nodes == null)
            {
                return;
            }

            foreach (var beerNode in nodes)
            {
                var nameNode = beerNode.SelectSingleNode(".//h5[@itemprop='name']");
                var priceNode = beerNode.SelectSingleNode(".//div[@class='price_container']");
                if (nameNode == null || priceNode == null)
                {
                    continue;
                }

                double parsedPrice;
                double? price = null;
                var priceText = priceNode.InnerText.TrimDecoded();
                priceText = priceText.Replace(',', '.').Substring(0, priceText.IndexOf(' '));
                if (double.TryParse(priceText, out parsedPrice))
                {
                    price = parsedPrice;
                }

                string imageUrl = null;
                var imageNode = beerNode.SelectSingleNode(".//img");
                if (imageNode != null)
                {
                    imageUrl = imageNode.GetAttributeValue("src", String.Empty).TrimDecoded();
                }

                var uri = new Uri(url);
                var urlNode = beerNode.SelectSingleNode(".//a[@class='product_img_link']");
                string beerUrl = null;
                if (urlNode != null)
                {
                    beerUrl = urlNode.GetAttributeValue("href", null).TrimDecoded();
                }

                var nameOnStore = nameNode.InnerText.TrimDecoded();
                var name = FilterBeerName(nameOnStore);
                Trace.WriteLine($"{this.Name}: [{name}] -> {price}");
                pageResult.Add(new BeerMeta(name, nameOnStore, beerUrl, imageUrl, price));
            }

            lock (result)
            {
                result.AddRange(pageResult);
            }
        }

        /// <summary>
        /// Filters the name of the beer.
        /// </summary>
        /// <param name="nameOnStore">The name on store.</param>
        /// <returns>The beer name.</returns>
        private static string FilterBeerName(string nameOnStore)
        {
            var regex = Regex.Match(nameOnStore, @"(?<Name>.+?)(( full crate)|( \d+-Pack)|( \d+(\.|,\d+)? cl))");
            return regex.Success ? regex.Groups["Name"].Value : nameOnStore;
        }
    }
}