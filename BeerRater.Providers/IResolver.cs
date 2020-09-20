using System.Collections.Generic;
using BeerRater.Providers.Process;

namespace BeerRater.Providers
{
    /// <summary>
    ///     The <see cref="IResolver{TType}" /> interfaces the providers resolver.
    /// </summary>
    /// <typeparam name="TType">The instance type.</typeparam>
    public interface IResolver<out TType>
    {
        /// <summary>
        ///     Gets the input provider.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>
        ///     The instances of <typeparamref name="TType" />.
        /// </returns>
        IReadOnlyList<TType> Resolve(IAppParameters parameters, params string[] args);
    }
}