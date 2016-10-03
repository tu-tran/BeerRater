namespace BeerRater.Processors
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;

    using BeerRater.Data;
    using BeerRater.Properties;
    using BeerRater.Utils;

    using Newtonsoft.Json;

    /// <summary>
    /// Reporter.
    /// </summary>
    internal class Reporter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Reporter" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="basePath">The base path.</param>
        public Reporter(string name, string basePath)
        {
            this.Name = name;
            this.BasePath = basePath;
        }

        /// <summary>
        /// Gets the base path.
        /// </summary>
        public string BasePath { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Generates the reports based on the specified infos.
        /// </summary>
        /// <param name="infos">The infos.</param>
        public void Generate(List<BeerInfo> infos)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<html>");
            var baseFile = Path.Combine(this.BasePath, this.Name);
            var jsFileName = "sorttable.js";
            var jsFile = Path.Combine(this.BasePath, jsFileName);
            File.WriteAllText(jsFile, Resources.JS_SortTable);
            var htmlReport = baseFile + ".html";
            using (var html = new StreamWriter(htmlReport, false))
            {
                var css = @"<style>img{max-height:60} td{vertical-align:middle}</style>";
                var encoding = @"<meta http-equiv='Content-Type' content='text/html;charset=UTF-8'>";
                html.WriteLine($@"<html><head>{encoding}<script type='text/javascript' src='{jsFileName}'></script><link rel='stylesheet' href='https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css' crossorigin='anonymous'>{css}</head><body><table class='table-condensed table-striped table-hover sortable'>
<thead><tr>
<th>IMAGE</th>
<th>NAME</th>
<th>OVERALL</th>
<th>WEIGHTED AVG</th>
<th>CALORIES</th>
<th>ABV</th>
<th>RATINGS</th>
<th>PRICE</th>
<th>STYLE</th></tr></thead><tbody>");
                using (var fs = new StreamWriter(baseFile + ".csv", false))
                {
                    fs.WriteLine("NAME\tOVERALL\tWEIGHTED AVG\tCALORIES\tABV\tRATINGS\tPRICE\tREFERENCE PRICE\tREFERENCE URL\tSTYLE\tURL\tIMAGE");
                    foreach (var res in infos)
                    {
                        var productUrl = string.IsNullOrEmpty(res.ProductUrl) ? res.ReviewUrl : res.ProductUrl;
                        var priceDiff = res.Price - res.ReferencePrice;
                        string priceDiffHtml = string.Empty;

                        if (res.ReferencePrice > 0.001)
                        {
                            if (priceDiff > 0.0)
                            {
                                priceDiffHtml = $"&nbsp;(<font color='red'><a href='{WebUtility.HtmlEncode(res.ReferencePriceUrl)}'>+{priceDiff.ToInvariantString()}</a></font>)";
                            }
                            else if (priceDiff < 0.0)
                            {
                                priceDiffHtml = $"&nbsp;(<font color='green'><a href='{WebUtility.HtmlEncode(res.ReferencePriceUrl)}'>{priceDiff.ToInvariantString()}</a></font>)";
                            }
                        }

                        fs.WriteLine(
                            res.Name + '\t' + res.Overall.ToInvariantString() + '\t' + res.WeightedAverage.ToInvariantString() + '\t' + res.Calories.ToInvariantString() + '\t' + res.ABV.ToInvariantString() + '\t' + res.Ratings.ToInvariantString() + '\t'
                            + res.Price.ToInvariantString() + '\t' + res.ReferencePrice.ToInvariantString() + '\t' + res.ReferencePriceUrl + '\t' + res.Style + '\t' + res.ReviewUrl + '\t' + res.ImageUrl);
                        html.WriteLine($@"<tr>
<td><a href='{WebUtility.HtmlEncode(productUrl)}'><img src='{WebUtility.HtmlEncode(res.ImageUrl)}' /></a></td>
<td><a href='{WebUtility.HtmlEncode(productUrl)}'>{WebUtility.HtmlEncode(res.Name)}</a></td>
<td><a href='{WebUtility.HtmlEncode(res.ReviewUrl)}'><b>{WebUtility.HtmlEncode(res.Overall.ToInvariantString())}</b></a></td>
<td>{WebUtility.HtmlEncode(res.WeightedAverage.ToInvariantString())}</td>
<td>{WebUtility.HtmlEncode(res.Calories.ToInvariantString())}</td>
<td>{WebUtility.HtmlEncode(res.ABV.ToInvariantString())}</td>
<td>{WebUtility.HtmlEncode(res.Ratings.ToInvariantString())}</td>
<td><b><i>{WebUtility.HtmlEncode(res.Price.ToInvariantString())}{priceDiffHtml}</b></i></td>
<td>{WebUtility.HtmlEncode(res.Style)}</td></tr>");
                    }
                }

                html.WriteLine(@"</tbody></table></body></html>");
            }

            File.WriteAllText(baseFile + ".json", JsonConvert.SerializeObject(infos, Formatting.Indented));
            "======================================================================".Output();
            "Open the report? (press Y / Enter to confirm)".Output();
            var key = Console.ReadKey(false);
            if (new[] { ConsoleKey.Enter, ConsoleKey.Y }.Contains(key.Key))
            {
                Process.Start(htmlReport);
            }
        }
    }
}
