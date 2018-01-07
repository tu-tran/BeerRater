namespace BeerRater.Providers
{
    using System.Collections.Generic;

    /// <summary>
    /// The <see cref="ResolverList{TType}" /> resolves type instances through a list.
    /// </summary>
    /// <typeparam name="TType">The instance type.</typeparam>
    public class ResolverList<TType> : List<TType>, IResolver<TType>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResolverList{TType}"/> class.
        /// </summary>
        /// <param name="instances">The instances.</param>
        public ResolverList(params TType[] instances)
        {
            if (instances != null)
            {
                this.AddRange(instances);
            }
        }

        /// <summary>
        /// Gets the input provider.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The instances of <typeparamref name="TType"/>.</returns>
        public IReadOnlyList<TType> Resolve(params string[] args)
        {
            return this;
        }
    }
}