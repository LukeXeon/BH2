using System.Linq;
using agora_gaming_rtc;
using Dash.Scripts.Network.Cloud;
using LeanCloud;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Android;

namespace Dash.Scripts.Config
{
    public class GameSDKManager : MonoBehaviour
    {
        public static GameSDKManager instance;

        [HideInInspector] public GameSDKInfoAsset info;

        private Application.LogCallback cb;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void LoadSDKManager()
        {
            instance = new GameObject(typeof(GameSDKManager).Name, typeof(GameSDKManager))
                .GetComponent<GameSDKManager>();
        }

        private void Awake()
        {
            Application.targetFrameRate = 60;
            PhotonNetwork.LogLevel = Application.isEditor ? PunLogLevel.Full : PunLogLevel.ErrorsOnly;
            info = Resources.LoadAll<GameSDKInfoAsset>("Config/Game").Single();
            AVClient.Initialize(info.leanCloudId, info.leanCloudKey, info.leanCloudUrl);
            AVObject.RegisterSubclass<EInUseWeapon>();
            AVObject.RegisterSubclass<EInUseShengHen>();
            AVObject.RegisterSubclass<EUserMate>();
            AVObject.RegisterSubclass<EPlayer>();
            AVObject.RegisterSubclass<EShengHen>();
            AVObject.RegisterSubclass<EWeapon>();
            DontDestroyOnLoad(this.gameObject);
            foreach (string permission in new[] {Permission.Microphone, Permission.Camera})
            {
                if (!Permission.HasUserAuthorizedPermission(permission))
                {
                    Permission.RequestUserPermission(permission);
                }
            }
            var engine = IRtcEngine.GetEngine(info.agoraAppId);
            engine.SetChannelProfile(CHANNEL_PROFILE.CHANNEL_PROFILE_GAME);
            Application.quitting += IRtcEngine.Destroy;
            cb = (a, b, c) =>
            {
                if (c == LogType.Exception)
                {
                    Application.logMessageReceived -= cb;
                    Instantiate(Resources.Load<GameObject>("Prefab/UI/Error/Error Canvas"))
                            .GetComponentInChildren<TextMeshProUGUI>().text = a + " " + b;
                }
            };
            Application.logMessageReceived += cb;
        }
    }
}