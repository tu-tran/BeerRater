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
        public string NAME;

        /// <summary>
        /// The ratings.
        /// </summary>
        public string RATINGS;

        /// <summary>
        /// The weighted average.
        /// </summary>
        public string WEIGHTED_AVG;

        /// <summary>
        /// The calories.
        /// </summary>
        public string CALORIES;

        /// <summary>
        /// The abv.
        /// </summary>
        public string ABV;

        /// <summary>
        /// The overall rating.
        /// </summary> 
        public string OVERALL;

        /// <summary>
        /// The URL.
        /// </summary>
        public string URL;

        /// <summary>
        /// The price.
        /// </summary>
        public string PRICE;

        /// <summary>
        /// The image URL.
        /// </summary>
        public string IMAGE_URL;

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format($"{this.NAME}\t{this.OVERALL}\t{this.RATINGS}\t{this.WEIGHTED_AVG}\t{this.CALORIES}\t{this.ABV}");
        }
    }
}