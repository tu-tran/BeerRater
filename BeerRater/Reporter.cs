namespace BeerRater
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;

    using BeerRater.Properties;

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
                html.WriteLine($@"<html><head><script type='text/javascript' src='{jsFileName}'></script><link rel='stylesheet' href='https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css' crossorigin='anonymous'></head><body><table class='table table-condensed table-striped table-hover sortable'>
<thead><tr>
<th>IMAGE</th>
<th>NAME</th>
<th>OVERALL</th>
<th>WEIGHTED AVG</th>
<th>CALORIES</th>
<th>ABV</th>
<th>RATINGS</th>
<th>PRICE</th></tr></thead><tbody>");
                using (var fs = new StreamWriter(baseFile + ".csv", false))
                {
                    fs.WriteLine("NAME\tOVERALL\tWEIGHTED AVG\tCALORIES\tABV\tRATINGS\tPRICE\tURL\tIMAGE");
                    foreach (var res in infos)
                    {
                        var productUrl = string.IsNullOrEmpty(res.ProductUrl) ? res.ReviewUrl : res.ProductUrl;
                        fs.WriteLine(
                            res.Name + '\t' + res.Overall + '\t' + res.WeightedAverage + '\t' + res.Calories + '\t' + res.ABV + '\t' + res.Ratings + '\t'
                            + res.Price + '\t' + res.ReviewUrl + '\t' + res.ImageUrl);
                        var imgHeight = string.IsNullOrEmpty(res.ImageUrl) ? 0 : 96;
                        html.WriteLine($@"<tr>
<td><a href='{WebUtility.HtmlEncode(productUrl)}'><img src='{WebUtility.HtmlEncode(res.ImageUrl)}' height='{imgHeight}' alt='{WebUtility.HtmlEncode(res.Name)}'/></a></td>
<td><a href='{WebUtility.HtmlEncode(productUrl)}'>{WebUtility.HtmlEncode(res.Name)}</a></td>
<td><a href='{WebUtility.HtmlEncode(res.ReviewUrl)}'><b>{WebUtility.HtmlEncode(res.Overall)}</b></a></td>
<td>{WebUtility.HtmlEncode(res.WeightedAverage)}</td>
<td>{WebUtility.HtmlEncode(res.Calories)}</td>
<td>{WebUtility.HtmlEncode(res.ABV)}</td>
<td>{WebUtility.HtmlEncode(res.Ratings)}</td>
<td><b><i>{WebUtility.HtmlEncode(res.Price)}</b></i></td></tr>");
                    }
                }

                html.WriteLine(@"</tbody></table></body></html>");
            }

            File.WriteAllText(baseFile + ".json", JsonConvert.SerializeObject(infos, Formatting.Indented));
            Console.WriteLine("======================================================================");
            Console.WriteLine("Open the report? (press Y / Enter to confirm)");
            var key = Console.ReadKey(false);
            if (new[] { ConsoleKey.Enter, ConsoleKey.Y }.Contains(key.Key))
            {
                Process.Start(htmlReport);
            }
        }
    }
}
