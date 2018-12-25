using BeerRater.Providers.Process;

namespace BeerRater.Providers
{
    using System.Collections.Generic;

    using BeerRater.Utils;

    /// <summary>
    /// The <see cref="ReflectionResolver{TType}" /> resolves type instances through reflection.
    /// </summary>
    /// <typeparam name="TType">The instance type.</typeparam>
    public class ReflectionResolver<TType> : IResolver<TType>
    {
        /// <summary>
        /// The instances.
        /// </summary>
        private static IReadOnlyList<TType> instances;

        /// <summary>
        /// The synchronize object.
        /// </summary>
        private static readonly object SyncObj = new object();

        /// <summary>
        /// Gets the input provider.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// The instances of <typeparamref name="TType" />.
        /// </returns>
        public IReadOnlyList<TType> Resolve(IAppParameters parameters, params string[] args)
        {
            lock (SyncObj)
            {
                if (instances == null)
                {
                    instances = TypeExtensions.GetLoadedTypes<TType>();
                }
            }

            return instances;
        }
    }
}