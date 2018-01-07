namespace BeerRater.Providers.Pricings
{
    using BeerRater.Utils;

    using Data;

    /// <summary>
    /// The <see cref="ProviderBase"/> abstracts the providers.
    /// </summary>
    public abstract class ProviderBase : BaseObject, IProvider
    {
        /// <summary>
        /// The API changed.
        /// </summary>
        private bool apiChanged;

        /// <summary>
        /// Gets or sets a value indicating whether the API was changed.
        /// </summary>
        /// <value><c>true</c> if [API changed]; otherwise, <c>false</c>.</value>
        protected bool ApiChanged
        {
            get { return this.apiChanged; }
            set
            {
                if (this.apiChanged == value)
                {
                    return;
                }

                this.apiChanged = value;
                if (!value)
                {
                    this.OutputError($"The API for {this.Name} has been changed!");
                }
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Updates the specified information.
        /// </summary>
        /// <param name="info">The information.</param>
        public void Update(BeerInfo info)
        {
            if (this.ApiChanged)
            {
                return;
            }

            this.DoUpdate(info);
        }

        /// <summary>
        /// Updates the specified information.
        /// </summary>
        /// <param name="info">The information.</param>
        protected abstract void DoUpdate(BeerInfo info);
    }
}