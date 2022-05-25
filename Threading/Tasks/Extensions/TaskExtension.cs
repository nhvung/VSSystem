using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VSSystem.Threading.Tasks.Extensions
{
    public static class TaskExtension
    {
        public static void ConsecutiveRun<TSource>(this IEnumerable<TSource> sources, Action<TSource> executeFunc, int numberOfThreads
            , Action<Exception> exceptionAction = null)
        {
            try
            {
                if (sources?.Count() > 0 && executeFunc != null)
                {
                    object lockObj = new object();
                    Queue<Action> actionQueue = new Queue<Action>();
                    foreach (var src in sources)
                    {
                        actionQueue.Enqueue(() => executeFunc.Invoke(src));
                    }
                    Action taskAction = delegate {
                        Action act = null;
                        do
                        {
                            try
                            {
                                lock (lockObj)
                                {                            
                                    if(actionQueue.Count > 0)
                                    {
                                        act = actionQueue.Dequeue();
                                    }
                                    else
                                    {
                                        act = null;
                                    }
                                }
                                if (act == null)
                                {
                                    break;
                                }
                                DateTime dt = DateTime.Now;
                                act.Invoke();
                                TimeSpan ts = DateTime.Now - dt;
                            }
                            catch (Exception ex)
                            {
                                if(ex.Message.IndexOf("Queue empty.", StringComparison.InvariantCultureIgnoreCase) < 0)
                                {
                                    exceptionAction?.Invoke(ex);
                                }
                                
                                break;
                            }
                        } while (true);
                    };
                    Task[] tasks = new Task[numberOfThreads];
                    for (int i = 0; i < numberOfThreads; i++)
                    {
                        tasks[i] = Task.Run(taskAction);
                    }
                    Task.WaitAll(tasks);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static Task ConsecutiveRunAsync<TSource>(this IEnumerable<TSource> sources, Action<TSource> executeFunc, int numberOfThreads
            , CancellationToken cancellationToken = default, Action<Exception> exceptionAction = null)
        {
            try
            {
                if (sources?.Count() > 0 && executeFunc != null)
                {
                    object lockObj = new object();
                    Queue<Action> actionQueue = new Queue<Action>();
                    foreach (var src in sources)
                    {
                        actionQueue.Enqueue(() => executeFunc.Invoke(src));
                    }
                    Action taskAction = delegate {
                        Action act = null;
                        do
                        {
                            try
                            {
                                lock (lockObj)
                                {
                                    if (actionQueue.Count > 0)
                                    {
                                        act = actionQueue.Dequeue();
                                    }
                                    else
                                    {
                                        act = null;
                                    }
                                }
                                if (act == null)
                                {
                                    break;
                                }
                                DateTime dt = DateTime.Now;
                                act.Invoke();
                                TimeSpan ts = DateTime.Now - dt;
                            }
                            catch (Exception ex)
                            {
                                if (ex.Message.IndexOf("Queue empty.", StringComparison.InvariantCultureIgnoreCase) < 0)
                                {
                                    exceptionAction?.Invoke(ex);
                                }

                                break;
                            }
                        } while (true);
                    };
                    Task[] tasks = new Task[numberOfThreads];
                    for (int i = 0; i < numberOfThreads; i++)
                    {
                        tasks[i] = Task.Run(taskAction, cancellationToken);
                    }
                    Task.WaitAll(tasks, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Task.CompletedTask;
        }

        public static List<TResult> ConsecutiveRun<TSource, TResult>(this IEnumerable<TSource> sources, Func<TSource, TResult> executeFunc, int numberOfThreads
            , Action<Exception> exceptionAction = null)
        {
            List<TResult> result = new List<TResult>();
            try
            {
                if (sources?.Count() > 0 && executeFunc != null)
                {
                    object lockObj = new object();
                    Queue<Action> actionQueue = new Queue<Action>();
                    foreach (var src in sources)
                    {
                        actionQueue.Enqueue(delegate { 
                            var tResult = executeFunc.Invoke(src);
                            lock (lockObj)
                            {
                                result.Add(tResult); 
                            }
                        });
                    }
                    Action taskAction = delegate {
                        Action act = null;
                        do
                        {
                            try
                            {
                                lock (lockObj)
                                {
                                    if (actionQueue.Count > 0)
                                    {
                                        act = actionQueue.Dequeue();
                                    }
                                    else
                                    {
                                        act = null;
                                    }
                                }
                                if (act == null)
                                {
                                    break;
                                }
                                DateTime dt = DateTime.Now;
                                act.Invoke();
                                TimeSpan ts = DateTime.Now - dt;
                            }
                            catch (Exception ex)
                            {
                                if (ex.Message.IndexOf("Queue empty.", StringComparison.InvariantCultureIgnoreCase) < 0)
                                {
                                    exceptionAction?.Invoke(ex);
                                }
                                break;
                            }
                        } while (true);
                    };
                    Task[] tasks = new Task[numberOfThreads];
                    for (int i = 0; i < numberOfThreads; i++)
                    {
                        tasks[i] = Task.Run(taskAction);
                    }
                    Task.WaitAll(tasks);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public static void ParallelRun<TSource>(this IEnumerable<TSource> sources, Action<TSource> executeFunc, int numberOfThreads
            , Action<Exception> exceptionAction = null)
        {
            try
            {
                if (sources?.Count() > 0 && executeFunc != null)
                {
                    TSource[] exeSources = sources.ToArray();
                    object lockObj = new object();
                    while (exeSources.Length > 0)
                    {
                        Task[] tasks = exeSources.Take(numberOfThreads)
                            .Select(src => Task.Run(() => {
                                DateTime dt = DateTime.Now;
                                try
                                {
                                    executeFunc.Invoke(src);
                                }
                                catch (Exception ex)
                                {
                                    exceptionAction?.Invoke(ex);
                                }
                                TimeSpan ts = DateTime.Now - dt;
                            }))
                            .ToArray();
                        Task.WaitAll(tasks);
                        exeSources = exeSources.Skip(numberOfThreads).ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
