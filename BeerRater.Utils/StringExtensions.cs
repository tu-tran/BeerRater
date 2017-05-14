namespace BeerRater.Utils
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;

    public static class StringExtensions
    {
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
        public static string ToInvariantString(this double? value)
        {
            return value?.ToString("0.00", CultureInfo.InvariantCulture) ?? string.Empty;
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
