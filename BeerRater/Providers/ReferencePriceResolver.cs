namespace BeerRater.Providers
{
    using System.Net;

    using BeerRater.Data;
    using BeerRater.Utils;

    using Newtonsoft.Json;

    using RestSharp;

    /// <summary>
    /// The price reference comparer.
    /// </summary>
    internal sealed class ReferencePriceResolver
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
        /// Gets the reference price.
        /// </summary>
        /// <param name="info">The information.</param>
        public static void UpdateReferencePrice(BeerInfo info)
        {
            if (!UpdateBeerInfo(info, info.Name) && info.Name != info.NameOnStore)
            {
                UpdateBeerInfo(info, info.NameOnStore);
            }
        }

        /// <summary>
        /// Gets the reference price.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private static bool UpdateBeerInfo(BeerInfo info, string name)
        {
            var beerName = name.ExtractBeerName().Trim();
            if (string.IsNullOrEmpty(beerName))
            {
                return false;
            }

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
                        info.ReferencePrice = priceNode.InnerText.TrimDecoded().ToDouble();
                        info.ReferencePriceUrl = priceUrl;
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
