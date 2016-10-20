namespace BeerRater.Tests
{
    using BeerRater.Data;
    using BeerRater.Providers;

    using NUnit.Framework;
    using NUnit.Framework.Internal;

    /// <summary>
    /// Tests the <see cref="ReferencePriceResolver"/>.
    /// </summary>
    [TestFixture]
    public class ReferencePriceResolverTests
    {
        [Test]
        public void ResolveTest()
        {
            var target = new BeerInfo { NameOnStore = "Chimay Blue 9% 33cl" };
            ReferencePriceResolver.UpdateReferencePrice(target);
            Assert.IsTrue(target.ReferencePrice > 0.0);
        }
    }
}
