using System;
using System.Collections.Generic;
using Dash.Scripts.Cloud;
using Dash.Scripts.Config;

namespace Dash.Scripts.Levels.Config
{
    public static class LevelConfigManager
    {
        public static Tuple<PlayerInfoAsset, RuntimePlayerInfo> playerInfo;

        public static readonly List<Tuple<WeaponInfoAsset, RuntimeWeaponInfo>> weaponInfos =
            new List<Tuple<WeaponInfoAsset, RuntimeWeaponInfo>>();

        public static readonly List<Tuple<ShengHenInfoAsset, RuntimeShengHenInfo>> shengHenInfos =
            new List<Tuple<ShengHenInfoAsset, RuntimeShengHenInfo>>();

        public static int currentWeaponIndex;

        public static void Prepare(CompletePlayer current)
        {
            currentWeaponIndex = 0;
            var completePlayer = current;
            weaponInfos.Clear();
            shengHenInfos.Clear();
            playerInfo = Tuple.Create(GameConfigManager.playerTable[completePlayer.player.typeId],
                RuntimePlayerInfo.Build(completePlayer.player, completePlayer.shengHens));
            foreach (var eInUseWeapon in completePlayer.weapons)
            {
                var info = GameConfigManager.weaponTable[eInUseWeapon.typeId];
                var runtimeInfo = RuntimeWeaponInfo.Build(eInUseWeapon);
                weaponInfos.Add(Tuple.Create(info, runtimeInfo));
            }

            foreach (var eInUseShengHen in completePlayer.shengHens)
            {
                var info = GameConfigManager.shengHenTable[eInUseShengHen.typeId];
                var runtimeInfo = RuntimeShengHenInfo.Build(eInUseShengHen);
                shengHenInfos.Add(Tuple.Create(info, runtimeInfo));
            }
        }

        public static void Clear()
        {
            currentWeaponIndex = 0;
            playerInfo = null;
            weaponInfos.Clear();
            shengHenInfos.Clear();
        }
    }
}