using System.Collections.Generic;

namespace Dash.Scripts.Network.Cloud
{
    public class Equipments
    {
        
        public Dictionary<string, PlayerWithUsing> players = new Dictionary<string, PlayerWithUsing>();

        public Dictionary<string, EWeapon> weapons = new Dictionary<string, EWeapon>();

        public Dictionary<string, EShengHen> shengHens = new Dictionary<string, EShengHen>();
    }

    public class PlayerWithUsing
    {
        public EPlayer player;
        public List<EInUseWeapon> weapons = new List<EInUseWeapon>();
        public List<EInUseShengHen> shengHens = new List<EInUseShengHen>();
    }
}