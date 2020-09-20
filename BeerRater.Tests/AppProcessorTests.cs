using BeerRater.Console;
using BeerRater.Providers;
using BeerRater.Providers.Inputs;
using BeerRater.Providers.Process;
using BeerRater.Providers.Reporters;
using BeerRater.Tests.Providers;
using NFluent;
using NUnit.Framework;

namespace BeerRater.Tests
{
    /// <summary>
    ///     The <see cref="AppProcessorTests" /> class tests the <see cref="AppProcessor" />.
    /// </summary>
    [TestFixture]
    public class AppProcessorTests
    {
        /// <summary>
        ///     Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            reporter = new TestReporter();
            target = Program.GetApp(new AppParameters {IsPriceCompared = false, IsRated = true, ThreadsCount = 1});
            target.InputResolver = new ResolverList<IInputProvider>(inputProvider);
        }

        /// <summary>
        ///     The target.
        /// </summary>
        private AppProcessor target;

        /// <summary>
        ///     The input provider.
        /// </summary>
        private readonly TestInputProvider inputProvider = new TestInputProvider();

        /// <summary>
        ///     The reporter.
        /// </summary>
        private TestReporter reporter;

        /// <summary>
        ///     Executes test.
        /// </summary>
        [Test]
        public void ExecuteTest()
        {
            var reporters = target.ReporterResolver.Resolve(null);
            Check.That(reporters).HasSize(1);
            var reporter = reporters[0] as AggregateReporter<IReporter>;
            Check.That(reporter).IsNotNull();
            Check.That(reporter.Reporters.Count).IsStrictlyGreaterThan(0);
            target.Execute();
        }

        /// <summary>
        ///     Executes test.
        /// </summary>
        [Test]
        public void ExecuteWithoutReporterTest()
        {
            target.ReporterResolver = new ResolverList<IReporter>(reporter);
            target.Execute();
            Check.That(reporter.LastSession.Count).IsEqualTo(1);
            var actual = reporter.LastSession[0];
            Check.That(actual.Name).IsEqualTo("Chimay Première (Red)");
            Check.That(actual.NameOnStore).IsEqualTo("Chimay Red Premiere 7% 75cl");
            Check.That(actual.Overall).HasAValue();
            Check.That(actual.Overall.Value).IsStrictlyGreaterThan(1.5);
            Check.That(actual.Ratings).HasAValue();
            Check.That(actual.Ratings.Value).IsStrictlyGreaterThan(1.5);
            Check.That(actual.Style).IsEqualTo("Dubbel");
            Check.That(actual.ImageUrl).IsNotNull();
            Check.That(actual.ReviewUrl).IsNotNull();
        }
    }
}