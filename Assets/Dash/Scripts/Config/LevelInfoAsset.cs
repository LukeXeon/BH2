using UnityEngine;

namespace Dash.Scripts.Config
{
    [CreateAssetMenu(fileName = "GameInfo", menuName = "Info/Game")]
    public class LevelInfoAsset : ScriptableObject
    {
        public int playerBaseLevelExp;
        public int playerLuckDrawExpAddOnce = 1;
        public int playerMaxLevel;
        public int shengHengBaseLevelExp;
        public int shengHengMaxLevel;
        public int userBaseLevelExp;
        public int userMaxLevel;
        public int weaponBaseLevelExp;
        public int weaponMaxLevel;
    }
}