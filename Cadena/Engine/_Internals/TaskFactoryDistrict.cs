﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cadena.Engine._Internals
{
    internal sealed class TaskFactoryDistrict
    {
        private readonly Dictionary<int, TaskFactory> _factories = new Dictionary<int, TaskFactory>();

        private readonly SortedDictionary<int, ShadowTaskScheduler> _shadowSchedulers =
            new SortedDictionary<int, ShadowTaskScheduler>();

        private readonly int _maxConcurrency;

        private int _currentWorkingTasks;

        /// <summary>
        /// Instantiate new task factory district.
        /// </summary>
        /// <param name="maxConcurrency"></param>
        public TaskFactoryDistrict(int maxConcurrency = -1)
        {
            if (maxConcurrency == 0 || maxConcurrency < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxConcurrency));
            }
            _maxConcurrency = maxConcurrency;
        }

        public TaskFactory GetOrCreateFactory(int priority)
        {
            lock (_factories)
            {
                if (!_factories.TryGetValue(priority, out var factory))
                {
                    lock (_shadowSchedulers)
                    {
                        factory = _factories[priority] = new TaskFactory(
                            _shadowSchedulers[priority] = new ShadowTaskScheduler(this));
                    }
                }
                return factory;
            }
        }

        #region Task runner utilities

        public Task Run(Action task, int priority, CancellationToken token = new CancellationToken())
        {
            return GetOrCreateFactory(priority).StartNew(task, token);
        }

        public Task Run(Func<Task> task, int priority, CancellationToken token = new CancellationToken())
        {
            return GetOrCreateFactory(priority).StartNew(task, token).Unwrap();
        }

        public Task<T> Run<T>(Func<T> task, int priority, CancellationToken token = new CancellationToken())
        {
            return GetOrCreateFactory(priority).StartNew(task, token);
        }

        public Task<T> Run<T>(Func<Task<T>> task, int priority, CancellationToken token = new CancellationToken())
        {
            return GetOrCreateFactory(priority).StartNew(task, token).Unwrap();
        }

        #endregion Task runner utilities

        // -------------------- Task Runner --------------------

        private void InvokeNewTask()
        {
            // fire and forget
            Task.Run(TaskRunner, CancellationToken.None);
        }

        private async Task TaskRunner()
        {
            while (true)
            {
                Func<Task> executor;
                lock (_shadowSchedulers)
                {
                    executor = _shadowSchedulers.Values
                                                .Select(s => s.GetNextLocalTaskExecutor())
                                                .FirstOrDefault(a => a != null);
                    if (executor == null)
                    {
                        _currentWorkingTasks--;
                        return;
                    }
                }
                var result = executor();
                await result.ConfigureAwait(false);
                /*
                if (result is Task<Task> wrappedTask)
                {
                    await wrappedTask.Unwrap().ConfigureAwait(false);
                }
                */
            }
        }

        private sealed class ShadowTaskScheduler : TaskScheduler
        {
            private readonly TaskFactoryDistrict _parent;

            // must be locked with _parent._shadowSchedulers, not lock(_taskList)!
            private readonly LinkedList<Task> _taskList;

            public ShadowTaskScheduler(TaskFactoryDistrict parent)
            {
                _taskList = new LinkedList<Task>();
                _parent = parent;
            }

            public Func<Task> GetNextLocalTaskExecutor()
            {
                var task = GetNextLocalTask();
                if (task == null) return null;
                return () => TryExecuteTask(task) ? task : null;
            }

            private Task GetNextLocalTask()
            {
                if (_taskList.Count == 0)
                {
                    return null;
                }
                var item = _taskList.First.Value;
                _taskList.RemoveFirst();
                return item;
            }

            protected override void QueueTask(Task task)
            {
                lock (_parent._shadowSchedulers)
                {
                    _taskList.AddLast(task);
                    if (_parent._currentWorkingTasks >= _parent._maxConcurrency)
                    {
                        // subtasks are already executed with maximum concurrency level.
                        return;
                    }
                    // invoke new subtask
                    _parent._currentWorkingTasks++;
                    _parent.InvokeNewTask();
                }
            }

            protected override IEnumerable<Task> GetScheduledTasks()
            {
                lock (_parent._shadowSchedulers)
                {
                    return _taskList.ToArray();
                }
            }

            // -------------------- Task Runner --------------------

            protected override bool TryDequeue(Task task)
            {
                lock (_parent._shadowSchedulers)
                {
                    return _taskList.Remove(task);
                }
            }

            protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
            {
                if (taskWasPreviouslyQueued)
                {
                    if (!TryDequeue(task))
                    {
                        return false;
                    }
                }
                return TryExecuteTask(task);
            }
        }
    }
}