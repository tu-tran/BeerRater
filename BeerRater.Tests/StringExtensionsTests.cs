namespace BeerRater.Tests
{
    using NUnit.Framework;

    using Providers;

    [TestFixture]
    public class StringExtensionsTests
    {
        /// <summary>
        /// Extracts the beer name test.
        /// </summary>
        [Test]
        public void ExtractBeerNameTest()
        {
            var input = "Saku 1820 Eripruul 4,8% 24x33cl";
            var result = SuperAlkoInputProvider.ExtractBeerName(input);
            Assert.AreEqual("Saku 1820 Eripruul", result);

            input = "Saku 1820 Eripruul 4.8% 24x33cl";
            result = SuperAlkoInputProvider.ExtractBeerName(input);
            Assert.AreEqual("Saku 1820 Eripruul", result);

            input = "Saku 1820 Eripruul 4,8% 33cl";
            result = SuperAlkoInputProvider.ExtractBeerName(input);
            Assert.AreEqual("Saku 1820 Eripruul", result);

            input = "Saku 1820 Eripruul 4.8% 33cl";
            result = SuperAlkoInputProvider.ExtractBeerName(input);
            Assert.AreEqual("Saku 1820 Eripruul", result);

            input = "Saku 1820 Eripruul 4% 33cl";
            result = SuperAlkoInputProvider.ExtractBeerName(input);
            Assert.AreEqual("Saku 1820 Eripruul", result);

            input = "Saku 1820 Eripruul 4% 23x33 cl";
            result = SuperAlkoInputProvider.ExtractBeerName(input);
            Assert.AreEqual("Saku 1820 Eripruul", result);

            input = "Saku 1820 Eripruul 4,5% 33 cl";
            result = SuperAlkoInputProvider.ExtractBeerName(input);
            Assert.AreEqual("Saku 1820 Eripruul", result);

            input = "Saku 1820 Eripruul 4,5% 33,5 cl";
            result = SuperAlkoInputProvider.ExtractBeerName(input);
            Assert.AreEqual("Saku 1820 Eripruul", result);

            input = "Saku 1820 Eripruul 4,5% 33.5 cl";
            result = SuperAlkoInputProvider.ExtractBeerName(input);
            Assert.AreEqual("Saku 1820 Eripruul", result);

            input = "Saku Antvärk Dark Lager 5.7% 24x33cl";
            result = SuperAlkoInputProvider.ExtractBeerName(input);
            Assert.AreEqual("Saku Antvärk Dark Lager", result);

            input = "Liefmans Goudenband 8% 75cl";
            result = SuperAlkoInputProvider.ExtractBeerName(input);
            Assert.AreEqual("Liefmans Goudenband", result);

            input = "Lindemans Kriek 3,5% 37,5cl";
            result = SuperAlkoInputProvider.ExtractBeerName(input);
            Assert.AreEqual("Lindemans Kriek", result);

            input = "Kyoto Alt Beer 5% 33cl";
            result = SuperAlkoInputProvider.ExtractBeerName(input);
            Assert.AreEqual("Kyoto Alt", result);
        }
    }
}
