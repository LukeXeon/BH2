using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Dash.Scripts.Core
{
    public static class UnityWebRequestExtensions
    {
        public static async Task<UnityWebRequest> SendWebRequestAsync(this UnityWebRequest request)
        {
            return await new UnityWebRequestAwaitable(request.SendWebRequest());
        }

        private struct UnityWebRequestAwaitable : INotifyCompletion
        {
            private readonly UnityWebRequestAsyncOperation operation;

            public UnityWebRequestAwaitable(UnityWebRequestAsyncOperation operation)
            {
                this.operation = operation;
            }

            public UnityWebRequestAwaitable GetAwaiter()
            {
                return this;
            }

            public void OnCompleted(Action continuation)
            {
                operation.completed += o => continuation();
            }

            public bool IsCompleted => operation.isDone;

            public UnityWebRequest GetResult()
            {
                return operation.webRequest;
            }
        }
    }
}