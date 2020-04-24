using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Dash.Scripts.Core
{
    public sealed class UnityTaskScheduler : TaskScheduler
    {
        private static volatile TaskScheduler taskScheduler;

        private readonly SynchronizationContext synchronizationContext;
        private readonly ConcurrentQueue<Task> tasks;
        private readonly WaitWhile waitQueue;

        private UnityTaskScheduler(SynchronizationContext ctx)
        {
            synchronizationContext = ctx;
            tasks = new ConcurrentQueue<Task>();
            waitQueue = new WaitWhile(() => tasks.IsEmpty);
        }

        public static TaskScheduler MainThread => taskScheduler;

        public override int MaximumConcurrencyLevel => 1;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            var go = new GameObject(nameof(UnityTaskScheduler), typeof(TaskRunner))
            {
                hideFlags = HideFlags.NotEditable & HideFlags.HideInHierarchy & HideFlags.HideInInspector
            };
            Object.DontDestroyOnLoad(go);
            var mono = go.GetComponent<TaskRunner>();
            var scheduler = new UnityTaskScheduler(SynchronizationContext.Current);
            mono.StartCoroutine(scheduler.Loop());
            taskScheduler = scheduler;
        }

        protected override void QueueTask(Task task)
        {
            tasks.Enqueue(task);
        }

        private IEnumerator Loop()
        {
            while (Application.isPlaying)
            {
                tasks.TryDequeue(out var task);
                if (task != null) TryExecuteTask(task);

                yield return waitQueue;
            }
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return SynchronizationContext.Current == synchronizationContext && TryExecuteTask(task);
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return null;
        }

        private class TaskRunner : MonoBehaviour
        {
        }
    }
}