namespace BeerRater
{
    using System;
    using System.Text.RegularExpressions;

    internal static class StringExtensions
    {
        /// <summary>
        /// Extracts the name of the beer.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The beer name.</returns>
        public static string ExtractBeerName(this string input)
        {
            var regex = Regex.Match(input, @"(?<Name>.+?) ?(?<Abv>\d[,\.]?\d? ?%) ?(?<Volume>(\d+x)?\d[,\.]?\d+? ?cl)?");
            var result = regex.Success ? regex.Groups["Name"].Value : input;
            result = result.Replace("A.Le Coq", "A. Le Coq");
            if (result.EndsWith(" beer", StringComparison.OrdinalIgnoreCase))
            {
                result = result.Substring(0, result.Length - " beer".Length);
            }

            return result;
        }
    }
}
