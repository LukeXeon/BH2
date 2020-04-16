using System;
using System.Collections.Generic;
using System.Linq;
using Dash.Scripts.Cloud;
using Dash.Scripts.Config;

namespace Dash.Scripts.GamePlay.Info
{
    [Serializable]
    public class RuntimePlayerInfo
    {
        public int gongJiLi;
        public int fangYuLi;
        public int shengMingZhi;
        public int nengLiangZhi;

        public static RuntimePlayerInfo Build(EPlayer player, List<EShengHen> shengHens)
        {
            int gongJiLi = 0;
            int fangYuLi = 0;
            int shengMingZhi = 0;
            int nengLiangZhi = 0;
            var pinfo = GameGlobalInfoManager.playerTable[player.typeId];
            var pLevel = GameGlobalInfoManager.GetPlayerLevel(player.exp);
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
                var sInfo = GameGlobalInfoManager.shengHenTable[s.typeId];
                var sLevel = GameGlobalInfoManager.GetShengHenLevel(s.exp);
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