using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SinGooCMS.Utility.Extension
{
    /// <summary>
    /// <see cref="System.Threading.Tasks"/>扩展类
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// 设置当前任务的最长等待时间<paramref name="timeout"/>，如果在指定的时间内还未执行完毕就报异常
        /// </summary>
        public static async Task<T> TimeoutAfter<T>(this Task<T> task, TimeSpan timeout)
        {
            await TimeoutAfterImpl(task, timeout).ConfigureAwait(false);
            return task.Result;
        }

        /// <summary>
        /// 设置当前任务集合全部执行完毕的最长等待时间<paramref name="timeout"/>，如果在指定的时间内还未执行完毕就报异常
        /// </summary>
        public static async Task<IEnumerable<Task<T>>> TimeoutAfter<T>(
            this IEnumerable<Task<T>> tasks, TimeSpan timeout)
        {
            await TimeoutAfterImpl(tasks, timeout).ConfigureAwait(false);
            return tasks;
        }

        /// <summary>
        /// 设置当前任务的最长等待时间<paramref name="timeout"/>，如果在指定的时间内还未执行完毕就报异常
        /// </summary>
        public static async Task TimeoutAfter(this Task task, TimeSpan timeout) =>
            await TimeoutAfterImpl(task, timeout).ConfigureAwait(false);

        /// <summary>
        /// 设置当前任务集合全部执行完毕的最长等待时间<paramref name="timeout"/>，如果在指定的时间内还未执行完毕就报异常
        /// </summary>
        public static async Task TimeoutAfter(this IEnumerable<Task> tasks, TimeSpan timeout) =>
            await TimeoutAfterImpl(tasks, timeout).ConfigureAwait(false);

        /// <summary>
        /// 使用指定的逻辑顺序处理任务集合的返回结果
        /// </summary>
        public static async Task ForEachInOrder<T>(this IEnumerable<Task<T>> tasks, Action<T> action)
        {
            if (tasks is null) { throw new ArgumentNullException(nameof(tasks)); }

            foreach (var task in tasks)
            {
                var value = await task.ConfigureAwait(false);
                action(value);
            }
        }

        /// <summary>
        /// 使用指定的逻辑顺序处理任务集合的返回结果
        /// </summary>
        public static async Task ForEachInOrder(this IEnumerable<Task> tasks, Action<Task> action)
        {
            if (tasks is null) { throw new ArgumentNullException(nameof(tasks)); }

            foreach (var task in tasks)
            {
                await task.ConfigureAwait(false);
                action(task);
            }
        }

        /// <summary>
        /// 等待当前集合中所有任务的返回，代码示例：
        /// </summary>
        /// <example>
        /// <code>
        /// var res3 = await new List&lt;Task&lt;int&gt;&gt;() {
        ///     Task.Run&lt;int&gt;(()=>1),
        ///     Task.Run&lt;int&gt;(()=>2),
        ///     Task.Run&lt;int&gt;(()=>3)
        /// };
        /// //输出: 1,2,3
        /// Console.WriteLine(string.Join(",", res3));
        /// </code>
        /// </example>
        public static TaskAwaiter<T[]> GetAwaiter<T>(this IEnumerable<Task<T>> tasks) =>
            Task.WhenAll(tasks).GetAwaiter();

        /// <summary>
        /// 等待当前集合中所有任务的返回
        /// </summary>
        /// <example>
        /// <code>
        ///     var res3 = await new List&lt;Task&gt;() {
        ///         Task.Run(()=>{}),
        ///         Task.Run(()=>{}),
        ///         Task.Run(()=>{})
        ///     };
        /// </code>
        /// </example>
        public static TaskAwaiter GetAwaiter(this IEnumerable<Task> tasks) =>
            Task.WhenAll(tasks).GetAwaiter();

        #region Exception Handling
        /// <summary>
        /// 处理当前任务中出现的异常
        /// </summary>
        /// <example>
        /// <code>
        /// Task.Run(() =>
        /// {
        ///     int i = 0;
        ///     i = 1 / i;
        ///  }).HandleExceptions((ex) =>
        ///  {
        ///     //输出: 尝试除以0
        ///     Console.WriteLine(ex.Message);
        ///  }).Wait();
        /// </code>
        /// </example>
        /// <param name="task">The task which might throw exceptions</param>
        /// <param name="exceptionsHandler">The handler to which every exception is passed</param>
        /// <returns>当前任务的延续任务</returns>
        public static Task HandleExceptions(this Task task, Action<Exception> exceptionsHandler)
        {
            return task.ContinueWith(t =>
            {
                var aggEx = t.Exception;

                if (aggEx is null) { return; }

                aggEx.Flatten().Handle(ie =>
                {
                    exceptionsHandler(ie);
                    return true;
                });
            },
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);
        }

        /// <summary>
        /// 使用<paramref name="exceptionHandler"/>处理当前任务预测 (<paramref name="exceptionPredicate"/>) 的异常, 如果<paramref name="exceptionPredicate"/>返回false, 则抛出异常
        /// </summary>
        /// 参考：
        /// <seealso cref="TaskExtensions.HandleException{T}(Task, Action{T})"/>
        /// <remarks>
        /// </remarks>
        public static Task HandleExceptions(
            this Task task, Func<Exception, bool> exceptionPredicate, Action<Exception> exceptionHandler)
        {
            return task.ContinueWith(t =>
            {
                var aggEx = t.Exception;

                if (aggEx is null) { return; }

                aggEx.Flatten().Handle(ie =>
                {
                    if (exceptionPredicate(ie))
                    {
                        exceptionHandler(ie);
                        return true;
                    }

                    return false;
                });
            },
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);
        }

        /// <summary>
        /// 预测当前任务抛出的异常是否是<typeparamref name="T"/>类型, 如果是,则调用<paramref name="exceptionHandler"/>处理异常, 否则抛出异常
        /// </summary>
        /// <remarks>
        /// 参考：
        /// <seealso cref="TaskExtensions.HandleException{T}(Task, Action{T})"/>
        /// </remarks>
        public static Task HandleException<T>(this Task task, Action<T> exceptionHandler)
            where T : Exception
        {
            return task.ContinueWith(t =>
            {
                var aggEx = t.Exception;

                if (aggEx is null) { return; }

                aggEx.Flatten().Handle(ex =>
                {
                    if (ex is T expectedException)
                    {
                        exceptionHandler(expectedException);
                        return true;
                    }

                    return false;
                });
            },
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);
        }
        #endregion

        private static async Task TimeoutAfterImpl(this Task task, TimeSpan timeoutPeriod)
        {
            if (task is null) { throw new ArgumentNullException(nameof(task)); }

            using (var cts = new CancellationTokenSource())
            {
                var timeoutTask = Task.Delay(timeoutPeriod, cts.Token);
                var finishedTask = await Task.WhenAny(task, timeoutTask).ConfigureAwait(false);

                if (finishedTask == timeoutTask)
                {
                    throw new TimeoutException("Task timed out after: " + timeoutPeriod.ToString());
                }

                cts.Cancel();
            }                
        }

        private static async Task TimeoutAfterImpl(this IEnumerable<Task> tasks, TimeSpan timeoutPeriod)
        {
            if (tasks is null) { throw new ArgumentNullException(nameof(tasks)); }

            using (var cts = new CancellationTokenSource())
            {
                var cToken = cts.Token;
                var timeoutTask = Task.Delay(timeoutPeriod, cToken);
                var tasksList = new List<Task>(tasks) { timeoutTask };

                while (tasksList.Count > 0)
                {
                    var finishedTask = await Task.WhenAny(tasksList).ConfigureAwait(false);

                    if (finishedTask == timeoutTask)
                    {
                        throw new TimeoutException("At least one of the tasks timed out after: " + timeoutPeriod.ToString());
                    }

                    tasksList.Remove(finishedTask);

                    if (tasksList.Count == 1 && tasksList[0] == timeoutTask) { break; }
                }

                cts.Cancel();
            }                
        }
    }
}