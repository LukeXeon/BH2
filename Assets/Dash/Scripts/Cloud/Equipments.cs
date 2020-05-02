using System.Collections.Generic;

namespace Dash.Scripts.Cloud
{
    public class Equipments
    {
        public readonly Dictionary<string, Player> players = new Dictionary<string, Player>();

        public readonly Dictionary<string, SealEntity> seals = new Dictionary<string, SealEntity>();

        public readonly Dictionary<string, WeaponEntity> weapons = new Dictionary<string, WeaponEntity>();

        public class Player
        {
            public PlayerEntity player;
            public readonly List<InUseSealEntity> seals = new List<InUseSealEntity>();
            public readonly List<InUseWeaponEntity> weapons = new List<InUseWeaponEntity>();
        }
    }
}