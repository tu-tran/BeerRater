namespace BeerRater.Tests
{
    using BeerRater.Providers.Inputs;

    using NUnit.Framework;

    [TestFixture]
    public class ExtractBeerNameTests
    {
        /// <summary>
        /// Extracts the beer name test.
        /// </summary>
        [Test]
        public void SuperAlkoExtractBeerNameTest()
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

        /// <summary>
        /// Extracts the beer name test.
        /// </summary>
        [Test]
        public void BelgiumInABoxInputExtractBeerNameTest()
        {
            var input = "Bacchus Kriekenbier 37.5 cl";
            var result = BelgiumInABoxInputProvider.ExtractBeerName(input);
            Assert.AreEqual("Bacchus Kriekenbier", result);

            input = "Adriaen Brouwer Wintergold 2014 33 cl";
            result = BelgiumInABoxInputProvider.ExtractBeerName(input);
            Assert.AreEqual("Adriaen Brouwer Wintergold", result);

            input = "Dekoninck Oude Kriek(Frank Boon) 37.5cl";
            result = BelgiumInABoxInputProvider.ExtractBeerName(input);
            Assert.AreEqual("Dekoninck Oude Kriek", result);

            input = "Westvleteren 12 (Abt) 2017 Volume Pack 42 x 33 cl...";
            result = BelgiumInABoxInputProvider.ExtractBeerName(input);
            Assert.AreEqual("Westvleteren 12", result);

            input = "Westvleteren 12 (Abt) 2017 33 cl";
            result = BelgiumInABoxInputProvider.ExtractBeerName(input);
            Assert.AreEqual("Westvleteren 12", result);

            input = "3 Fonteinen Oude Geuze Volume Pack (18 x 75 cl)";
            result = BelgiumInABoxInputProvider.ExtractBeerName(input);
            Assert.AreEqual("3 Fonteinen Oude Geuze", result);

            input = "3 Fonteinen Oude Geuze 37.5 cl 2016 volume pack of...";
            result = BelgiumInABoxInputProvider.ExtractBeerName(input);
            Assert.AreEqual("3 Fonteinen Oude Geuze", result);

            input = "Horal Oude Geuze Megablend Volume Pack 18 x 75 cl";
            result = BelgiumInABoxInputProvider.ExtractBeerName(input);
            Assert.AreEqual("Horal Oude Geuze Megablend", result);

            input = "St-Feuillien Triple 9 Litre";
            result = BelgiumInABoxInputProvider.ExtractBeerName(input);
            Assert.AreEqual("St-Feuillien Triple", result);

            input = "Quintine mixed crate (4x5) 20 x 33 cl";
            result = BelgiumInABoxInputProvider.ExtractBeerName(input);
            Assert.AreEqual("Quintine", result);

            input = "La Corne mixed crate (Blonde-Triple-Black) 24 x 33 cl";
            result = BelgiumInABoxInputProvider.ExtractBeerName(input);
            Assert.AreEqual("La Corne", result);

            input = "Straffe Hendrik Quadrupel 11% full crate 24 x 33 cl";
            result = BelgiumInABoxInputProvider.ExtractBeerName(input);
            Assert.AreEqual("Straffe Hendrik Quadrupel 11%", result);

            input = "Duvel Tripel Hop Citra 2017 full crate 24 x 33 cl";
            result = BelgiumInABoxInputProvider.ExtractBeerName(input);
            Assert.AreEqual("Duvel Tripel Hop Citra", result);

            input = "La Trappe mixed crate (6x4) 24 x 33 cl";
            result = BelgiumInABoxInputProvider.ExtractBeerName(input);
            Assert.AreEqual("La Trappe", result);

            input = "Goliath Triple full crate 24 x 33 cl";
            result = BelgiumInABoxInputProvider.ExtractBeerName(input);
            Assert.AreEqual("Goliath Triple", result);

            input = "Dekoninck Oude Kriek (Frank Boon) 37.5cl";
            result = BelgiumInABoxInputProvider.ExtractBeerName(input);
            Assert.AreEqual("Dekoninck Oude Kriek", result);

            input = "Spencer Trappist IPA 33 cl";
            result = BelgiumInABoxInputProvider.ExtractBeerName(input);
            Assert.AreEqual("Spencer Trappist IPA", result);

            input = "St Louis Premium Kriek (Lambic) 37.5 cl";
            result = BelgiumInABoxInputProvider.ExtractBeerName(input);
            Assert.AreEqual("St Louis Premium Kriek", result);

            input = "Orval Trappist Cheese : Fromage à la Bière (+/- 2 kg)";
            result = BelgiumInABoxInputProvider.ExtractBeerName(input);
            Assert.AreEqual("Orval Trappist Cheese : Fromage à la Bière", result);

            input = "Lupulus Saaz Hops (Finest Harvest Edition) 2016 1.5 L";
            result = BelgiumInABoxInputProvider.ExtractBeerName(input);
            Assert.AreEqual("Lupulus Saaz Hops", result);

            input = "Royal Straight (Het Nest) 5 x 33 cl";
            result = BelgiumInABoxInputProvider.ExtractBeerName(input);
            Assert.AreEqual("Royal Straight", result);
            
            input = "Westvleteren Trappist Gift Box of 2 x Westvleteren";
            result = BelgiumInABoxInputProvider.ExtractBeerName(input);
            Assert.AreEqual("Westvleteren Trappist", result);

            input = "Oud Beersel Bersalis Kadet/Tripel + Bersalis";
            result = BelgiumInABoxInputProvider.ExtractBeerName(input);
            Assert.AreEqual("Oud Beersel Bersalis Kadet/Tripel", result);

            input = "";
            result = BelgiumInABoxInputProvider.ExtractBeerName(input);
            Assert.AreEqual("", result);
        }
    }
}
