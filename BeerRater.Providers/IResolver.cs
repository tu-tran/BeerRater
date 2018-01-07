namespace BeerRater.Providers
{
    using System.Collections.Generic;

    /// <summary>
    /// The <see cref="IResolver{TType}" /> interfaces the providers resolver.
    /// </summary>
    /// <typeparam name="TType">The instance type.</typeparam>
    public interface IResolver<out TType>
    {
        /// <summary>
        /// Gets the input provider.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The instances of <typeparamref name="TType"/>.</returns>
        IReadOnlyList<TType> Resolve(params string[] args);
    }
}