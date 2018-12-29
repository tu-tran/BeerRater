namespace BeerRater.Data
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// The beer info.
    /// </summary>
    [DebuggerDisplay("{Name} - Price: {Price} - Rating: {Overall}")]
    public class BeerInfo
    {
        /// <summary>
        /// The reference prices.
        /// </summary>
        private List<ReferencePrice> prices;

        /// <summary>
        /// The weighted average.
        /// </summary>
        private double? weightedAverage;

        /// <summary>
        /// The ratings.
        /// </summary>
        private double? ratings;

        /// <summary>
        /// The price.
        /// </summary>
        private double? price;

        /// <summary>
        /// The overall.
        /// </summary>
        private double? overall;

        /// <summary>
        /// The calories.
        /// </summary>
        private double? calories;

        /// <summary>
        /// The abv.
        /// </summary>
        private double? abv;

        /// <summary>
        /// The abv.
        /// </summary>
        public double? ABV
        {
            get => this.abv;
            set => SetValue(ref this.abv, value);
        }

        /// <summary>
        /// The calories.
        /// </summary>
        public double? Calories
        {
            get => this.calories;
            set => SetValue(ref this.calories, value);
        }

        /// <summary>
        /// The overall rating.
        /// </summary>
        public double? Overall
        {
            get => this.overall;
            set => SetValue(ref this.overall, value);
        }

        /// <summary>
        /// The referencePrice.
        /// </summary>
        public double? Price
        {
            get => this.price;
            set => SetValue(ref this.price, value);
        }

        /// <summary>
        /// The ratings.
        /// </summary>
        public double? Ratings
        {
            get => this.ratings;
            set => SetValue(ref this.ratings, value);
        }

        /// <summary>
        /// The weighted average.
        /// </summary>
        public double? WeightedAverage
        {
            get => this.weightedAverage;
            set => SetValue(ref this.weightedAverage, value);
        }

        /// <summary>
        /// The image URL.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// The name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The name from the query.
        /// </summary>
        public string NameOnStore { get; }

        /// <summary>
        /// The product URL.
        /// </summary>
        public string ProductUrl { get; set; }

        /// <summary>
        /// The URL.
        /// </summary>
        public string ReviewUrl { get; set; }

        /// <summary>
        /// The style.
        /// </summary>
        public string Style { get; set; }

        /// <summary>
        /// The data source.
        /// </summary>
        public string DataSource { get; set; }

        /// <summary>
        /// The reference prices.
        /// </summary>
        public IReadOnlyList<ReferencePrice> ReferencePrices
        {
            get { return this.prices; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BeerInfo" /> struct.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="nameOnStore">The name on store.</param>
        /// <param name="productUrl">The product URL.</param>
        /// <param name="imageUrl">The image URL.</param>
        /// <param name="price">The price.</param>
        public BeerInfo(string name, string nameOnStore = null, string productUrl = null, string imageUrl = null, double? price = null, string dataSource = null)
        {
            this.Name = name;
            this.NameOnStore = string.IsNullOrEmpty(nameOnStore) ? name : nameOnStore;
            this.ProductUrl = productUrl;
            this.ImageUrl = imageUrl;
            this.Price = price;
            this.DataSource = dataSource;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        public BeerInfo Clone()
        {
            return (BeerInfo)this.MemberwiseClone();
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
            if (this.prices == null)
            {
                this.prices = new List<ReferencePrice>(1);
            }

            var matching = this.prices.FirstOrDefault(p => string.Equals(p.Url, referencePrice.Url, StringComparison.OrdinalIgnoreCase));
            if (matching == null)
            {
                this.prices.Add(referencePrice);
            }
            else
            {
                matching.Price = referencePrice.Price;
            }
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        private void SetValue(ref double? field, double? value)
        {
            if (value.HasValue)
            {
                field = Math.Round(value.Value, 2, MidpointRounding.AwayFromZero);
            }
            else
            {
                field = null;
            }
        }
    }
}