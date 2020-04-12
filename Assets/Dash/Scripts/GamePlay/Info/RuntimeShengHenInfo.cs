using Dash.Scripts.Config;
using Dash.Scripts.Network.Cloud;

namespace Dash.Scripts.GamePlay.Info
{
    
    public class RuntimeShengHenInfo
    {
        public int fangYuLi;
        public int shengMingZhi;
        public int nengLiangZhi;

        public static RuntimeShengHenInfo Build(EShengHen shengHen)
        {
            var info = GameInfoManager.shengHenTable[shengHen.typeId];
            int fangYuLi = info.fangYuLi;
            int shengMingZhi = info.shengMingZhi;
            int nengLiangZhi = info.nengLiangZhi;
            var level = GameInfoManager.GetShengHenLevel(shengHen.exp);
            fangYuLi += info.fangYuLi2 * level.count;
            shengMingZhi += info.shengMingZhi2 * level.count;
            nengLiangZhi += info.nengLiangZhi2 * level.count;

            return new RuntimeShengHenInfo
            {
                fangYuLi = fangYuLi,
                shengMingZhi = shengMingZhi,
                nengLiangZhi = nengLiangZhi
            };
        }
    }
}