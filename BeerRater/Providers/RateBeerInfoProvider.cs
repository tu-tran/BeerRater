namespace BeerRater.Providers
{
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using BeerRater.Data;
    using BeerRater.Utils;

    /// <summary>
    /// The web crawler.
    /// </summary>
    internal static class RateBeerInfoProvider
    {
        /// <summary>
        /// Queries the specified beer release name.
        /// </summary>
        /// <param name="releaseName">Name of the beer release.</param>
        /// <returns>The beer info</returns>
        public static BeerInfo Query(string releaseName)
        {
            var result = new BeerInfo();
            var beerName = releaseName.ExtractBeerName();
            var encodedTitle = HttpUtility.UrlEncode(beerName, Encoding.Default);
            var queryUrl = $"https://www.ratebeer.com/findbeer.asp?beername={encodedTitle}";
            var referrer = "https://www.ratebeer.com";
            var searchBeerDoc = queryUrl.GetDocument(referrer);
            var searchRow = searchBeerDoc.DocumentNode.SelectSingleNode(@"//table[@class='table table-hover table-striped']//a");
            if (searchRow == null)
            {
                result.Name = beerName;
                return result;
            }

            result.Name = searchRow.InnerText.TrimDecoded();
            var url = "https://www.ratebeer.com/" + searchRow.GetAttributeValue("href", "");
            result.ReviewUrl = url;

            var resultDoc = url.GetDocument(queryUrl);
            var proceedNode = resultDoc.DocumentNode.SelectSingleNode("//a[@class='medium orange awesome']");
            if (proceedNode != null)
            {
                var proceedReferrer = url;
                url = "https://www.ratebeer.com/" + proceedNode.GetAttributeValue("href", "");
                resultDoc = url.GetDocument(proceedReferrer);
            }

            var overallNode = resultDoc.DocumentNode.SelectSingleNode(@"//span[@*='ratingValue']");
            result.Overall = (overallNode == null ? "" : overallNode.InnerText.TrimDecoded()).ToDouble();

            var rateNode = resultDoc.DocumentNode.SelectSingleNode("//div/small/abbr");
            if (rateNode != null)
            {
                // RATINGS: 1069   WEIGHTED AVG: 3.3/5   EST. CALORIES: 135   ABV: 4.5%
                var rates = rateNode.ParentNode.InnerText.TrimDecoded();
                var regex = Regex.Match(rates, @"RATINGS: (?<Ratings>\d*).*?WEIGHTED AVG: (?<Avg>.+?)\/5.*?EST\. CALORIES: (?<Calories>\d+?).*?ABV: (?<Abv>.+?)%");
                result.Ratings = regex.Groups["Ratings"].Value.Trim().ToDouble();
                result.ABV = regex.Groups["Abv"].Value.Trim().ToDouble();
                result.Calories = regex.Groups["Calories"].Value.Trim().ToDouble();
                result.WeightedAverage = regex.Groups["Avg"].Value.Trim().ToDouble();
            }

            if (string.IsNullOrEmpty(result.ImageUrl))
            {
                var imageNode = resultDoc.DocumentNode.SelectSingleNode(@"//img[@id='beerImg']");
                if (imageNode != null)
                {
                    result.ImageUrl = imageNode.GetAttributeValue("src", "");
                }
            }

            var infoNode = resultDoc.DocumentNode.SelectSingleNode("//td[@id='_aggregateRating6']");
            if (infoNode != null)
            {
                var styleNode = infoNode.NextSibling.SelectSingleNode(".//a[contains(@href, '/beerstyles')]");
                if (styleNode != null)
                {
                    result.Style = styleNode.InnerText.TrimDecoded();
                }
                else
                {
                    var infoText = infoNode.NextSibling.InnerText.TrimDecoded();
                    var regex = Regex.Match(infoText, "Style: (?<Style>.+?)(  )|$",
                        RegexOptions.Compiled | RegexOptions.Singleline);
                    result.Style = (regex.Success
                        ? regex.Groups["Style"].Value
                        : Regex.Replace(infoText, "  +", @". ")).Trim();
                }
            }

            return result;
        }
    }
}