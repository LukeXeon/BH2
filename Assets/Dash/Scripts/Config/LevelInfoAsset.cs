using UnityEngine;

namespace Dash.Scripts.Config
{
    [CreateAssetMenu(fileName = "GameInfo", menuName = "Info/Game")]
    public class LevelInfoAsset : ScriptableObject
    {
        public int shengHengMaxLevel;
        public int shengHengBaseLevelExp;
        public int weaponMaxLevel;
        public int weaponBaseLevelExp;
        public int playerMaxLevel;
        public int playerBaseLevelExp;
        public int playerLuckDrawExpAddOnce = 1;
    }
}