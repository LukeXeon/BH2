using Photon.Pun;

namespace Dash.Scripts.Levels.Config
{
    public static class LocalPlayer
    {
        public static int weaponIndex;

        public static int hp
        {
            get => GetLocalPlayerValue(nameof(hp), 0);
            set => SetLocalPlayerValue(nameof(hp), value);
        }

        public static int mp
        {
            get => GetLocalPlayerValue(nameof(mp), 0);
            set => SetLocalPlayerValue(nameof(mp), value);
        }

        private static T GetLocalPlayerValue<T>(string name, T def)
        {
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(name, out var value);
            return value is T ? (T) value : def;
        }

        private static void SetLocalPlayerValue<T>(string name, T value)
        {
            PhotonNetwork.LocalPlayer.CustomProperties[name] = value;
        }
    }
}