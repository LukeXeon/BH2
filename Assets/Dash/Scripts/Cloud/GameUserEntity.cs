using Parse;

namespace Dash.Scripts.Cloud
{
    [ParseClassName("GameUser")]
    public class GameUserEntity : ParseObject
    {
        [ParseFieldName(nameof(user))]
        public ParseUser user
        {
            get => GetProperty<ParseUser>(nameof(user));
            set => SetProperty(value, nameof(user));
        }


        [ParseFieldName(nameof(exp))]
        public int exp
        {
            get => GetProperty<int>(nameof(exp));
            set => SetProperty(value, nameof(exp));
        }


        [ParseFieldName(nameof(player))]
        public PlayerEntity player
        {
            get => GetProperty<PlayerEntity>(nameof(player));
            set => SetProperty(value, nameof(player));
        }

        [ParseFieldName(nameof(gold))]
        public int gold
        {
            get => GetProperty<int>(nameof(gold));
            set => SetProperty(value, nameof(gold));
        }

        [ParseFieldName(nameof(crystal))]
        public int crystal
        {
            get => GetProperty<int>(nameof(crystal));
            set => SetProperty(value, nameof(crystal));
        }

        [ParseFieldName(nameof(name))]
        public string name
        {
            get => GetProperty<string>(nameof(name));
            set => SetProperty(value, nameof(name));
        }
    }
}