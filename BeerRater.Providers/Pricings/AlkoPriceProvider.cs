using System.Collections.Specialized;
using System.Net;
using System.Text;
using BeerRater.Data;
using BeerRater.Providers.Utils;
using BeerRater.Utils;
using HtmlAgilityPack;

namespace BeerRater.Providers.Pricings
{
    /// <summary>
    ///     The <see cref="AlkoPriceProvider" /> class provides beer price.
    /// </summary>
    internal sealed class AlkoPriceProvider : PriceProviderBase
    {
        /// <summary>
        ///     Gets the name.
        /// </summary>
        public override string Name => "Alko Price Provider";

        /// <summary>
        ///     Gets price.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The beer price.</returns>
        protected override ReferencePrice GetPrice(string name)
        {
            var url =
                "https://www.alko.fi/INTERSHOP/web/WFS/Alko-OnlineShop-Site/en_US/-/EUR/ViewSuggestSearch-Suggest?AjaxRequestMarker=true";
            using (var client = new WebClient())
            {
                byte[] responseBytes = null;
                var attempts = 0;
                while (attempts++ < 5 && responseBytes == null)
                    responseBytes = client.UploadValues(url,
                        new NameValueCollection
                        {
                            {"SynchronizerToken", "a818cc584565ec34136bfb8d7c5516bf097f77649eb5c2049d42454a6a0be951"},
                            {"SearchTerm", name}
                        });

                if (responseBytes == null) return null;

                var response = WebUtility.HtmlDecode(Encoding.Default.GetString(responseBytes));
                if (string.IsNullOrEmpty(response))
                {
                    ApiChanged = true;
                    return null;
                }

                var document = new HtmlDocument();
                document.LoadHtml(response);
                var matchNode = document.DocumentNode.SelectSingleNode("//a[@data-item-type='product']");
                if (matchNode != null)
                {
                    var productUrl = matchNode.GetAttributeValue("href", string.Empty);
                    var referrerUrl = "http://www.alko.fi/en/";
                    var productDoc = productUrl.GetDocument(referrerUrl);
                    var priceNode = productDoc.DocumentNode.SelectSingleNode("//span[@itemprop='price']");
                    if (priceNode != null)
                    {
                        var price = priceNode.GetAttributeValue("content", string.Empty);
                        if (!string.IsNullOrEmpty(price)) return new ReferencePrice(price.ToDouble() ?? default(double), productUrl);
                    }
                }
            }

            return null;
        }

        /// <summary>
        ///     The <see cref="QueryResult" /> class .
        /// </summary>
        public class QueryResult
        {
            /// <summary>
            ///     Gets or sets the products.
            /// </summary>
            public CategoryResult Products { get; set; }
        }

        /// <summary>
        ///     The <see cref="CategoryResult" /> class .
        /// </summary>
        public class CategoryResult
        {
            /// <summary>
            ///     Gets or sets the results.
            /// </summary>
            public Entry[] Results { get; set; }
        }

        /// <summary>
        ///     The result.
        /// </summary>
        public class Entry
        {
            /// <summary>
            ///     Gets or sets the identifier.
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            ///     Gets or sets the name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            ///     Gets or sets the URL.
            /// </summary>
            public string Url { get; set; }
        }
    }
}