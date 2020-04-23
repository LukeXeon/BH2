using System.Linq;
using agora_gaming_rtc;
using Dash.Scripts.Cloud;
using Dash.Scripts.Config;
using Dash.Scripts.Levels.Core;
using LeanCloud;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using Debug = UnityEngine.Debug;

namespace Dash.Scripts.Core
{
    public class GameBootManager
    {
        public static GameBootInfoAsset info;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            Debug.Log("Boot Loaded");
            Application.targetFrameRate = 60;
            PhotonNetwork.PrefabPool = new PunPool();
            PhotonNetwork.LogLevel = Application.isEditor ? PunLogLevel.Full : PunLogLevel.ErrorsOnly;
            info = Resources.LoadAll<GameBootInfoAsset>("Config/Game").Single();
            AVClient.Initialize(info.leanCloudId, info.leanCloudKey, info.leanCloudUrl);
            AVObject.RegisterSubclass<EInUseWeapon>();
            AVObject.RegisterSubclass<EInUseShengHen>();
            AVObject.RegisterSubclass<EUserMate>();
            AVObject.RegisterSubclass<EPlayer>();
            AVObject.RegisterSubclass<EShengHen>();
            AVObject.RegisterSubclass<EWeapon>();
            foreach (string permission in new[] {Permission.Microphone, Permission.Camera})
            {
                if (!Permission.HasUserAuthorizedPermission(permission))
                {
                    Permission.RequestUserPermission(permission);
                }
            }

            var engine = IRtcEngine.GetEngine(info.agoraAppId);
            engine.SetChannelProfile(CHANNEL_PROFILE.CHANNEL_PROFILE_GAME);
            if (Application.isEditor)
            {
                Application.quitting += IRtcEngine.Destroy;
            }
            else
            {
                void Callback(string a, string b, LogType c)
                {
                    if (c == LogType.Exception)
                    {
                        Application.logMessageReceived -= Callback;
                        var go = Object.Instantiate(Resources.Load<GameObject>("Prefab/UI/Error/Error Canvas"));
                        go.GetComponentInChildren<TextMeshProUGUI>()
                                .text = a + " " + b;
                        Object.DontDestroyOnLoad(go);
                    }
                }

                Application.logMessageReceived += Callback;
            }
        }
    }
}