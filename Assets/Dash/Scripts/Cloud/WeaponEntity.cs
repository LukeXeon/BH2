using Parse;

namespace Dash.Scripts.Cloud
{
    [ParseClassName("Weapon")]
    public class WeaponEntity : ParseObject
    {
        [ParseFieldName(nameof(typeId))]
        public int typeId
        {
            get => GetProperty<int>(nameof(typeId));
            set => SetProperty(value, nameof(typeId));
        }

        [ParseFieldName(nameof(exp))]
        public int exp
        {
            get => GetProperty<int>(nameof(exp));
            set => SetProperty(value, nameof(exp));
        }


        [ParseFieldName(nameof(user))]
        public ParseUser user
        {
            get => GetProperty<ParseUser>(nameof(user));
            set => SetProperty(value, nameof(user));
        }


        [ParseFieldName(nameof(player))]
        public PlayerEntity player
        {
            get => GetProperty<PlayerEntity>(nameof(player));
            set => SetProperty(value, nameof(player));
        }
    }
}