using UnityEngine;

namespace Dash.Scripts.Config
{
    [CreateAssetMenu(fileName = "SDKInfo", menuName = "Info/SDK")]
    public class GameBootInfoAsset : ScriptableObject
    {
        [Header("LeanCloud")] public string leanCloudId;

        public string leanCloudKey;

        public string leanCloudUrl;

        [Header("Agora")]
        public string agoraAppId;

        [Header("github")]
        public string githubClientId;
        
        public string githubClientSecret;
    }
}