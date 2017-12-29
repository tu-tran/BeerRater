namespace BeerRater.Utils
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class QueryQueue
    {
        /// <summary>
        /// The maximum threads.
        /// </summary>
        private readonly int maxThreads;

        /// <summary>
        /// The name.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The tasks.
        /// </summary>
        private readonly List<Task> tasks;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryQueue" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="maxThreads">The maximum threads.</param>
        internal QueryQueue(string name, int maxThreads)
        {
            this.name = name;
            this.maxThreads = maxThreads < 1 ? 1 : maxThreads;
            this.tasks = new List<Task>();
        }

        /// <summary>
        /// Starts the specified action.
        /// </summary>
        /// <typeparam name="TParam">The type of the parameter.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="parameters">The parameters.</param>
        public void Start<TParam>(Action<TParam, int> action, IList<TParam> parameters)
        {
            for (var i = 0; i < parameters.Count; i++)
            {
                var parameter = parameters[i];
                var index = i;

                if (this.tasks.Count == this.maxThreads)
                {
                    Task.WaitAny(this.tasks.ToArray());
                }

                this.tasks.RemoveAll(t => t.IsCompleted);
                this.tasks.Add(Task.Factory.StartNew(() =>
                {
                    try
                    {
                        Trace.WriteLine($"Spawning thread {this.name}_{this.tasks.Count}/{this.maxThreads}");
                        action(parameter, index);
                    }
                    catch (Exception e)
                    {
                        e.OutputError();
                    }
                }));
            }

            Task.WaitAll(this.tasks.ToArray());
            this.tasks.RemoveAll(t => t.IsCompleted);
        }
    }
}
