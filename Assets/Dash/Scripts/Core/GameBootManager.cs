using System.Linq;
using agora_gaming_rtc;
using Dash.Scripts.Cloud;
using Dash.Scripts.Setting;
using Dash.Scripts.Gameplay.Levels;
using Parse;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Android;

namespace Dash.Scripts.Core
{
    public class GameBootManager
    {
        public static GlobalSettingAsset info;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            Application.targetFrameRate = 60;
            info = Resources.Load<GlobalSettingAsset>("GlobalSetting");
            ParseClient.Initialize(new ParseClient.Configuration
            {
                ApplicationID = info.cloudId,
                ServerURI = info.cloudUrl,
                MasterKey = info.cloudKey
            });
            GithubClient.Initialize(info.githubClientId, info.githubClientSecret);
            PhotonNetwork.PrefabPool = new PunPool();

            ParseObject.RegisterSubclass<InUseWeaponEntity>();
            ParseObject.RegisterSubclass<InUseSealEntity>();
            ParseObject.RegisterSubclass<GameUserEntity>();
            ParseObject.RegisterSubclass<PlayerEntity>();
            ParseObject.RegisterSubclass<SealEntity>();
            ParseObject.RegisterSubclass<WeaponEntity>();
            foreach (var permission in new[] {Permission.Microphone, Permission.Camera})
                if (!Permission.HasUserAuthorizedPermission(permission))
                    Permission.RequestUserPermission(permission);

            var engine = IRtcEngine.GetEngine(info.rtcAppId);
            engine.SetChannelProfile(CHANNEL_PROFILE.CHANNEL_PROFILE_GAME);
            if (Application.isEditor)
            {
                Application.quitting += IRtcEngine.Destroy;
            }
            else
            {
                void Callback(string a, string b, LogType c)
                {
                    if (c == LogType.Exception || LogType.Error == c)
                    {
                        Application.logMessageReceived -= Callback;
                        CreateErrorPanel(a + " " + b);
                    }
                }

                Application.logMessageReceived += Callback;
            }
        }

        public static void CreateErrorPanel(string text)
        {
            var go = Object.Instantiate(Resources.Load<GameObject>("Prefab/UI/Error/Error Canvas"));
            go.GetComponentInChildren<TextMeshProUGUI>()
                .text = text;
            Object.DontDestroyOnLoad(go);
        }
    }
}