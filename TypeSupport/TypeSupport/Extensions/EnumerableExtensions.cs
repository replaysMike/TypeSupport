using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TypeSupport.Extensions
{
    /// <summary>
    /// Enumerable extensions
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// An awaitable ForEach implementation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="degreeOfParallelism">Number of threads to use</param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static Task ForEachAsync<T>(this IEnumerable<T> source, int degreeOfParallelism, Func<T, Task> body)
        {
#if FEATURE_TASK
            return Task.WhenAll(
                from partition in Partitioner.Create(source).GetPartitions(degreeOfParallelism)
                select Task.Run(async delegate {
                    using (partition)
                    {
                        while (partition.MoveNext())
                        {
                            await body(partition.Current);
                        }
                    }
                }));
#else
            var taskFactory = new TaskFactory();
            var tasks = from partition in Partitioner.Create(source).GetPartitions(degreeOfParallelism)
                        select taskFactory.StartNew(delegate
                        {
                            using (partition)
                            {
                                while (partition.MoveNext())
                                {
                                    body(partition.Current);
                                }
                            }
                        });
            return taskFactory.ContinueWhenAll(tasks.ToArray(), continuation => { }, TaskContinuationOptions.ExecuteSynchronously);
#endif
        }

        /// <summary>
        /// An awaitable ForEach implementation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static Task ForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> body)
        {
            return source.ForEachAsync<T>(Environment.ProcessorCount, body);
        }

        /// <summary>
        /// Determines wether an element is in the List<> with case sensitivity options
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="toCheck"></param>
        /// <param name="comp"></param>
        /// <returns></returns>
        public static bool Contains<T>(this IEnumerable<T> source, string toCheck, StringComparison comp)
        {
            foreach (var el in source)
            {
                var els = el.ToString();
                if (els.IndexOf(toCheck, comp) >= 0)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether a <typeparamref name="T"/> contains any items in another <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type of data to compare</typeparam>
        /// <param name="source">The source enumerable/list</param>
        /// <param name="anyItems">The enumerable/list containing items to compare</param>
        /// <returns></returns>
        public static bool ContainsAny<T>(this IEnumerable<T> source, IEnumerable<T> anyItems)
        {
            return source.Any(x => anyItems.Any(y => y.Equals(x)));
        }

        /// <summary>
        /// Fill/append a list with default values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="count">The number of items to add</param>
        public static IEnumerable<T> Fill<T>(this ICollection<T> source, int count)
        {
            var type = typeof(T);
            for (var i = 0; i < count; i++)
            {
                if (type.IsValueType)
                    source.Add((T)Convert.ChangeType(0, type));
                else
                    source.Add(Activator.CreateInstance<T>());
            }

            return source;
        }
    }
}
