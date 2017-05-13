using System.Diagnostics;

namespace BeerRater.Utils
{
    /// <summary>
    /// The beer meta.
    /// </summary>
    [DebuggerDisplay("{Name} - {Price}")]
    public struct BeerMeta
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BeerMeta" /> struct.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="nameOnStore">The name on store.</param>
        /// <param name="productUrl">The product URL.</param>
        /// <param name="imageUrl">The image URL.</param>
        /// <param name="price">The price.</param>
        public BeerMeta(string name, string nameOnStore = null, string productUrl = null, string imageUrl = null, double? price = null)
        {
            this.Name = name;
            this.NameOnStore = string.IsNullOrEmpty(nameOnStore) ? name : nameOnStore;
            this.ProductUrl = productUrl;
            this.ImageUrl = imageUrl;
            this.Price = price;
        }

        /// <summary>
        /// Gets the image URL.
        /// </summary>
        public string ImageUrl { get; private set; }

        /// <summary>
        /// Gets the price.
        /// </summary>
        public double? Price { get; private set; }

        /// <summary>
        /// Gets the product url.
        /// </summary>
        public string ProductUrl { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the name on store.
        /// </summary>
        public string NameOnStore { get; private set; }

        /// <summary>
        /// To information.
        /// </summary>
        /// <returns>
        /// The beer information.
        /// </returns>
        public BeerInfo ToInfo()
        {
            return new BeerInfo
            {
                ImageUrl = this.ImageUrl,
                Name = this.Name,
                NameOnStore = this.NameOnStore,
                ProductUrl = this.ProductUrl,
                Price = this.Price ?? default(double)
            };
        }
    }
}