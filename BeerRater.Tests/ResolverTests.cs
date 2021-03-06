﻿using System.Linq;
using BeerRater.Data;
using BeerRater.Providers;
using BeerRater.Providers.Pricings;
using BeerRater.Providers.Process;
using BeerRater.Providers.Ratings;
using NUnit.Framework;

namespace BeerRater.Tests
{
    /// <summary>
    ///     Tests the <see cref="ReferencePriceResolver" />.
    /// </summary>
    [TestFixture]
    public class ResolverTests
    {
        /// <summary>
        ///     Beers advocate rating resolve test.
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
        ///     RateBeer rating resolve test.
        /// </summary>
        [Test]
        public void RateBeerResolveTest()
        {
            var target = new BeerInfo("Young's Double Chocolate Stout");
            new RateBeerProvider().Query(target);
            Assert.NotNull(target);
            Assert.IsTrue(target.Overall > 0.0);
            Assert.IsTrue(target.WeightedAverage > 0.0);

            target = new BeerInfo("Chimay Première (Red)");
            new RateBeerProvider().Query(target);
            Assert.NotNull(target);
            Assert.IsTrue(target.Overall > 0.0);
            Assert.IsTrue(target.WeightedAverage > 0.0);

            target = new BeerInfo("Gruut Blond");
            new RatingsResolver(new ReflectionResolver<IRatingProvider>().Resolve(null)).Query(target);
            Assert.NotNull(target);
            Assert.IsTrue(target.Overall > 0.0);
            Assert.IsTrue(target.WeightedAverage > 0.0);
        }

        /// <summary>
        ///     References price resolve test.
        /// </summary>
        [Test]
        public void ReferencePriceResolveTest()
        {
            var target = new BeerInfo("Chimay Blue");
            var priceProviders = new ReflectionResolver<IPriceProvider>().Resolve(null);
            foreach (var pricer in priceProviders) pricer.Update(target);

            Assert.IsTrue(target.ReferencePrices.Any());
        }
    }
}