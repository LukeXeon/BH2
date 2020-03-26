using UnityEngine;

namespace Dash.Scripts.Assets
{
    [CreateAssetMenu(fileName = "GameInfo", menuName = "Info/Game")]
    public class GameInfoAsset : ScriptableObject
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