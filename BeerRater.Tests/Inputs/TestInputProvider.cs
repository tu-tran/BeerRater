namespace BeerRater.Tests.Inputs
{
    using System.Collections.Generic;

    using Data;

    using Providers.Inputs;

    internal class TestInputProvider : IInputProvider
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get { return "UnitTest"; }
        }

        /// <summary>
        /// Determines whether the specified arguments is compatible.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// <c>true</c> if the specified arguments is compatible; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCompatible(params string[] args)
        {
            return true;
        }

        /// <summary>
        /// Gets the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public IList<BeerInfo> GetBeerMeta(params string[] args)
        {
            return new List<BeerInfo> { new BeerInfo("Lindemans Kriek 3,5% 37,5cl", "Lindemans Kriek 3,5% 37,5cl", "http://localhost", null, 5.0, "UnitTest") };
        }
    }
}
