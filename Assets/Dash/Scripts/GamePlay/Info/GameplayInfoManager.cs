using System;
using System.Collections.Generic;
using Dash.Scripts.Cloud;
using Dash.Scripts.Config;

namespace Dash.Scripts.GamePlay.Info
{
    public static class GameplayInfoManager
    {
        public static Tuple<PlayerInfoAsset, RuntimePlayerInfo> playerInfo;

        public static List<Tuple<WeaponInfoAsset, RuntimeWeaponInfo>> weaponInfos =
            new List<Tuple<WeaponInfoAsset, RuntimeWeaponInfo>>();

        public static List<Tuple<ShengHenInfoAsset, RuntimeShengHenInfo>> shengHenInfos =
            new List<Tuple<ShengHenInfoAsset, RuntimeShengHenInfo>>();

        public static CompletePlayer current;

        public static void OnJoinRoom()
        {
            var completePlayer = current;
            weaponInfos.Clear();
            shengHenInfos.Clear();
            playerInfo = Tuple.Create(GameGlobalInfoManager.playerTable[completePlayer.player.typeId],
                RuntimePlayerInfo.Build(completePlayer.player, completePlayer.shengHens));
            foreach (var eInUseWeapon in completePlayer.weapons)
            {
                var info = GameGlobalInfoManager.weaponTable[eInUseWeapon.typeId];
                var runtimeInfo = RuntimeWeaponInfo.Build(eInUseWeapon);
                weaponInfos.Add(Tuple.Create(info, runtimeInfo));
            }

            foreach (var eInUseShengHen in completePlayer.shengHens)
            {
                var info = GameGlobalInfoManager.shengHenTable[eInUseShengHen.typeId];
                var runtimeInfo = RuntimeShengHenInfo.Build(eInUseShengHen);
                shengHenInfos.Add(Tuple.Create(info, runtimeInfo));
            }
        }
        
    }
}