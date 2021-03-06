﻿using System.IO;
using BeerRater.Providers.Process;
using BeerRater.Utils;

namespace BeerRater.Providers.Reporters
{
    public class CsvReporter : IReporter
    {
        /// <inheritdoc />
        public void Generate(QuerySession session)
        {
            var basePath = Path.GetDirectoryName(session.Name);
            var target = Path.Combine(basePath, "CSV", session.Name + ".csv");
            Directory.CreateDirectory(Path.GetDirectoryName(target) ?? string.Empty);
            using (var csvStream = new StreamWriter(target, false))
            {
                csvStream.WriteLine("NAME\tOVERALL\tWEIGHTED AVG\tCALORIES\tABV\tRATINGS\tPRICE\tSTYLE\tURL\tIMAGE");
                foreach (var res in session)
                    csvStream.WriteLine(
                        res.NameOnStore + '\t' + res.Overall.ToInvariantString() + '\t' +
                        res.WeightedAverage.ToInvariantString() +
                        '\t' +
                        res.Calories.ToInvariantString() + '\t' + res.ABV.ToInvariantString() + '\t' +
                        res.Ratings.ToInvariantString() + '\t'
                        + res.TotalPrice.ToInvariantString() + '\t' + res.Style + '\t' + res.ReviewUrl + '\t' +
                        res.ImageUrl);
            }
        }
    }
}