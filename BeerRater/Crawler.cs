namespace BeerRater
{
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;

    using HtmlAgilityPack;

    /// <summary>
    /// The web crawler.
    /// </summary>
    internal static class Crawler
    {
        /// <summary>
        /// Queries the specified beer release name.
        /// </summary>
        /// <param name="releaseName">Name of the beer release.</param>
        /// <returns>The beer info</returns>
        public static BeerInfo Query(string releaseName)
        {
            var encodedTitle = HttpUtility.UrlEncode(releaseName, Encoding.Default);
            var queryUrl = string.Format("https://www.ratebeer.com/findbeer.asp?beername={0}", encodedTitle);
            var referrer = "https://www.ratebeer.com";
            var searhBeerDoc = GetDocument(queryUrl, referrer);
            var searchRow = searhBeerDoc.DocumentNode.SelectSingleNode(@"//table[@class=""table table-hover table-striped""]//a");
            var url = "https://www.ratebeer.com/" + searchRow.GetAttributeValue("href", "");

            var resultDoc = GetDocument(url, queryUrl);
            var proceedNode = resultDoc.DocumentNode.SelectSingleNode("//a[@class='medium orange awesome']");
            if (proceedNode != null)
            {
                var proceedReferrer = url;
                url = "https://www.ratebeer.com/" + proceedNode.GetAttributeValue("href", "");
                resultDoc = GetDocument(url, proceedReferrer);
            }

            var overall = resultDoc.DocumentNode.SelectSingleNode(@"//span[@*='ratingValue']").InnerText;
            var rates = WebUtility.HtmlDecode(resultDoc.DocumentNode.SelectSingleNode("//div/small/abbr").ParentNode.InnerText);

            // RATINGS: 1069   WEIGHTED AVG: 3.3/5   EST. CALORIES: 135   ABV: 4.5%
            var regex = Regex.Match(rates, @"RATINGS: (?<Ratings>\d*).*?WEIGHTED AVG: (?<Avg>.+?)\/5.*?EST\. CALORIES: (?<Calories>\d+?).*?ABV: (?<Abv>.+?)%");
            var imageNode = resultDoc.DocumentNode.SelectSingleNode(@"//img[@id='beerImg']");
            var imageUrl = imageNode == null ? "" : imageNode.GetAttributeValue("src", "");

            return new BeerInfo
            {
                OVERALL = WebUtility.HtmlDecode(overall),
                RATINGS = WebUtility.HtmlDecode(regex.Groups["Ratings"].Value),
                ABV = WebUtility.HtmlDecode(regex.Groups["Abv"].Value),
                CALORIES = WebUtility.HtmlDecode(regex.Groups["Calories"].Value),
                NAME = WebUtility.HtmlDecode(searchRow.InnerText),
                WEIGHTED_AVG = WebUtility.HtmlDecode(regex.Groups["Avg"].Value),
                URL = url,
                IMAGE_URL = imageUrl
            };
        }

        /// <summary>
        /// Gets the web document.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="referrer">The referrer.</param>
        /// <param name="isMobile">A value indicating whether to.</param>
        /// <returns>The web document.</returns>
        private static HtmlDocument GetDocument(string url, string referrer = "", bool isMobile = true)
        {
            var htmlDoc = new HtmlDocument();
            var request = GetRequest(url, referrer, isMobile);
            using (var respStream = request.GetResponse().GetResponseStream())
            {
                if (respStream != null)
                    htmlDoc.Load(respStream, Encoding.Default);
            }

            return htmlDoc;
        }

        /// <summary>
        /// Gets the web request.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="referrer">The referrer.</param>
        /// <param name="isMobile">A value indicating whether to.</param>
        /// <returns>The web request.</returns>
        public static HttpWebRequest GetRequest(string url, string referrer = "", bool isMobile = true)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = isMobile
                                    ? "Mozilla/5.0 (Linux; U; Android 4.2; en-us; SonyC6903 Build/14.1.G.1.518) AppleWebKit/534.30 (KHTML, like Gecko) Version/4.0 Mobile Safari/534.30"
                                    : "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; AS; rv:11.0) like Gecko";
            request.Referer = referrer;
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            return request;
        }
    }
}