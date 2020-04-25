using System;
using System.Collections.Generic;
using Dash.Scripts.Cloud;
using Dash.Scripts.Config;

namespace Dash.Scripts.GamePlay.Config
{
    [Serializable]
    public class RuntimePlayerInfo
    {
        public int fangYuLi;
        public int gongJiLi;
        public int nengLiangZhi;
        public int shengMingZhi;

        public static RuntimePlayerInfo Build(EPlayer player, List<EShengHen> shengHens)
        {
            var gongJiLi = 0;
            var fangYuLi = 0;
            var shengMingZhi = 0;
            var nengLiangZhi = 0;
            var pinfo = GameConfigManager.playerTable[player.typeId];
            var pLevel = GameConfigManager.GetPlayerLevel(player.exp);
            gongJiLi += pinfo.gongJiLi;
            gongJiLi += pinfo.gongJiLi2 * pLevel.count;
            fangYuLi += pinfo.fangYuLi;
            fangYuLi += pinfo.fangYuLi2 * pLevel.count;
            shengMingZhi += pinfo.shengMingZhi;
            shengMingZhi += pinfo.shengMingZhi2 * pLevel.count;
            nengLiangZhi += pinfo.nengLiangZhi;
            nengLiangZhi += pinfo.nengLiangZhi2 * pLevel.count;

            foreach (var s in shengHens)
            {
                var sInfo = GameConfigManager.shengHenTable[s.typeId];
                var sLevel = GameConfigManager.GetShengHenLevel(s.exp);
                fangYuLi += sInfo.fangYuLi;
                fangYuLi += sInfo.fangYuLi2 * sLevel.count;
                shengMingZhi += sInfo.shengMingZhi;
                shengMingZhi += sInfo.shengMingZhi2 * sLevel.count;
                nengLiangZhi += sInfo.nengLiangZhi;
                nengLiangZhi += sInfo.nengLiangZhi2 * sLevel.count;
            }

            return new RuntimePlayerInfo
            {
                gongJiLi = gongJiLi,
                fangYuLi = fangYuLi,
                shengMingZhi = shengMingZhi,
                nengLiangZhi = nengLiangZhi
            };
        }
    }
}