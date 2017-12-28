using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerRater.Tests
{
    using Console;

    using Inputs;

    using NUnit.Framework;

    using Providers.Inputs;

    /// <summary>
    /// The <see cref="AppProcessorTests"/> class tests the <see cref="AppProcessor"/>.
    /// </summary>
    [TestFixture]
    public class AppProcessorTests
    {
        /// <summary>
        /// Executes test.
        /// </summary>
        [Test]
        public void ExecuteTest()
        {
            var target = new AppProcessor(
                new List<IInputProvider> { new TestInputProvider() },
                new AppParameters { IsPriceCompared = false, IsRated = true, ThreadsCount = 1 },
                new string[0]);
            target.Execute();
        }
    }
}
