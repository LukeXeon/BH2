using Dash.Scripts.Config;
using Dash.Scripts.Network.Cloud;

namespace Dash.Scripts.GamePlay.Info
{
    public class RuntimeWeaponInfo
    {
        public int gongJiLi;
        public float sheSu;

        public static RuntimeWeaponInfo Build(EWeapon weapon)
        {
            var info = GameInfoManager.weaponTable[weapon.typeId];
            int gongJiLi = info.gongJiLi;
            int sheSu = info.sheSu;
            var level = GameInfoManager.GetWeaponLevel(weapon.exp);
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