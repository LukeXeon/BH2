using UnityEngine;

namespace Dash.Scripts.Config
{
    [CreateAssetMenu(fileName = "SDKInfo", menuName = "Info/SDK")]
    public class GameBootInfoAsset : ScriptableObject
    {
        [Header("Agora")] public string agoraAppId;

        [Header("Github")] public string githubClientId;

        public string githubClientSecret;
        [Header("LeanCloud")] public string leanCloudId;

        public string leanCloudKey;

        public string leanCloudUrl;
    }
}