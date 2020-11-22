using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BeerRater.Data
{
    /// <summary>
    ///     The beer info.
    /// </summary>
    [DebuggerDisplay("{Name} - Total Price: {TotalPrice} - Rating: {Overall}")]
    public class BeerInfo
    {
        /// <summary>
        ///     The abv.
        /// </summary>
        private double? abv;

        /// <summary>
        ///     The calories.
        /// </summary>
        private double? calories;

        /// <summary>
        ///     The overall.
        /// </summary>
        private double? overall;

        /// <summary>
        ///     The price.
        /// </summary>
        private double? price;

        /// <summary>
        ///     The deposit.
        /// </summary>
        private double? deposit;

        /// <summary>
        ///     The reference prices.
        /// </summary>
        private List<ReferencePrice> prices;

        /// <summary>
        ///     The ratings.
        /// </summary>
        private double? ratings;

        /// <summary>
        ///     The weighted average.
        /// </summary>
        private double? weightedAverage;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BeerInfo" /> struct.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="nameOnStore">The name on store.</param>
        /// <param name="productUrl">The product URL.</param>
        /// <param name="imageUrl">The image URL.</param>
        /// <param name="price">The price.</param>
        public BeerInfo(string name, string nameOnStore = null, string productUrl = null, string imageUrl = null,
            double? price = null, string dataSource = null)
        {
            Name = name;
            NameOnStore = string.IsNullOrEmpty(nameOnStore) ? name : nameOnStore;
            ProductUrl = productUrl;
            ImageUrl = imageUrl;
            Price = price;
            DataSource = dataSource;
        }

        /// <summary>
        ///     The abv.
        /// </summary>
        public double? ABV
        {
            get => abv;
            set => SetValue(ref abv, value);
        }

        /// <summary>
        ///     The calories.
        /// </summary>
        public double? Calories
        {
            get => calories;
            set => SetValue(ref calories, value);
        }

        /// <summary>
        ///     The overall rating.
        /// </summary>
        public double? Overall
        {
            get => overall;
            set => SetValue(ref overall, value);
        }

        /// <summary>
        ///     The referencePrice.
        /// </summary>
        public double? Price
        {
            get => price;
            set => SetValue(ref price, value);
        }

        /// <summary>
        ///     The Deposit cost.
        /// </summary>
        public double? Deposit
        {
            get => deposit;
            set => SetValue(ref deposit, value);
        }

        /// <summary>
        ///     The total price, including deposit.
        /// </summary>
        public double? TotalPrice => this.Price.HasValue ? (double?)(this.Price.Value + (this.Deposit ?? 0.0)) : null;

        /// <summary>
        ///     The ratings.
        /// </summary>
        public double? Ratings
        {
            get => ratings;
            set => SetValue(ref ratings, value);
        }

        /// <summary>
        ///     The weighted average.
        /// </summary>
        public double? WeightedAverage
        {
            get => weightedAverage;
            set => SetValue(ref weightedAverage, value);
        }

        /// <summary>
        ///     The image URL.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        ///     The name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     The name from the query.
        /// </summary>
        public string NameOnStore { get; }

        /// <summary>
        ///     The product URL.
        /// </summary>
        public string ProductUrl { get; set; }

        /// <summary>
        ///     The URL.
        /// </summary>
        public string ReviewUrl { get; set; }

        /// <summary>
        ///     The style.
        /// </summary>
        public string Style { get; set; }

        /// <summary>
        ///     The data source.
        /// </summary>
        public string DataSource { get; set; }

        /// <summary>
        ///     The reference prices.
        /// </summary>
        public IReadOnlyList<ReferencePrice> ReferencePrices => prices;

        /// <summary>
        ///     Clones this instance.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        public BeerInfo Clone()
        {
            return (BeerInfo) MemberwiseClone();
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format($"{Name}\t{Overall}\t{Ratings}\t{WeightedAverage}\t{Calories}\t{ABV}\t{TotalPrice}");
        }

        /// <summary>
        ///     Adds referencePrice.
        /// </summary>
        /// <param name="referencePrice">The referencePrice.</param>
        public void AddPrice(ReferencePrice referencePrice)
        {
            if (prices == null) prices = new List<ReferencePrice>(1);

            var matching =
                prices.FirstOrDefault(p =>
                    string.Equals(p.Url, referencePrice.Url, StringComparison.OrdinalIgnoreCase));
            if (matching == null)
                prices.Add(referencePrice);
            else
                matching.Price = referencePrice.Price;
        }

        /// <summary>
        ///     Sets the value.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        private void SetValue(ref double? field, double? value)
        {
            if (value.HasValue)
                field = Math.Round(value.Value, 2, MidpointRounding.AwayFromZero);
            else
                field = null;
        }
    }
}