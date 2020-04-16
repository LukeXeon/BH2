using Dash.Scripts.Cloud;
using Dash.Scripts.Config;

namespace Dash.Scripts.GamePlay.Info
{
    public class RuntimeWeaponInfo
    {
        public int gongJiLi;
        public float sheSu;

        public static RuntimeWeaponInfo Build(EWeapon weapon)
        {
            var info = GameGlobalInfoManager.weaponTable[weapon.typeId];
            int gongJiLi = info.gongJiLi;
            int sheSu = info.sheSu;
            var level = GameGlobalInfoManager.GetWeaponLevel(weapon.exp);
            gongJiLi += info.gongJiLi2 * level.count;
            sheSu += info.sheSu2 * level.count;
            return new RuntimeWeaponInfo
            {
                gongJiLi = gongJiLi,
                sheSu = sheSu
            };
        }
    }
}