using System.Linq;
using agora_gaming_rtc;
using Dash.Scripts.Cloud;
using Dash.Scripts.Config;
using Dash.Scripts.GamePlay.Levels;
using Parse;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Android;

namespace Dash.Scripts.Core
{
    public class GameBootManager
    {
        public static GameBootInfoAsset info;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            ParseClient.Initialize(new ParseClient.Configuration
            {
                ApplicationID = "dae5020c8c7711eaa57502fcdc4e7412",
                ServerURI = "http://175.24.83.68:1337/parse/",
                MasterKey = "e51518ca8c7711ea9afe02fcdc4e7412"
             });
            Debug.Log("Boot Loaded");
            Application.targetFrameRate = 60;
            PhotonNetwork.PrefabPool = new PunPool();
            info = Resources.LoadAll<GameBootInfoAsset>("Config/Game").Single();
            //AVClient.Initialize(info.leanCloudId, info.leanCloudKey, info.leanCloudUrl);
            ParseObject.RegisterSubclass<InUseWeaponEntity>();
            ParseObject.RegisterSubclass<InUseSealEntity>();
            ParseObject.RegisterSubclass<GameUserEntity>();
            ParseObject.RegisterSubclass<PlayerEntity>();
            ParseObject.RegisterSubclass<SealEntity>();
            ParseObject.RegisterSubclass<WeaponEntity>();
            foreach (var permission in new[] {Permission.Microphone, Permission.Camera})
                if (!Permission.HasUserAuthorizedPermission(permission))
                    Permission.RequestUserPermission(permission);

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