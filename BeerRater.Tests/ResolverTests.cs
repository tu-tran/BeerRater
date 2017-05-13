namespace BeerRater.Tests
{
    using Console.Providers;

    using Data;

    using NUnit.Framework;

    using Providers.Ratings;

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
            var target = new BeerInfo { NameOnStore = "Chimay Blue 9% 33cl" };
            ReferencePriceResolver.UpdateReferencePrice(target);
            Assert.IsTrue(target.ReferencePrice > 0.0);
        }

        /// <summary>
        /// Beers advocate rating resolve test.
        /// </summary>
        [Test]
        public void BeerAdvocateRatingResolveTest()
        {
            var target = new BeerAdvocateProvider().Query("De Troch Chapeau Pêche");
            Assert.NotNull(target);
            Assert.IsTrue(target.Overall > 0.0);

            target = RatingsResolver.Instance.Query("Gruut Blond");
            Assert.NotNull(target);
            Assert.IsTrue(target.Overall > 0.0);
        }
    }
}
