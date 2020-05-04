using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Dash.Scripts.Core;
using Flurl;
using Newtonsoft.Json;
using Parse;
using Parse.Core.Internal;
using UnityEngine;
using UnityEngine.Networking;

namespace Dash.Scripts.Cloud
{
    public static class GithubClient
    {
        private const string CallbackUrl = "http://127.0.0.1:8080/oauth/redirect/";

        private const string GetUserUrl = @"https://api.github.com/user";

        private const string LogInBaseUrl = @"https://github.com/login/oauth/authorize";

        private const string AccessTokenUrl = @"https://github.com/login/oauth/access_token";

        private static string ApplicationId;
        private static string ClientSecret;

        public static void Initialize(string applicationId, string clientSecret)
        {
            ApplicationId = applicationId;
            ClientSecret = clientSecret;
            typeof(ParseUser).GetMethod(
                "RegisterProvider",
                BindingFlags.NonPublic | BindingFlags.Static,
                null,
                new[] {typeof(IParseAuthenticationProvider)},
                null
            )?.Invoke(null, new object[] {new GithubAuthenticationProvider()});
        }

        public static string LogInUrl
        {
            get
            {
                var url = new Url(LogInBaseUrl);
                url.SetQueryParam("client_id", ApplicationId);
                url.SetQueryParam("redirect_uri", CallbackUrl);
                Debug.Log(url);
                return url;
            }
        }

        public static async Task<ParseUser> LogInAsync(CancellationToken cancellationToken)
        {
            var user = await ParseUserExtensions.LogInWithAsync("github", cancellationToken);
            await CloudManager.HandleLogin();
            return user;
        }

        private class GithubAuthenticationProvider : IParseAuthenticationProvider
        {
            private readonly TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            public async Task<IDictionary<string, object>> AuthenticateAsync(CancellationToken cancellationToken)
            {
                var http = new HttpListener {AuthenticationSchemes = AuthenticationSchemes.None};
                http.Prefixes.Add(CallbackUrl);
                http.Start();
                var ctxTask = http.GetContextAsync();
                string code;
                try
                {
                    while (!cancellationToken.IsCancellationRequested &&
                           !(ctxTask.IsCompleted || ctxTask.IsCanceled || ctxTask.IsFaulted))
                    {
                        await Task.Yield();
                    }

                    if (cancellationToken.IsCancellationRequested || ctxTask.IsCanceled || ctxTask.IsFaulted)
                    {
                        http.Abort();
                        throw new TaskCanceledException();
                    }

                    code = (await ctxTask).Request.QueryString["code"];
                    Debug.Log(code);
                }
                finally
                {
                    http.Abort();
                    Debug.Log("Abort");
                }

                var url = new Url(AccessTokenUrl);
                url.SetQueryParam("client_id", ApplicationId);
                url.SetQueryParam("client_secret", ClientSecret);
                url.SetQueryParam("code", code);
                var wr = UnityWebRequest.Post(url, string.Empty);
                wr.SetRequestHeader("Accept", "application/json");
                await wr.SendWebRequestAsync();
                var token = JsonConvert.DeserializeObject<Dictionary<string, string>>(
                    wr.downloadHandler.text
                )["access_token"];
                wr = UnityWebRequest.Get(GetUserUrl);
                wr.SetRequestHeader("Authorization", "token " + token);
                wr.SetRequestHeader("Accept", "application/json");
                await wr.SendWebRequestAsync();
                if (wr.responseCode == 200)
                {
                    var userInfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(wr.downloadHandler.text);
                    var result = new Dictionary<string, object>
                    {
                        {"access_token", token},
                        {"id", userInfo["id"]}
                    };
                    PlayerPrefs.SetString(AuthType, JsonConvert.SerializeObject(result));
                    PlayerPrefs.Save();
                    return result;
                }

                if (wr.responseCode == 0)
                {
                    throw new IOException("国内的Github服务暂时不可用");
                }

                throw new IOException("error code " + wr.responseCode);
            }

            public void Deauthenticate()
            {
                Task.Factory.StartNew(() =>
                    {
                        PlayerPrefs.DeleteKey(AuthType);
                        PlayerPrefs.Save();
                    }, default,
                    TaskCreationOptions.None, taskScheduler
                );
            }

            public bool RestoreAuthentication(IDictionary<string, object> authData)
            {
                var l = JsonConvert.SerializeObject(authData);
                var r = Task<string>.Factory.StartNew(() => PlayerPrefs.GetString(AuthType), default,
                    TaskCreationOptions.None, taskScheduler);
                return l == r.Result;
            }

            public string AuthType => "github";
        }
    }
}