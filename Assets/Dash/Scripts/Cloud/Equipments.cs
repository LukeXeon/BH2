using System.Collections.Generic;

namespace Dash.Scripts.Cloud
{
    public class Equipments
    {
        public Dictionary<string, PlayerWithUsing> players = new Dictionary<string, PlayerWithUsing>();

        public Dictionary<string, EShengHen> shengHens = new Dictionary<string, EShengHen>();

        public Dictionary<string, EWeapon> weapons = new Dictionary<string, EWeapon>();
    }

    public class PlayerWithUsing
    {
        public EPlayer player;
        public List<EInUseShengHen> shengHens = new List<EInUseShengHen>();
        public List<EInUseWeapon> weapons = new List<EInUseWeapon>();
    }
}