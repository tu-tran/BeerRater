using System.Linq;
using BeerRater.Data;
using BeerRater.Providers.Utils;
using BeerRater.Utils;

namespace BeerRater.Providers.Ratings
{
    /// <summary>
    ///     The web crawler.
    /// </summary>
    public class BeerAdvocateProvider : IRatingProvider
    {
        /// <summary>
        ///     Queries the specified beer release name.
        /// </summary>
        /// <param name="beerInfo">The beer information.</param>
        public void Query(BeerInfo beerInfo)
        {
            var encodedTitle = beerInfo.Name.UrlParamEncode();
            var queryUrl = $"https://www.beeradvocate.com/search/?q={encodedTitle}&qt=beer";
            var referrer = "https://www.beeradvocate.com";
            var searchBeerDoc = queryUrl.GetDocument(referrer);
            var searchRow = searchBeerDoc.DocumentNode.SelectSingleNode(@"//div[@id='ba-content']//ul/li/a");
            if (searchRow == null)
            {
                beerInfo.ReviewUrl = queryUrl;
                return;
            }

            beerInfo.Name = searchRow.InnerText.TrimDecoded();
            var url = "https://www.beeradvocate.com" + searchRow.GetAttributeValue("href", "");
            beerInfo.ReviewUrl = url;

            var resultDoc = url.GetDocument(queryUrl);
            var overallNode = resultDoc.DocumentNode.SelectSingleNode(@"//span[@*='ratingValue']");
            beerInfo.Overall = (overallNode == null ? "" : overallNode.InnerText.TrimDecoded()).ToDouble();

            var statsNode = resultDoc.DocumentNode.SelectSingleNode("//div[@id='item_stats']");
            if (statsNode != null)
            {
                var ratingNode = statsNode.SelectSingleNode(".//span[@class='ba-ratings']");
                if (ratingNode != null)
                    beerInfo.Ratings = ratingNode.InnerText.TrimDecoded().ToDouble();

                var avgNode = statsNode.SelectSingleNode(".//span[@class='ba-ravg']");
                if (avgNode != null)
                    beerInfo.Overall = avgNode.InnerText.TrimDecoded().ToDouble();
            }

            var infoNode = resultDoc.DocumentNode.SelectNodes("//div[@class='break']")
                .FirstOrDefault(n => n.InnerText.Contains("BEER INFO"));
            if (infoNode != null)
            {
                var styleNode = infoNode.SelectSingleNode(".//a[contains(@href, '/beer/style/')]");
                if (styleNode != null)
                {
                    beerInfo.Style = styleNode.InnerText.TrimDecoded();
                    var sibling = styleNode.NextSibling;
                    while (sibling != null)
                    {
                        if (sibling.InnerText.Contains("ABV"))
                        {
                            beerInfo.ABV = sibling.NextSibling.InnerText.TrimDecoded().Replace("%", "").ToDouble();
                            break;
                        }

                        sibling = sibling.NextSibling;
                    }
                }

                if (string.IsNullOrEmpty(beerInfo.ImageUrl))
                {
                    var imageNode = infoNode.SelectSingleNode(".//img");
                    if (imageNode != null) beerInfo.ImageUrl = imageNode.GetAttributeValue("src", "");
                }
            }
        }
    }
}