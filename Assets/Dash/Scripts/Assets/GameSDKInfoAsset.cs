using UnityEngine;

namespace Dash.Scripts.Assets
{
    [CreateAssetMenu(fileName = "SDKInfo", menuName = "Info/SDK")]
    public class GameSDKInfoAsset : ScriptableObject
    {
        [Header("LeanCloud")] public string leanCloudId;

        public string leanCloudKey;

        public string leanCloudUrl;

        [Header("Agora")]
        public string agoraAppId;

        public GameObject onErrorShow;

        [Header("github")]
        public string githubClientId;
        
        public string githubClientSecret;
    }
}