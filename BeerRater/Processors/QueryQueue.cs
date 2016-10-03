namespace BeerRater.Processors
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Utils;

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
        /// The name.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryQueue"/> class.
        /// </summary>
        public QueryQueue(string name) : this(name, DefaultThreads)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryQueue" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="maxThreads">The maximum threads.</param>
        public QueryQueue(string name, int maxThreads)
        {
            this.name = name;
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
            var tasks = new List<Task>(this.maxThreads);
            for (var i = 0; i < parameters.Count; i++)
            {
                var parameter = parameters[i];
                var index = i;

                tasks.Add(Task.Factory.StartNew(() =>
                {
                    Thread.CurrentThread.Name = $"{this.name}_{index}";
                    Trace.WriteLine($"Spawning thread {Thread.CurrentThread.Name}");
                    action(parameter);
                }));

                if (tasks.Count == this.maxThreads)
                {
                    var doneIndex = Task.WaitAny(tasks.ToArray());
                    tasks.RemoveAt(doneIndex);
                }
            }

            Task.WaitAll(tasks.ToArray());
        }
    }
}
