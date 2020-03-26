using System.Linq;
using System.Threading.Tasks;
using agora_gaming_rtc;
using LeanCloud;
using UnityEngine;
using Dash.Scripts.Assets;
using Dash.Scripts.Network.Cloud;
using Photon.Pun;
using TMPro;

namespace Dash.Scripts
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
            var task = Task.Run(() => IRtcEngine.QueryEngine() ?? IRtcEngine.GetEngine(info.agoraAppId));
            Task.WhenAny(task, Task.Delay(1000)).GetAwaiter().OnCompleted(() =>
            {
                if (!task.IsCompleted)
                {
                    Debug.Log("声网又挂了！！！");
                    IRtcEngine.Destroy();
                }
                else
                {
                    Debug.Log("声网没挂。");
                }
            });
            Application.quitting += () => { Task.Run(IRtcEngine.Destroy); };
            cb = (a, b, c) =>
            {
                if (c == LogType.Exception)
                {
                    Application.logMessageReceived -= cb;
                    Instantiate(info.onErrorShow).GetComponentInChildren<TextMeshProUGUI>().text = a + " " + b;
                }
            };
            Application.logMessageReceived += cb;
        }
    }
}