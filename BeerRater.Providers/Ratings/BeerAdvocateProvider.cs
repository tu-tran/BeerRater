namespace BeerRater.Providers.Ratings
{
    using BeerRater.Utils;
    using Data;
    using RestSharp.Extensions.MonoHttp;
    using System.Linq;
    using System.Net;
    using System.Text;
    using Utils;

    /// <summary>
    /// The web crawler.
    /// </summary>
    public class BeerAdvocateProvider : IRatingProvider
    {
        /// <summary>
        /// Queries the specified beer release name.
        /// </summary>
        /// <param name="beerName">Name of the beer release.</param>
        /// <returns>The beer info</returns>
        public BeerInfo Query(string beerName)
        {
            var result = new BeerInfo(beerName);
            var encodedTitle = beerName.UrlParamEncode();
            var queryUrl = $"https://www.beeradvocate.com/search/?q={encodedTitle}&qt=beer";
            var referrer = "https://www.beeradvocate.com";
            var searchBeerDoc = queryUrl.GetDocument(referrer);
            var searchRow = searchBeerDoc.DocumentNode.SelectSingleNode(@"//div[@id='ba-content']//ul/li/a");
            if (searchRow == null)
            {
                result.Name = beerName;
                result.ReviewUrl = queryUrl;
                return result;
            }

            result.Name = searchRow.InnerText.TrimDecoded();
            var url = "https://www.beeradvocate.com" + searchRow.GetAttributeValue("href", "");
            result.ReviewUrl = url;

            var resultDoc = url.GetDocument(queryUrl);
            var overallNode = resultDoc.DocumentNode.SelectSingleNode(@"//span[@*='ratingValue']");
            result.Overall = (overallNode == null ? "" : overallNode.InnerText.TrimDecoded()).ToDouble();

            var statsNode = resultDoc.DocumentNode.SelectSingleNode("//div[@id='item_stats']");
            if (statsNode != null)
            {
                var ratingNode = statsNode.SelectSingleNode(".//span[@class='ba-ratings']");
                if (ratingNode != null)
                    result.Ratings = ratingNode.InnerText.TrimDecoded().ToDouble();

                var avgNode = statsNode.SelectSingleNode(".//span[@class='ba-ravg']");
                if (avgNode != null)
                    result.Overall = avgNode.InnerText.TrimDecoded().ToDouble();
            }

            var infoNode = resultDoc.DocumentNode.SelectNodes("//div[@class='break']").FirstOrDefault(n => n.InnerText.Contains("BEER INFO"));
            if (infoNode != null)
            {
                var styleNode = infoNode.SelectSingleNode(".//a[contains(@href, '/beer/style/')]");
                if (styleNode != null)
                {
                    result.Style = styleNode.InnerText.TrimDecoded();
                    var sibling = styleNode.NextSibling;
                    while (sibling != null)
                    {
                        if (sibling.InnerText.Contains("ABV"))
                        {
                            result.ABV = sibling.NextSibling.InnerText.TrimDecoded().Replace("%", "").ToDouble();
                            break;
                        }

                        sibling = sibling.NextSibling;
                    }
                }

                if (string.IsNullOrEmpty(result.ImageUrl))
                {
                    var imageNode = infoNode.SelectSingleNode(".//img");
                    if (imageNode != null)
                    {
                        result.ImageUrl = imageNode.GetAttributeValue("src", "");
                    }
                }
            }

            return result;
        }
    }
}