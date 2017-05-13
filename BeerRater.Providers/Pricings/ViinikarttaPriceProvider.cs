﻿namespace BeerRater.Providers.Pricings
{
    using System.Net;

    using BeerRater.Utils;

    using Data;

    using Newtonsoft.Json;

    using RestSharp;

    using Utils;

    /// <summary>
    /// The <see cref="ViinikarttaPriceProvider"/> class provides beer price.
    /// </summary>
    internal sealed class ViinikarttaPriceProvider : PriceProviderBase
    {
        /// <summary>
        /// The query result.
        /// </summary>
        public class QueryResult
        {
            /// <summary>
            /// The results.
            /// </summary>
            public Result[] results { get; set; }
        }

        /// <summary>
        /// The result.
        /// </summary>
        public class Result
        {
            /// <summary>
            /// The identifier.
            /// </summary>
            public int id { get; set; }
        }

        /// <summary>
        /// Gets price.
        /// </summary>
        /// <param name="beerName">Name of the beer.</param>
        /// <returns>The beer price.</returns>
        public override ReferencePrice GetPrice(string beerName)
        {
            var url = $"http://www.viinikartta.fi/db/search_by_name_fragment.php?term={WebUtility.UrlEncode(beerName.Trim())}&searchtype=";
            var client = new RestClient(url);
            var request = new RestRequest(".", Method.GET) { RequestFormat = DataFormat.Json };

            // easily add HTTP Headers
            var baseUrl = "http://www.viinikartta.fi/";
            request.AddHeader("Referer", baseUrl);
            request.AddHeader("User-Agent", WebExtensions.GetUserAgent(false));
            request.AddHeader("X-Requested-With", "XMLHttpRequest");

            // execute the request
            var response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var data = JsonConvert.DeserializeObject<QueryResult>(response.Content);
                if (data != null && data.results != null && data.results.Length > 0)
                {
                    var priceUrl = $"{baseUrl}viini/{data.results[0].id}";
                    var infoDoc = priceUrl.GetDocument(baseUrl);
                    var priceNode = infoDoc.DocumentNode.SelectSingleNode("//span[@itemprop='price']");
                    if (priceNode != null)
                    {
                        return new ReferencePrice(priceNode.InnerText.TrimDecoded().ToDouble(), priceUrl);
                    }
                }
            }

            return null;
        }
    }
}
