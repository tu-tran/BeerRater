namespace BeerRater.Providers.Ratings
{
    using System.Text.RegularExpressions;

    using BeerRater.Utils;

    using Data;

    using Utils;

    /// <summary>
    /// The web crawler.
    /// </summary>
    public class RateBeerProvider : IRatingProvider
    {
        /// <summary>
        /// Queries the specified beer release name.
        /// </summary>
        /// <param name="beerInfo">The beer information.</param>
        public void Query(BeerInfo beerInfo)
        {
            var encodedTitle = beerInfo.Name.UrlParamEncode();
            var queryUrl = $"https://www.ratebeer.com/findbeer.asp?beername={encodedTitle}";
            var referrer = "https://www.ratebeer.com";
            var searchBeerDoc = queryUrl.GetDocument(referrer);
            var searchRow = searchBeerDoc.DocumentNode.SelectSingleNode(@"//table[@class='table table-hover table-striped']//a");
            if (searchRow == null)
            {
                beerInfo.ReviewUrl = queryUrl;
                return;
            }

            beerInfo.Name = searchRow.InnerText.TrimDecoded();
            var url = "https://www.ratebeer.com/" + searchRow.GetAttributeValue("href", "");
            beerInfo.ReviewUrl = url;

            var resultDoc = url.GetDocument(queryUrl);
            var proceedNode = resultDoc.DocumentNode.SelectSingleNode("//a[@class='medium orange awesome']");
            if (proceedNode != null)
            {
                var proceedReferrer = url;
                url = "https://www.ratebeer.com/" + proceedNode.GetAttributeValue("href", "");
                resultDoc = url.GetDocument(proceedReferrer);
            }

            var overallNode = resultDoc.DocumentNode.SelectSingleNode(@"//span[@*='ratingValue']");
            beerInfo.Overall = (overallNode == null ? "" : overallNode.InnerText.TrimDecoded()).ToDouble();

            var rateNode = resultDoc.DocumentNode.SelectSingleNode("//div/small/abbr");
            if (rateNode != null)
            {
                // RATINGS: 1069   WEIGHTED AVG: 3.3/5   EST. CALORIES: 135   ABV: 4.5%
                var rates = rateNode.ParentNode.InnerText.TrimDecoded();
                var regex = Regex.Match(rates, @"RATINGS: (?<Ratings>\d*).*?WEIGHTED AVG: (?<Avg>.+?)\/5.*?EST\. CALORIES: (?<Calories>\d+?).*?ABV: (?<Abv>.+?)%");
                beerInfo.Ratings = regex.Groups["Ratings"].Value.Trim().ToDouble();
                beerInfo.ABV = regex.Groups["Abv"].Value.Trim().ToDouble();
                beerInfo.Calories = regex.Groups["Calories"].Value.Trim().ToDouble();
                beerInfo.WeightedAverage = regex.Groups["Avg"].Value.Trim().ToDouble();
            }

            if (string.IsNullOrEmpty(beerInfo.ImageUrl))
            {
                var imageNode = resultDoc.DocumentNode.SelectSingleNode(@"//img[@id='beerImg']");
                if (imageNode != null)
                {
                    beerInfo.ImageUrl = imageNode.GetAttributeValue("src", "");
                }
            }

            var infoNode = resultDoc.DocumentNode.SelectSingleNode("//*[@id='_aggregateRating6' or contains(@class, 'aggregate-rating-container') or contains(@class, 'description-container')]");
            if (infoNode != null)
            {
                infoNode = infoNode.ParentNode;
            }

            infoNode = infoNode ?? resultDoc.DocumentNode;
            var styleNode = infoNode.SelectSingleNode(".//a[contains(@href, '/beerstyles/') and string-length(@href) > 12]");
            if (styleNode != null)
            {
                beerInfo.Style = styleNode.InnerText.TrimDecoded();
            }
            else
            {
                var infoText = infoNode.InnerText.TrimDecoded();
                var regex = Regex.Match(infoText, "Style: (?<Style>.+?)(  )|$", RegexOptions.Compiled);
                beerInfo.Style = (regex.Success
                    ? regex.Groups["Style"].Value
                    : Regex.Replace(infoText, "  +", @". ")).Trim();
            }
        }
    }
}