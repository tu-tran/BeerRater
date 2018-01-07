namespace BeerRater.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// The <see cref="TypeExtensions"/> class provides type extensions.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Gets the instances of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The instances of type <typeparamref name="T"/>.</returns>
        public static IReadOnlyList<T> GetLoadedTypes<T>()
        {
            var result = new List<T>();
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes().Where(t => t.IsClass && !t.IsAbstract && typeof(T).IsAssignableFrom(t)));
            foreach (var type in types)
            {
                try
                {
                    result.Add((T)Activator.CreateInstance(type));
                }
                catch (Exception ex)
                {
                    Trace.TraceError($"Failed to create instance for type {type}: {ex.Message}");
                }
            }

            return result;
        }
    }
}
