using Dash.Scripts.Core;
using UnityEngine;

namespace Dash.Scripts.Setting
{
    [CreateAssetMenu(fileName = "GlobalSetting", menuName = "Info/Global Setting")]
    public class GlobalSettingAsset : ScriptableObject
    {
        [Header("Player")] public int playerBaseLevelExp;
        public int playerLuckDrawExpAddOnce = 1;
        public int playerMaxLevel;
        [Header("Seal")] public int sealBaseLevelExp;
        public int sealMaxLevel;
        [Header("User")] public int userBaseLevelExp;
        public int userMaxLevel;
        [Header("Weapon")] public int weaponBaseLevelExp;
        public int weaponMaxLevel;
        [Header("RTC SDK")] public string rtcAppId;
        [Header("Github")] public string githubClientId;
        public string githubClientSecret;
        [Header("Cloud SDK")] public string cloudId;
        public string cloudKey;
        public string cloudUrl;
        [Header("Indexers")]
        public GuidIndexer[] networkIndexers;
        public WeaponInfoAsset[] weaponInfoAssets;
        public SealInfoAsset[] sealInfoAssets;
        public PlayerInfoAsset[] playerInfoAssets;
        public LevelInfoAsset[] levelInfoAssets;
        public WeaponTypeInfoAsset[] weaponTypeInfoAssets;
        public StoreItemInfoAsset[] storeItemInfoAssets;
    }
}