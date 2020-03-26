using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Dash.Scripts.Assets
{
    public class GameInfoManager : MonoBehaviour
    {
        public static GameInfoAsset gameInfoAsset;

        public static int maxPlayerId;
        public static int maxWeaponId;
        public static int maxShengHeId;

        public static Dictionary<int, PlayerInfoAsset> playerTable;
        public static Dictionary<int, WeaponInfoAsset> weaponTable;
        public static Dictionary<int, ShengHenInfoAsset> shengHenTable;


        static GameInfoManager()
        {
            playerTable = new Dictionary<int, PlayerInfoAsset>();
            weaponTable = new Dictionary<int, WeaponInfoAsset>();
            shengHenTable = new Dictionary<int, ShengHenInfoAsset>();
            foreach (var item in Resources.LoadAll<PlayerInfoAsset>("Config/Player"))
            {
                playerTable[item.typeId] = item;
            }

            foreach (var item in Resources.LoadAll<WeaponInfoAsset>("Config/Weapon"))
            {
                weaponTable[item.typeId] = item;
            }

            foreach (var item in Resources.LoadAll<ShengHenInfoAsset>("Config/ShengHen"))
            {
                shengHenTable[item.typeId] = item;
            }

            gameInfoAsset = Resources.LoadAll<GameInfoAsset>("Config/Game").Single();
            maxPlayerId = playerTable.Keys.Max(o => o);
            maxWeaponId = weaponTable.Keys.Max(o => o);
            maxShengHeId = shengHenTable.Keys.Max(o => o);
        }

        public static LevelInfo GetWeaponLevel(int exp)
        {
            return GetLevel(exp, gameInfoAsset.weaponMaxLevel, gameInfoAsset.weaponBaseLevelExp);
        }

        public static LevelInfo GetShengHenLevel(int exp)
        {
            return GetLevel(exp, gameInfoAsset.shengHengMaxLevel, gameInfoAsset.shengHengBaseLevelExp);
        }

        public static LevelInfo GetPlayerLevel(int exp)
        {
            return GetLevel(exp, gameInfoAsset.playerMaxLevel, gameInfoAsset.playerBaseLevelExp);
        }


        private static LevelInfo GetLevel(int exp, int maxLevel, int levelExp)
        {
            int i = 1;
            for (; i <= maxLevel; i++)
            {
                exp -= levelExp * i;
                if (exp < 0)
                {
                    return new LevelInfo
                    {
                        count = i,
                        currentExp = levelExp * i + exp,
                        maxExp = i * levelExp
                    };
                }
            }

            return new LevelInfo
            {
                count = maxLevel,
                currentExp = i * levelExp,
                maxExp = i * levelExp
            };
        }
    }
}