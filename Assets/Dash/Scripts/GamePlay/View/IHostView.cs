using System;
using System.Collections.Generic;
using System.Reflection;
using Photon.Pun;

namespace Dash.Scripts.GamePlay.View
{
    public interface IHostView
    {
        PhotonView PhotonView { get; }

        void OnChildRpc(string method, object[] args);
    }

    public static class HostViewUtils
    {
        private static readonly Dictionary<(Type, string), MethodInfo> methodInfos =
            new Dictionary<(Type, string), MethodInfo>();

        public static void HandleChildRpc(this IHostView hostView, object child, string method, object[] args)
        {
            var type = child.GetType();
            methodInfos.TryGetValue((type, method), out var methodInfo);
            if (methodInfo == null)
            {
                methodInfo = type.GetMethod(method);
                methodInfos[(type, method)] = methodInfo;
            }

            if (methodInfo != null)
            {
                methodInfo.Invoke(child, args);
            }
        }
    }
}