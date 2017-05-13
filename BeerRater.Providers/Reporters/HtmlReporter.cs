namespace BeerRater.Providers.Reporters
{
    using BeerRater.Utils;

    using Data;

    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;

    using Properties;

    /// <summary>
    /// The <see cref="HtmlReporter"/> generates the HTML report.
    /// </summary>
    public class HtmlReporter : IReporter
    {
        /// <summary>
        /// Generates the reports based on the specified infos.
        /// </summary>
        /// <param name="infos">The infos.</param>
        /// <param name="basePath">The base path.</param>
        /// <param name="reportName">Name of the report.</param>
        public void Generate(IList<BeerInfo> infos, string basePath, string reportName)
        {
            var jsFileName = "sorttable.js";
            var jsFile = Path.Combine(basePath, jsFileName);
            File.WriteAllText(jsFile, Resources.JS_Sorttable);
            var target = Path.Combine(basePath, reportName) + ".html";
            using (var htmlStream = new StreamWriter(target, false, Encoding.Default))
            {
                var css = @"<style>img{max-height:60} td{vertical-align:middle}</style>";
                var encoding = @"<meta http-equiv='Content-Type' content='text/html;charset=UTF-8'>";
                htmlStream.WriteLine(
                    $@"<html><head>{encoding}<script type='text/javascript' src='{jsFileName}'></script>
<link rel='stylesheet' href='https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css' crossorigin='anonymous'>{css}</head>
<body><table class='table-condensed table-striped table-hover sortable'>
<thead><tr>
<th>#</th>
<th></th>
<th>NAME</th>
<th>OVERALL</th>
<th>WEIGHTED AVG</th>
<th>CALORIES</th>
<th>ABV</th>
<th>RATINGS</th>
<th>PRICE</th>
<th>STYLE</th></tr></thead><tbody>");

                var index = 1;
                foreach (var res in infos)
                {
                    var productUrl = string.IsNullOrEmpty(res.ProductUrl) ? res.ReviewUrl : res.ProductUrl;
                    var priceDiff = res.Price - res.ReferencePrice;
                    string priceDiffHtml = string.Empty;

                    if (res.ReferencePrice > 0.001)
                    {
                        if (priceDiff > 0.0)
                        {
                            priceDiffHtml = $"&nbsp;(<a href='{WebUtility.HtmlEncode(res.ReferencePriceUrl)}'><font color='red'>+{priceDiff.ToInvariantString()}</font></a>)";
                        }
                        else if (priceDiff < 0.0)
                        {
                            priceDiffHtml = $"&nbsp;(<a href='{WebUtility.HtmlEncode(res.ReferencePriceUrl)}'><font color='green'>{priceDiff.ToInvariantString()}</font></a>)";
                        }
                    }

                    htmlStream.WriteLine($@"<tr>
<td>{index++}</td>
<td><a href='{WebUtility.HtmlEncode(productUrl)}'><img src='{WebUtility.HtmlEncode(res.ImageUrl)}' /></a></td>
<td><a href='{WebUtility.HtmlEncode(productUrl)}'>{WebUtility.HtmlEncode(res.NameOnStore)}</a></td>
<td><a href='{WebUtility.HtmlEncode(res.ReviewUrl)}'><b>{WebUtility.HtmlEncode(res.Overall.ToInvariantString())}</b></a></td>
<td>{WebUtility.HtmlEncode(res.WeightedAverage.ToInvariantString())}</td>
<td>{WebUtility.HtmlEncode(res.Calories.ToInvariantString())}</td>
<td>{WebUtility.HtmlEncode(res.ABV.ToInvariantString())}</td>
<td>{WebUtility.HtmlEncode(res.Ratings.ToInvariantString())}</td>
<td><b><i>{WebUtility.HtmlEncode(res.Price.ToInvariantString())}{priceDiffHtml}</b></i></td>
<td>{WebUtility.HtmlEncode(res.Style)}</td></tr>");
                }

                htmlStream.WriteLine(@"</tbody></table></body></html>");
            }
        }
    }
}
