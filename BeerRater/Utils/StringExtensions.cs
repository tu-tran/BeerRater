namespace BeerRater.Utils
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
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
            var regex = Regex.Match(input, @"(?<Name>.+?)( ?\(\w.+\))? \(?(?<Abv>\d+[,\.]?\d? ?%)\)? ?(?<Volume>(\d+x)?\d[,\.]?\d+? ?cl)?");
            var result = regex.Success ? regex.Groups["Name"].Value : input;
            result = result.Replace("A.Le Coq", "A. Le Coq");
            if (result.EndsWith(" beer", StringComparison.OrdinalIgnoreCase))
            {
                result = result.Substring(0, result.Length - " beer".Length);
            }

            return result;
        }

        /// <summary>
        /// To the double.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>The value.</returns>
        public static double ToDouble(this string target)
        {
            double result;
            double.TryParse(target.Replace(',', '.'), out result);
            return result;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public static string ToInvariantString(this double value)
        {
            return value.ToString("0.00", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the valid name for a thread by filtering out invalid characters from <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The target.</param>
        /// <returns>The valid thread name.</returns>
        public static string GetValidThreadName(this string input)
        {
            return new string(input.Where(c => char.IsLetter(c) || char.IsDigit(c)).ToArray());
        }

        /// <summary>
        /// Writes the specified input to output.
        /// </summary>
        /// <param name="input">The input.</param>
        public static void Output(this string input)
        {
            Console.WriteLine(input);
            Trace.WriteLine(input);
        }

        /// <summary>
        /// Writes the specified input to error output.
        /// </summary>
        /// <param name="input">The input.</param>
        public static void OutputError(this string input)
        {
            Console.Error.WriteLine(input);
            Trace.TraceError(input);
        }
    }
}
