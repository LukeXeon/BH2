using System.Collections.Generic;
using System.Linq;
using Dash.Scripts.Core;
using UnityEngine;

namespace Dash.Scripts.Setting
{
    public static class GameSettingManager
    {
        public static readonly GlobalSettingAsset setting;

        public static readonly int maxPlayerId;
        public static readonly int maxWeaponId;
        public static readonly int maxSealId;

        public static readonly SortedDictionary<int, PlayerInfoAsset> playerTable;
        public static readonly SortedDictionary<int, WeaponInfoAsset> weaponTable;
        public static readonly SortedDictionary<string, WeaponTypeInfoAsset> weaponTypeTable;
        public static readonly SortedDictionary<int, SealInfoAsset> SealsTable;
        public static readonly SortedDictionary<int, LevelInfoAsset> LevelsInfoTable;

        static GameSettingManager()
        {
            var asset = Resources.Load<GlobalSettingAsset>("GlobalSetting");
            setting = asset;
            playerTable = asset.playerInfoAssets.ToSortedDictionary(i => i.typeId, i => i);
            foreach (var playerInfoAsset in asset.playerInfoAssets)
            {
                playerInfoAsset.skel.GetSkeletonData(true);
            }
            weaponTable = asset.weaponInfoAssets.ToSortedDictionary(i => i.typeId, i => i);
            weaponTypeTable = asset.weaponTypeInfoAssets.ToSortedDictionary(i => i.matchName, i => i);
            SealsTable = asset.sealInfoAssets.ToSortedDictionary(i => i.typeId, i => i);
            LevelsInfoTable = asset.levelInfoAssets.ToSortedDictionary(i => i.typeId, i => i);
            maxPlayerId = playerTable.Keys.Max(o => o);
            maxWeaponId = weaponTable.Keys.Max(o => o);
            maxSealId = SealsTable.Keys.Max(o => o);
        }

        public static RankInfo GetWeaponLevel(int exp)
        {
            return GetLevel(exp, setting.weaponMaxLevel, setting.weaponBaseLevelExp);
        }

        public static RankInfo GetSealLevel(int exp)
        {
            return GetLevel(exp, setting.sealMaxLevel, setting.sealBaseLevelExp);
        }

        public static RankInfo GetPlayerLevel(int exp)
        {
            return GetLevel(exp, setting.playerMaxLevel, setting.playerBaseLevelExp);
        }

        public static RankInfo GetUserLevel(int exp)
        {
            return GetLevel(exp, setting.userMaxLevel, setting.userBaseLevelExp);
        }

        private static RankInfo GetLevel(int exp, int maxLevel, int levelExp)
        {
            var i = 1;
            for (; i <= maxLevel; i++)
            {
                exp -= levelExp * i;
                if (exp < 0)
                    return new RankInfo
                    {
                        count = i,
                        currentExp = levelExp * i + exp,
                        maxExp = i * levelExp
                    };
            }

            return new RankInfo
            {
                count = maxLevel,
                currentExp = i * levelExp,
                maxExp = i * levelExp
            };
        }

        public static int GetDamageReduction(int fangYuLi)
        {
            return (int) Mathf.Log(fangYuLi);
        }
    }
}