namespace BeerRater.Data
{
    /// <summary>
    /// The <see cref="ReferencePrice"/> class.
    /// </summary>
    public class ReferencePrice
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferencePrice"/> class.
        /// </summary>
        /// <param name="price">The price.</param>
        /// <param name="url">The url.</param>
        public ReferencePrice(double price, string url)
        {
            this.Price = price;
            this.Url = url;
        }

        /// <summary>
        /// Gets the url.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets the price.
        /// </summary>
        public double Price { get; set; }
    }
}
