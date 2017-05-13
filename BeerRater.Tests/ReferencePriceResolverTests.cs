namespace BeerRater.Tests
{
    using Console.Providers;

    using Data;

    using NUnit.Framework;

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
