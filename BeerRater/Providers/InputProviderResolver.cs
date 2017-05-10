namespace BeerRater.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// The input provider.
    /// </summary>
    internal static class InputProviderResolver
    {
        /// <summary>
        /// Gets the input provider.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The input provider.</returns>
        public static IList<IInputProvider> Get(params string[] args)
        {
            var result = new List<IInputProvider>();
            var types =
                AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes().Where(t => t.IsClass && !t.IsAbstract && typeof(IInputProvider).IsAssignableFrom(t)));
            foreach (var type in types)
            {
                try
                {
                    var provider = (IInputProvider)Activator.CreateInstance(type);
                    if (provider.IsCompatible(args))
                    {
                        result.Add(provider);
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Failed to create data provider {0}: {1}", type, ex);
                }
            }

            var index = 1;
            Console.WriteLine($@"
/*=====================================================*\
/*   ____                   _____       _              *\
/*  |  _ \                 |  __ \     | |             *\
/*  | |_) | ___  ___ _ __  | |__) |__ _| |_ ___ _ __   *\
/*  |  _ < / _ \/ _ \ '__| |  _  // _` | __/ _ \ '__|  *\
/*  | |_) |  __/  __/ |    | | \ \ (_| | ||  __/ |     *\
/*  |____/ \___|\___|_|    |_|  \_\__,_|\__\___|_|     *\
/*                                                     *\
/*=====================================================*\


Select the input:

  0. All
{string.Join(Environment.NewLine, result.Select(s => $"  {index++}. {s.Name}"))}
");
            var selection = -1;
            while (selection < 0 || selection > result.Count)
            {
                var input = Console.ReadKey(true).KeyChar.ToString();
                if (!int.TryParse(input, out selection))
                {
                    selection = -1;
                }
            }

            Console.WriteLine($"User selection: {selection}");
            return selection == 0 ? result : new List<IInputProvider> { result[selection - 1] };
        }
    }
}
