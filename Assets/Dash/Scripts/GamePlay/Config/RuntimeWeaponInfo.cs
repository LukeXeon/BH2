using Dash.Scripts.Cloud;
using Dash.Scripts.Config;

namespace Dash.Scripts.GamePlay.Config
{
    public class RuntimeWeaponInfo
    {
        public int gongJiLi;

        public static RuntimeWeaponInfo Build(WeaponEntity weapon)
        {
            var info = GameConfigManager.weaponTable[weapon.typeId];
            var gongJiLi = info.gongJiLi;
            var sheSu = info.sheSu;
            var level = GameConfigManager.GetWeaponLevel(weapon.exp);
            gongJiLi += info.gongJiLi2 * level.count;
            return new RuntimeWeaponInfo
            {
                gongJiLi = gongJiLi
            };
        }
    }
}