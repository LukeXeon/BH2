using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Dash.Scripts.Core
{
    public static class UnityAsyncExtensions
    {
        public struct UnityAsyncOperationAwaitable : INotifyCompletion
        {
            private readonly AsyncOperation operation;

            public UnityAsyncOperationAwaitable(AsyncOperation operation)
            {
                this.operation = operation;
            }

            public void OnCompleted(Action continuation)
            {
                operation.completed += o => continuation();
            }

            public bool IsCompleted => operation.isDone;

            public AsyncOperation GetResult() => operation;
        }

        public static UnityAsyncOperationAwaitable GetAwaiter(this AsyncOperation operation)
        {
            return new UnityAsyncOperationAwaitable(operation);
        }
    }
}