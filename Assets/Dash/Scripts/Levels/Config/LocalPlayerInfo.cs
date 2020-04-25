using System;
using System.Collections.Generic;
using Dash.Scripts.Cloud;
using Dash.Scripts.Config;

namespace Dash.Scripts.Levels.Config
{
    public static class LocalPlayerInfo
    {
        public static Tuple<PlayerInfoAsset, RuntimePlayerInfo> playerInfo;

        public static readonly List<Tuple<WeaponInfoAsset, RuntimeWeaponInfo>> weaponInfos =
            new List<Tuple<WeaponInfoAsset, RuntimeWeaponInfo>>();

        public static readonly List<Tuple<ShengHenInfoAsset, RuntimeShengHenInfo>> shengHenInfos =
            new List<Tuple<ShengHenInfoAsset, RuntimeShengHenInfo>>();

        public static void Prepare(CompletePlayer current)
        {
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

            LocalPlayer.weaponIndex = 0;
            LocalPlayer.hp = playerInfo.Item1.shengMingZhi;
            LocalPlayer.mp = playerInfo.Item2.nengLiangZhi;
        }

        public static void Clear()
        {
            playerInfo = null;
            weaponInfos.Clear();
            shengHenInfos.Clear();
        }
    }
}