using Dash.Scripts.Cloud;
using Dash.Scripts.Setting;

namespace Dash.Scripts.GamePlay.Setting
{
    public class RuntimeWeaponInfo
    {
        public int gongJiLi;

        public static RuntimeWeaponInfo Build(WeaponEntity weapon)
        {
            var info = GameSettingManager.weaponTable[weapon.typeId];
            var gongJiLi = info.gongJiLi;
            var sheSu = info.sheSu;
            var level = GameSettingManager.GetWeaponLevel(weapon.exp);
            gongJiLi += info.gongJiLi2 * level.count;
            return new RuntimeWeaponInfo
            {
                gongJiLi = gongJiLi
            };
        }
    }
}