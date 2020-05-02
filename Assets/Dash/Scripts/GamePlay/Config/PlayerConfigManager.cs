using System;
using System.Collections.Generic;
using Dash.Scripts.Cloud;
using Dash.Scripts.Config;
using UnityEngine;

namespace Dash.Scripts.GamePlay.Config
{
    public static class PlayerConfigManager
    {
        public static Tuple<PlayerInfoAsset, RuntimePlayerInfo> playerInfo;

        public static readonly List<Tuple<WeaponInfoAsset, RuntimeWeaponInfo>> weaponInfos =
            new List<Tuple<WeaponInfoAsset, RuntimeWeaponInfo>>();

        public static readonly List<Tuple<ShengHenInfoAsset, RuntimeShengHenInfo>> shengHenInfos =
            new List<Tuple<ShengHenInfoAsset, RuntimeShengHenInfo>>();

        public static void Prepare(CompletePlayer current)
        {
            weaponInfos.Clear();
            shengHenInfos.Clear();
            var completePlayer = current;
            playerInfo = Tuple.Create(GameConfigManager.playerTable[completePlayer.player.typeId],
                RuntimePlayerInfo.Build(completePlayer.player, completePlayer.seals));
            foreach (var eInUseWeapon in completePlayer.weapons)
            {
                var info = GameConfigManager.weaponTable[eInUseWeapon.typeId];
                var runtimeInfo = RuntimeWeaponInfo.Build(eInUseWeapon);
                weaponInfos.Add(Tuple.Create(info, runtimeInfo));
            }

            foreach (var eInUseShengHen in completePlayer.seals)
            {
                var info = GameConfigManager.shengHenTable[eInUseShengHen.typeId];
                var runtimeInfo = RuntimeShengHenInfo.Build(eInUseShengHen);
                shengHenInfos.Add(Tuple.Create(info, runtimeInfo));
            }

            LocalPlayer.weaponIndex = 0;
            LocalPlayer.hp = playerInfo.Item2.shengMingZhi;
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