namespace BeerRater.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// The <see cref="CustomEqualityComparer&lt;T&gt;"/> class provides an implementation for the <see cref="IEqualityComparer{T}"/> interface,
    /// which compares objects for equality using a custom function.
    /// </summary>
    /// <seealso cref="IEqualityComparer&lt;T&gt;"/>
    /// <seealso cref="EqualityComparer&lt;T&gt;"/>
    /// <typeparam name="T">The type of objects to compare.</typeparam>
    public class CustomEqualityComparer<T> : EqualityComparer<T>
    {
        /// <summary>
        /// The function to use for comparing equality between two objects of type <typeparamref name="T"/>.
        /// </summary>
        private readonly Func<T, T, bool> equalityComparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomEqualityComparer{T}"/> class.
        /// </summary>
        /// <param name="equalityComparer">The equality comparer.</param>
        public CustomEqualityComparer(Func<T, T, bool> equalityComparer)
        {
            this.equalityComparer = equalityComparer;
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <typeparamref name="T"/> to compare.</param>
        /// <param name="y">The second object of type <typeparamref name="T"/> to compare.</param>
        /// <returns>True if the specified objects are equal; otherwise, false.</returns>
        public override bool Equals(T x, T y)
        {
            return this.equalityComparer(x, y);
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj">The object for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        public override int GetHashCode(T obj)
        {
            return obj != null ? obj.GetHashCode() : 0;
        }
    }
}
