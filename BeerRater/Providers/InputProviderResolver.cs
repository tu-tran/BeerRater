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

            return result;
        }
    }
}
