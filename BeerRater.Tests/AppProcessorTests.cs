namespace BeerRater.Tests
{
    using BeerRater.Providers;
    using BeerRater.Providers.Inputs;
    using BeerRater.Providers.Process;
    using BeerRater.Providers.Reporters;

    using Console;

    using NFluent;

    using NUnit.Framework;

    using Providers;

    /// <summary>
    /// The <see cref="AppProcessorTests"/> class tests the <see cref="AppProcessor"/>.
    /// </summary>
    [TestFixture]
    public class AppProcessorTests
    {
        /// <summary>
        /// The target.
        /// </summary>
        private AppProcessor target;

        /// <summary>
        /// The input provider.
        /// </summary>
        private readonly TestInputProvider inputProvider = new TestInputProvider();

        /// <summary>
        /// The reporter.
        /// </summary>
        private TestReporter reporter;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.reporter = new TestReporter();
            this.target = Program.GetApp(new AppParameters { IsPriceCompared = false, IsRated = true, ThreadsCount = 1 });
            this.target.InputerResolver = new ResolverList<IInputProvider>(this.inputProvider);
        }

        /// <summary>
        /// Executes test.
        /// </summary>
        [Test]
        public void ExecuteWithoutReporterTest()
        {
            this.target.ReporterResolver = new ResolverList<IReporter>(this.reporter);
            this.target.Execute();
            Check.That(this.reporter.LastSession.Count).IsEqualTo(1);
            var actual = this.reporter.LastSession[0];
            Check.That(actual.Name).IsEqualTo("Chimay Première (Red)");
            Check.That(actual.NameOnStore).IsEqualTo("Chimay Red Premiere 7% 75cl");
            Check.That(actual.Overall).HasAValue();
            Check.That(actual.Overall.Value).IsGreaterThan(1.5);
            Check.That(actual.Ratings).HasAValue();
            Check.That(actual.Ratings.Value).IsGreaterThan(1.5);
            Check.That(actual.Style).IsEqualTo("Dubbel");
            Check.That(actual.ImageUrl).IsNotNull();
            Check.That(actual.ReviewUrl).IsNotNull();
        }

        /// <summary>
        /// Executes test.
        /// </summary>
        [Test]
        public void ExecuteTest()
        {
            var reporters = this.target.ReporterResolver.Resolve();
            Check.That(reporters).HasSize(1);
            var reporter = reporters[0] as AggregateReporter<IReporter>;
            Check.That(reporter).IsNotNull();
            Check.That(reporter.Reporters.Count).IsStrictlyGreaterThan(0);
            this.target.Execute();

        }
    }
}