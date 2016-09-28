namespace BeerRater
{
    /// <summary>
    /// The beer meta.
    /// </summary>
    public struct BeerMeta
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BeerMeta"/> struct.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="price">The price.</param>
        public BeerMeta(string name, double? price = null)
        {
            this.Name = name;
            this.Price = price;
        }

        /// <summary>
        /// Gets the price.
        /// </summary>
        public double? Price { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }
    }
}