using Parse;

namespace Dash.Scripts.Cloud
{
    [ParseClassName("InUseWeapon")]
    public class InUseWeaponEntity : ParseObject
    {
        [ParseFieldName(nameof(player))]
        public PlayerEntity player
        {
            get => GetProperty<PlayerEntity>(nameof(player));
            set => SetProperty(value, nameof(player));
        }

        [ParseFieldName(nameof(index))]
        public int index
        {
            get => GetProperty<int>(nameof(index));
            set => SetProperty(value, nameof(index));
        }


        [ParseFieldName(nameof(weapon))]
        public WeaponEntity weapon
        {
            get => GetProperty<WeaponEntity>(nameof(weapon));
            set => SetProperty(value, nameof(weapon));
        }

        [ParseFieldName(nameof(user))]
        public ParseUser user
        {
            get => GetProperty<ParseUser>(nameof(user));
            set => SetProperty(value, nameof(user));
        }
    }
}