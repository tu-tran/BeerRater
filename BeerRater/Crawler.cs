namespace BeerRater
{
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;

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
            var searchBeerDoc = queryUrl.GetDocument(referrer);
            var searchRow = searchBeerDoc.DocumentNode.SelectSingleNode(@"//table[@class=""table table-hover table-striped""]//a");
            var url = "https://www.ratebeer.com/" + searchRow.GetAttributeValue("href", "");

            var resultDoc = url.GetDocument(queryUrl);
            var proceedNode = resultDoc.DocumentNode.SelectSingleNode("//a[@class='medium orange awesome']");
            if (proceedNode != null)
            {
                var proceedReferrer = url;
                url = "https://www.ratebeer.com/" + proceedNode.GetAttributeValue("href", "");
                resultDoc = url.GetDocument(proceedReferrer);
            }

            var overall = resultDoc.DocumentNode.SelectSingleNode(@"//span[@*='ratingValue']").InnerText;
            var rates = WebUtility.HtmlDecode(resultDoc.DocumentNode.SelectSingleNode("//div/small/abbr").ParentNode.InnerText);

            // RATINGS: 1069   WEIGHTED AVG: 3.3/5   EST. CALORIES: 135   ABV: 4.5%
            var regex = Regex.Match(rates, @"RATINGS: (?<Ratings>\d*).*?WEIGHTED AVG: (?<Avg>.+?)\/5.*?EST\. CALORIES: (?<Calories>\d+?).*?ABV: (?<Abv>.+?)%");
            var imageNode = resultDoc.DocumentNode.SelectSingleNode(@"//img[@id='beerImg']");
            var imageUrl = imageNode == null ? "" : imageNode.GetAttributeValue("src", "");

            return new BeerInfo
            {
                OVERALL = overall,
                RATINGS = regex.Groups["Ratings"].Value.Decode(),
                ABV = regex.Groups["Abv"].Value.Decode(),
                CALORIES = regex.Groups["Calories"].Value.Decode(),
                NAME = searchRow.InnerText.Decode(),
                WEIGHTED_AVG = regex.Groups["Avg"].Value.Decode(),
                URL = url,
                IMAGE_URL = imageUrl
            };
        }
    }
}