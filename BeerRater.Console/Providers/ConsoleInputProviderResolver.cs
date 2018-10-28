﻿namespace BeerRater.Console.Providers
{
    using BeerRater.Providers.Inputs;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BeerRater.Providers;
    using BeerRater.Providers.Process;

    using Utils;

    /// <summary>
    /// The input provider.
    /// </summary>
    internal class ConsoleInputProviderResolver : BaseObject, IResolver<IInputProvider>
    {
        /// <summary>
        /// Gets the input provider.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The input provider.</returns>
        public IReadOnlyList<IInputProvider> Resolve(params string[] args)
        {
            var providers = TypeExtensions.GetLoadedTypes<IInputProvider>().Where(p => p.IsCompatible(args)).ToList();
            var index = 1;
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($@"
/*=====================================================*\
/*   ____                   _____       _              *\
/*  |  _ \                 |  __ \     | |             *\
/*  | |_) | ___  ___ _ __  | |__) |__ _| |_ ___ _ __   *\
/*  |  _ < / _ \/ _ \ '__| |  _  // _` | __/ _ \ '__|  *\
/*  | |_) |  __/  __/ |    | | \ \ (_| | ||  __/ |     *\
/*  |____/ \___|\___|_|    |_|  \_\__,_|\__\___|_|     *\
/*                                                     *\
/*=====================================================*\");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($@"

Select the input:

  0. All
{string.Join(Environment.NewLine, providers.Select(s => $"  {index++}. {s.Name}"))}
");

            Console.ForegroundColor = oldColor;
            var selection = -1;
            while (selection < 0 || selection > providers.Count)
            {
                var input = Console.ReadKey(true).KeyChar.ToString();
                if (!int.TryParse(input, out selection))
                {
                    selection = -1;
                }
            }

            this.Output($"User selection: {selection}");
            return selection == 0 ? providers : new List<IInputProvider> {providers[selection - 1]};
        }
    }
}