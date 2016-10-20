namespace BeerRater.Providers
{
    using System.Net;

    using BeerRater.Data;
    using BeerRater.Utils;

    using Newtonsoft.Json;

    using RestSharp;

    /// <summary>
    /// The <see cref="AlkoPriceProvider"/> class provides beer price.
    /// </summary>
    internal sealed class AlkoPriceProvider : PriceProviderBase
    {
        /// <summary>
        /// The <see cref="QueryResult"/> class .
        /// </summary>
        public class QueryResult
        {
            /// <summary>
            /// Gets or sets the products.
            /// </summary>
            public CategoryResult Products { get; set; }
        }

        /// <summary>
        /// The <see cref="CategoryResult"/> class .
        /// </summary>
        public class CategoryResult
        {
            /// <summary>
            /// Gets or sets the results.
            /// </summary>
            public Entry[] Results { get; set; }
        }

        /// <summary>
        /// The result.
        /// </summary>
        public class Entry
        {
            /// <summary>
            /// Gets or sets the identifier.
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the URL.
            /// </summary>
            public string Url { get; set; }
        }

        /// <summary>
        /// Gets price.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The beer price.</returns>
        public override BeerPrice GetPrice(string name)
        {
            var url = $"http://www.alko.fi/api/find/summary?language=en&products=6&query={WebUtility.UrlEncode(name)}&stores=3";
            var client = new RestClient(url);
            var request = new RestRequest(".", Method.GET) { RequestFormat = DataFormat.Json };

            // easily add HTTP Headers
            var baseUrl = "http://www.alko.fi";
            var referrerUrl = "http://www.alko.fi/en/";
            request.AddHeader("Referer", referrerUrl);
            request.AddHeader("User-Agent", WebExtensions.GetUserAgent(false));

            // execute the request
            var response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var data = JsonConvert.DeserializeObject<QueryResult>(response.Content);
                if (data != null && data.Products != null && data.Products.Results != null && data.Products.Results.Length > 0 && !string.IsNullOrEmpty(data.Products.Results[0].Url))
                {
                    var priceUrl = $"{baseUrl}{data.Products.Results[0].Url}";
                    var infoDoc = priceUrl.GetDocument(referrerUrl);
                    var priceNode = infoDoc.DocumentNode.SelectSingleNode("//span[@itemprop='price']");
                    if (priceNode != null)
                    {
                        return new BeerPrice(priceNode.InnerText.TrimDecoded().ToDouble(), priceUrl);
                    }
                }
            }

            return null;
        }
    }
}
