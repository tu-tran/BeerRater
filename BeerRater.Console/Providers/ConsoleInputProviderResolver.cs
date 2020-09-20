using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BeerRater.Providers;
using BeerRater.Providers.Inputs;
using BeerRater.Providers.Process;
using BeerRater.Utils;

namespace BeerRater.Console.Providers
{
    /// <summary>
    ///     The input provider.
    /// </summary>
    internal class ConsoleInputProviderResolver : BaseObject, IResolver<IInputProvider>
    {
        /// <inheritdoc />
        public IReadOnlyList<IInputProvider> Resolve(IAppParameters parameters, params string[] args)
        {
            var providers = TypeExtensions.GetLoadedTypes<IInputProvider>().Where(p => p.IsCompatible(args)).ToList();
            var options = GetOptions(parameters).ToArray();
            Print(providers, options);

            var selection = -1;
            while (selection < 0 || selection > providers.Count)
            {
                var input = System.Console.ReadKey(true).KeyChar.ToString();
                if (int.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out selection))
                {
                    var optionSelection = selection - providers.Count;
                    if (optionSelection > 0)
                    {
                        if (optionSelection <= options.Length)
                        {
                            options[optionSelection - 1].Item2();
                            options = GetOptions(parameters).ToArray();
                            Print(providers, options);
                            parameters.Save();
                        }
                        else
                        {
                            Print(providers, options);
                            PrintError($"Invalid selection: {selection}");
                            selection = -1;
                        }
                    }
                }
                else
                {
                    Print(providers, options);
                    PrintError($"Invalid selection: {selection}");
                    selection = -1;
                }
            }

            Output($"User selection: {selection}");
            return selection == 0 ? providers : new List<IInputProvider> {providers[selection - 1]};
        }

        /// <summary>
        ///     Gets the options settings.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>
        ///     The options settings.
        /// </returns>
        private IEnumerable<Tuple<string, Action>> GetOptions(IAppParameters parameters)
        {
            yield return Tuple.Create($"[{(parameters.IsPriceCompared ? "X" : " ")}] Price comparison",
                new Action(
                    () => { parameters.IsPriceCompared = !parameters.IsPriceCompared; }));
            yield return Tuple.Create($"[{(parameters.IsRated ? "X" : " ")}] Beer rating",
                new Action(
                    () => { parameters.IsRated = !parameters.IsRated; }));
        }

        /// <summary>
        ///     Prints the specified parameters.
        /// </summary>
        /// <param name="providers">The providers.</param>
        /// <param name="options">The options.</param>
        private void Print(IEnumerable<IInputProvider> providers, IEnumerable<Tuple<string, Action>> options)
        {
            System.Console.Clear();
            var index = 1;
            var oldColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine(
                @"/*=====================================================*\
/*   ____                   _____       _              *\
/*  |  _ \                 |  __ \     | |             *\
/*  | |_) | ___  ___ _ __  | |__) |__ _| |_ ___ _ __   *\
/*  |  _ < / _ \/ _ \ '__| |  _  // _` | __/ _ \ '__|  *\
/*  | |_) |  __/  __/ |    | | \ \ (_| | ||  __/ |     *\
/*  |____/ \___|\___|_|    |_|  \_\__,_|\__\___|_|     *\
/*                                                     *\
/*=====================================================*\
");

            System.Console.ForegroundColor = ConsoleColor.Cyan;
            System.Console.WriteLine($@"
Select the input:

  0. All
{string.Join(Environment.NewLine, providers.Select(s => $"  {index++}. {s.Name}"))}
---------- OPTIONS ----------
{string.Join(Environment.NewLine, options.Select(s => $"  {index++}. {s.Item1}"))}
");
            System.Console.ForegroundColor = oldColor;
        }

        /// <summary>
        ///     Prints the specified error <see cref="message" />.
        /// </summary>
        /// <param name="message">The message.</param>
        private void PrintError(string message)
        {
            var oldColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine(message);
            System.Console.ForegroundColor = oldColor;
        }
    }
}