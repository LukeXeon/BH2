using LeanCloud;

namespace Dash.Scripts.Cloud
{
    [AVClassName("InUseWeapon")]
    public class EInUseWeapon : AVObject
    {
        [AVFieldName(nameof(player))]
        public EPlayer player
        {
            get => GetProperty<EPlayer>(nameof(player));
            set => SetProperty(value, nameof(player));
        }

        [AVFieldName(nameof(index))]
        public int index
        {
            get => GetProperty<int>(nameof(index));
            set => SetProperty(value, nameof(index));
        }


        [AVFieldName(nameof(weapon))]
        public EWeapon weapon
        {
            get => GetProperty<EWeapon>(nameof(weapon));
            set => SetProperty(value, nameof(weapon));
        }

        [AVFieldName(nameof(user))]
        public AVUser user
        {
            get => GetProperty<AVUser>(nameof(user));
            set => SetProperty(value, nameof(user));
        }
    }
}