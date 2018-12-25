using Newtonsoft.Json;

namespace BeerRater.Tests
{
    using Data;
    using NUnit.Framework;
    using System.Linq;

    using BeerRater.Providers;
    using BeerRater.Providers.Pricings;
    using BeerRater.Providers.Process;
    using BeerRater.Providers.Ratings;

    /// <summary>
    /// Tests the <see cref="ReferencePriceResolver"/>.
    /// </summary>
    [TestFixture]
    public class ResolverTests
    {
        /// <summary>
        /// References price resolve test.
        /// </summary>
        [Test]
        public void ReferencePriceResolveTest()
        {
            var target = new BeerInfo("Chimay Blue");
            var priceProviders = new ReflectionResolver<IPriceProvider>().Resolve(null);
            foreach (var pricer in priceProviders)
            {
                pricer.Update(target);
            }

            Assert.IsTrue(target.ReferencePrices.Any());
        }

        /// <summary>
        /// Beers advocate rating resolve test.
        /// </summary>
        [Test]
        public void BeerAdvocateRatingResolveTest()
        {
            var target = new BeerInfo("De Troch Chapeau Pêche");
            new BeerAdvocateProvider().Query(target);
            Assert.NotNull(target);
            Assert.IsTrue(target.Overall > 0.0);

            target = new BeerInfo("Gruut Blond");
            new RatingsResolver(new ReflectionResolver<IRatingProvider>().Resolve(null)).Query(target);
            Assert.NotNull(target);
            Assert.IsTrue(target.Overall > 0.0);
        }

        /// <summary>
        /// RateBeer rating resolve test.
        /// </summary>
        [Test]
        public void RateBeerResolveTest()
        {
            var typedef = new { };
            var result = RateBeerProvider.GetBeerResult("Young's Double Chocolate Stout");
            var data = JsonConvert.DeserializeObject<dynamic>(result);
            var target = new BeerInfo("Chimay Première (Red)");
            new RateBeerProvider().Query(target);
            Assert.NotNull(target);
            Assert.IsTrue(target.Overall > 0.0);

            target = new BeerInfo("Gruut Blond");
            new RatingsResolver(new ReflectionResolver<IRatingProvider>().Resolve(null)).Query(target);
            Assert.NotNull(target);
            Assert.IsTrue(target.Overall > 0.0);
        }
    }
}