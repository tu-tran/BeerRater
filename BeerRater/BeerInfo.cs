namespace BeerRater
{
    /// <summary>
    /// The beer info.
    /// </summary>
    public struct BeerInfo
    {
        /// <summary>
        /// The name.
        /// </summary>
        public string Name;

        /// <summary>
        /// The ratings.
        /// </summary>
        public string Ratings;

        /// <summary>
        /// The weighted average.
        /// </summary>
        public string WeightedAverage;

        /// <summary>
        /// The calories.
        /// </summary>
        public string Calories;

        /// <summary>
        /// The abv.
        /// </summary>
        public string ABV;

        /// <summary>
        /// The overall rating.
        /// </summary> 
        public string Overall;

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
        /// The price.
        /// </summary>
        public string Price;
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
    }
}