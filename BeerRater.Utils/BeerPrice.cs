namespace BeerRater.Utils
{
    /// <summary>
    /// The <see cref="BeerPrice"/> class.
    /// </summary>
    public class BeerPrice
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BeerPrice"/> class.
        /// </summary>
        /// <param name="price">The price.</param>
        /// <param name="reference">The reference.</param>
        public BeerPrice(double price, string reference)
        {
            this.Price = price;
            this.Reference = reference;
        }

        /// <summary>
        /// Gets the reference.
        /// </summary>
        public string Reference { get; private set; }

        /// <summary>
        /// Gets the price.
        /// </summary>
        public double Price { get; private set; }
    }
}
