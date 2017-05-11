namespace BeerRater.Processors
{
    using System;

    using BeerRater.Utils;

    /// <summary>
    /// Multitasking.
    /// </summary>
    internal class Multitask
    {
        /// <summary>
        /// The pool size
        /// </summary>
        private static int poolSize;

        /// <summary>
        /// The default threads count.
        /// </summary>
        internal static int PoolSize
        {
            get
            {
                return poolSize;
            }

            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("value");
                };
                poolSize = value;
            }
        }

        static Multitask()
        {
            PoolSize = Environment.ProcessorCount;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryQueue"/> class.
        /// </summary>
        public Multitask() : this(PoolSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Multitask"/> class.
        /// </summary>
        /// <param name="maxThreads">The maximum threads.</param>
        public Multitask(int maxThreads)
        {
            this.Queue = new QueryQueue(this.GetType().Name.GetValidThreadName(), maxThreads);
        }

        /// <summary>
        /// Gets the queue.
        /// </summary>
        protected QueryQueue Queue { get; private set; }
    }
}