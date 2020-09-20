using BeerRater.Data;

namespace BeerRater.Providers.Pricings
{
    /// <summary>
    ///     The <see cref="ProviderBase" /> abstracts the providers.
    /// </summary>
    public abstract class ProviderBase : BaseObject, IProvider
    {
        /// <summary>
        ///     The API changed.
        /// </summary>
        private bool apiChanged;

        /// <summary>
        ///     Gets or sets a value indicating whether the API was changed.
        /// </summary>
        /// <value><c>true</c> if [API changed]; otherwise, <c>false</c>.</value>
        protected bool ApiChanged
        {
            get => apiChanged;
            set
            {
                if (apiChanged == value) return;

                apiChanged = value;
                if (!value) OutputError($"The API for {Name} has been changed!");
            }
        }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        ///     Updates the specified information.
        /// </summary>
        /// <param name="info">The information.</param>
        public void Update(BeerInfo info)
        {
            if (ApiChanged) return;

            DoUpdate(info);
        }

        /// <summary>
        ///     Updates the specified information.
        /// </summary>
        /// <param name="info">The information.</param>
        protected abstract void DoUpdate(BeerInfo info);
    }
}