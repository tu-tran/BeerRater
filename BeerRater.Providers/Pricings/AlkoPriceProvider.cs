namespace BeerRater.Providers.Pricings
{
    using BeerRater.Utils;
    using Data;
    using Newtonsoft.Json;
    using RestSharp;
    using System.Net;
    using Utils;

    /// <summary>
    /// The <see cref="AlkoPriceProvider"/> class provides beer price.
    /// </summary>
    internal sealed class AlkoPriceProvider : PriceProviderBase
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name { get { return "Alko Price Provider"; } }

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
        protected override ReferencePrice GetPrice(string name)
        {
            var url = $"http://www.alko.fi/api/find/summary?language=en&products=6&query={WebUtility.UrlEncode(name)}&stores=3";
            var referrerUrl = "http://www.alko.fi/en/";
            var response = url.GetRestResponse(referrerUrl, Method.GET, DataFormat.Json, false);
            if (string.IsNullOrEmpty(response))
            {
                this.ApiChanged = true;
                return null;
            }

            var data = JsonConvert.DeserializeObject<QueryResult>(response);
            if (data != null && data.Products != null && data.Products.Results != null && data.Products.Results.Length > 0 && !string.IsNullOrEmpty(data.Products.Results[0].Url))
            {
                var baseUrl = "http://www.alko.fi";
                var priceUrl = $"{baseUrl}{data.Products.Results[0].Url}";
                var infoDoc = priceUrl.GetDocument(referrerUrl);
                var priceNode = infoDoc.DocumentNode.SelectSingleNode("//span[@itemprop='price']");
                if (priceNode != null)
                {
                    return new ReferencePrice(priceNode.InnerText.TrimDecoded().ToDouble(), priceUrl);
                }
            }

            return null;
        }
    }
}
