namespace BeerRater
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;

    internal sealed class QueryQueue
    {
        /// <summary>
        /// The maximum threads.
        /// </summary>
        private readonly int maxThreads;

        /// <summary>
        /// The default threads count.
        /// </summary>
        private static readonly int DefaultThreads =
#if SINGLE_THREAD
            1
#else
            Environment.ProcessorCount
#endif
            ;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryQueue"/> class.
        /// </summary>
        public QueryQueue() : this(DefaultThreads)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryQueue"/> class.
        /// </summary>
        /// <param name="maxThreads">The maximum threads.</param>
        public QueryQueue(int maxThreads)
        {
            this.maxThreads = maxThreads < 1 ? 1 : maxThreads;
        }

        /// <summary>
        /// Starts the specified action.
        /// </summary>
        /// <typeparam name="TParam">The type of the parameter.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="parameters">The parameters.</param>
        public void Start<TParam>(Action<TParam> action, IList<TParam> parameters)
        {
            var tasks = new List<Task>(maxThreads);
            for (var i = 0; i < parameters.Count; i++)
            {
                var parameter = parameters[i];
                tasks.Add(Task.Factory.StartNew(() => action(parameter)));
                if ((tasks.Count == this.maxThreads) || i == parameters.Count - 1)
                {
                    Task.WaitAll(tasks.ToArray());
                }
            }
        }
    }
}
