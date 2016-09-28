namespace BeerRater
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;

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
            var htmlReport = baseFile + ".html";
            using (var html = new StreamWriter(htmlReport, false))
            {
                html.WriteLine(@"<html><table><tr>
<th>IMAGE</th>
<th>NAME</th>
<th>OVERALL</th>
<th>WEIGHTED AVG</th>
<th>CALORIES</th>
<th>ABV</th>
<th>RATINGS</th>
<th>PRICE</th></tr>");
                using (var fs = new StreamWriter(baseFile + ".csv", false))
                {
                    fs.WriteLine("NAME\tOVERALL\tWEIGHTED AVG\tCALORIES\tABV\tRATINGS\tPRICE\tURL\tIMAGE");
                    foreach (var res in infos)
                    {
                        fs.WriteLine(
                            res.NAME + '\t' + res.OVERALL + '\t' + res.WEIGHTED_AVG + '\t' + res.CALORIES + '\t' + res.ABV + '\t' + res.RATINGS + '\t'
                            + res.PRICE + '\t' + res.URL + '\t' + res.IMAGE_URL);
                        var imgHeight = string.IsNullOrEmpty(res.IMAGE_URL) ? 0 : 96;
                        html.WriteLine($@"<tr>
<td><a href='{WebUtility.HtmlEncode(res.URL)}'><img src='{WebUtility.HtmlEncode(res.IMAGE_URL)}' height='{imgHeight}' alt='{WebUtility.HtmlEncode(res.NAME)}'/></a></td>
<td><a href='{WebUtility.HtmlEncode(res.URL)}'>{WebUtility.HtmlEncode(res.NAME)}</a></td>
<td><b>{WebUtility.HtmlEncode(res.OVERALL)}</b></td>
<td>{WebUtility.HtmlEncode(res.WEIGHTED_AVG)}</td>
<td>{WebUtility.HtmlEncode(res.CALORIES)}</td>
<td>{WebUtility.HtmlEncode(res.ABV)}</td>
<td>{WebUtility.HtmlEncode(res.RATINGS)}</td>
<td><b><i>{WebUtility.HtmlEncode(res.PRICE)}</b></i></td></tr>");
                    }
                }

                html.WriteLine(@"</table><html>");
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
