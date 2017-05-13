namespace BeerRater.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The beer info.
    /// </summary>
    public class BeerInfo
    {
        /// <summary>
        /// The name.
        /// </summary>
        public string Name;

        /// <summary>
        /// The name from the query.
        /// </summary>
        public string NameOnStore;

        /// <summary>
        /// The ratings.
        /// </summary>
        public double Ratings;

        /// <summary>
        /// The weighted average.
        /// </summary>
        public double WeightedAverage;

        /// <summary>
        /// The calories.
        /// </summary>
        public double Calories;

        /// <summary>
        /// The abv.
        /// </summary>
        public double ABV;

        /// <summary>
        /// The overall rating.
        /// </summary> 
        public double Overall;

        /// <summary>
        /// The style.
        /// </summary>
        public string Style;

        /// <summary>
        /// The product URL.
        /// </summary>
        public string ProductUrl;

        /// <summary>
        /// The URL.
        /// </summary>
        public string ReviewUrl;

        /// <summary>
        /// The image URL.
        /// </summary>
        public string ImageUrl;

        /// <summary>
        /// The referencePrice.
        /// </summary>
        public double Price;

        /// <summary>
        /// The reference prices.
        /// </summary>
        private readonly List<ReferencePrice> prices = new List<ReferencePrice>();

        /// <summary>
        /// The reference prices.
        /// </summary>
        public IEnumerable<ReferencePrice> ReferencePrices
        {
            get { return this.prices; }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        public BeerInfo Clone()
        {
            return (BeerInfo)base.MemberwiseClone();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format($"{this.Name}\t{this.Overall}\t{this.Ratings}\t{this.WeightedAverage}\t{this.Calories}\t{this.ABV}\t{this.Price}");
        }

        /// <summary>
        /// Adds referencePrice.
        /// </summary>
        /// <param name="referencePrice">The referencePrice.</param>
        public void AddPrice(ReferencePrice referencePrice)
        {
            var matching = this.prices.FirstOrDefault(p => string.Equals(p.Url, referencePrice.Url, StringComparison.OrdinalIgnoreCase));
            if (matching != null)
            {
                matching.Price = referencePrice.Price;
            }
        }
    }
}