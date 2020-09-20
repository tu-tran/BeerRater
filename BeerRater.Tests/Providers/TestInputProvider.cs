using System.Collections.Generic;
using BeerRater.Data;
using BeerRater.Providers.Inputs;

namespace BeerRater.Tests.Providers
{
    internal class TestInputProvider : IInputProvider
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TestInputProvider" /> class.
        /// </summary>
        public TestInputProvider()
        {
            Beers = new List<BeerInfo>
                {new BeerInfo("Chimay Red Premiere", "Chimay Red Premiere 7% 75cl", null, null, 7.5, "UnitTest")};
        }

        /// <summary>
        ///     Gets or sets the beers.
        /// </summary>
        public List<BeerInfo> Beers { get; set; }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        public string Name => "UnitTest";

        /// <summary>
        ///     Determines whether the specified arguments is compatible.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>
        ///     <c>true</c> if the specified arguments is compatible; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCompatible(params string[] args)
        {
            return true;
        }

        /// <summary>
        ///     Gets the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public IReadOnlyList<BeerInfo> GetBeerMeta(params string[] args)
        {
            return Beers;
        }
    }
}