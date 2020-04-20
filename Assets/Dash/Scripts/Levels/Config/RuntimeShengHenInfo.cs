using Dash.Scripts.Cloud;
using Dash.Scripts.Config;

namespace Dash.Scripts.Levels.Config
{
    
    public class RuntimeShengHenInfo
    {
        public int fangYuLi;
        public int shengMingZhi;
        public int nengLiangZhi;

        public static RuntimeShengHenInfo Build(EShengHen shengHen)
        {
            var info = GameConfigManager.shengHenTable[shengHen.typeId];
            int fangYuLi = info.fangYuLi;
            int shengMingZhi = info.shengMingZhi;
            int nengLiangZhi = info.nengLiangZhi;
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