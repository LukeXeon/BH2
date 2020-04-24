using Dash.Scripts.Cloud;
using Dash.Scripts.Config;

namespace Dash.Scripts.Levels.Config
{
    public class RuntimeWeaponInfo
    {
        public int gongJiLi;
        public float sheSu;

        public static RuntimeWeaponInfo Build(EWeapon weapon)
        {
            var info = GameConfigManager.weaponTable[weapon.typeId];
            var gongJiLi = info.gongJiLi;
            var sheSu = info.sheSu;
            var level = GameConfigManager.GetWeaponLevel(weapon.exp);
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