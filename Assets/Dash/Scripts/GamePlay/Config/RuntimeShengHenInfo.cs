using Dash.Scripts.Cloud;
using Dash.Scripts.Config;

namespace Dash.Scripts.GamePlay.Config
{
    public class RuntimeShengHenInfo
    {
        public int fangYuLi;
        public int nengLiangZhi;
        public int shengMingZhi;

        public static RuntimeShengHenInfo Build(SealEntity shengHen)
        {
            var info = GameConfigManager.SealsTable[shengHen.typeId];
            var fangYuLi = info.fangYuLi;
            var shengMingZhi = info.shengMingZhi;
            var nengLiangZhi = info.nengLiangZhi;
            var level = GameConfigManager.GetShengHenLevel(shengHen.exp);
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