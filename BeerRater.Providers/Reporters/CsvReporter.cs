namespace BeerRater.Providers.Reporters
{
    using BeerRater.Utils;
    using Data;
    using System.Collections.Generic;
    using System.IO;

    public class CsvReporter : IReporter
    {
        /// <summary>
        /// Generates the reports based on the specified infos.
        /// </summary>
        /// <param name="infos">The infos.</param>
        /// <param name="basePath">The base path.</param>
        /// <param name="reportName">Name of the report.</param>
        public void Generate(IList<BeerInfo> infos, string basePath, string reportName)
        {
            var target = Path.Combine(basePath, reportName) + ".csv";
            using (var csvStream = new StreamWriter(target, false))
            {
                csvStream.WriteLine("NAME\tOVERALL\tWEIGHTED AVG\tCALORIES\tABV\tRATINGS\tPRICE\tREFERENCE PRICE\tREFERENCE URL\tSTYLE\tURL\tIMAGE");
                foreach (var res in infos)
                {
                    csvStream.WriteLine(
                        res.NameOnStore + '\t' + res.Overall.ToInvariantString() + '\t' + res.WeightedAverage.ToInvariantString() + '\t' +
                        res.Calories.ToInvariantString() + '\t' + res.ABV.ToInvariantString() + '\t' + res.Ratings.ToInvariantString() + '\t'
                        + res.Price.ToInvariantString() + '\t' + res.ReferencePrice.ToInvariantString() + '\t' + res.ReferencePriceUrl + '\t' + res.Style +
                        '\t' + res.ReviewUrl + '\t' + res.ImageUrl);
                }
            }
        }
    }
}
