using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Dash.Scripts.Core;
using Spine.Collections;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Dash.Scripts.Config
{
    public class GameInfoManager : MonoBehaviour
    {
        public static readonly LevelInfoAsset levelInfo;

        public static readonly int maxPlayerId;
        public static readonly int maxWeaponId;
        public static readonly int maxShengHeId;

        public static readonly SortedDictionary<int, PlayerInfoAsset> playerTable;
        public static readonly SortedDictionary<int, WeaponInfoAsset> weaponTable;
        public static readonly SortedDictionary<string, WeaponTypeInfoAsset> weaponTypeTable;
        public static readonly SortedDictionary<int, ShengHenInfoAsset> shengHenTable;
        public static readonly SortedDictionary<int, GuanQiaInfoAsset> guanQiaInfoTable;

        static GameInfoManager()
        {
            playerTable = Resources.LoadAll<PlayerInfoAsset>("Config/Player").ToSortedDictionary(i => i.typeId, i => i);
            weaponTable = Resources.LoadAll<WeaponInfoAsset>("Config/Weapon").ToSortedDictionary(i => i.typeId, i => i);
            weaponTypeTable = Resources.LoadAll<WeaponTypeInfoAsset>("Config/WeaponType")
                .ToSortedDictionary(i => i.matchName, i => i);
            shengHenTable = Resources.LoadAll<ShengHenInfoAsset>("Config/ShengHen")
                .ToSortedDictionary(i => i.typeId, i => i);
            guanQiaInfoTable = Resources.LoadAll<GuanQiaInfoAsset>("Config/GuanQia")
                .ToSortedDictionary(i => i.typeId, i => i);
            levelInfo = Resources.LoadAll<LevelInfoAsset>("Config/Game").Single();
            maxPlayerId = playerTable.Keys.Max(o => o);
            maxWeaponId = weaponTable.Keys.Max(o => o);
            maxShengHeId = shengHenTable.Keys.Max(o => o);
        }

        public static LevelInfo GetWeaponLevel(int exp)
        {
            return GetLevel(exp, levelInfo.weaponMaxLevel, levelInfo.weaponBaseLevelExp);
        }

        public static LevelInfo GetShengHenLevel(int exp)
        {
            return GetLevel(exp, levelInfo.shengHengMaxLevel, levelInfo.shengHengBaseLevelExp);
        }

        public static LevelInfo GetPlayerLevel(int exp)
        {
            return GetLevel(exp, levelInfo.playerMaxLevel, levelInfo.playerBaseLevelExp);
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