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

        [ParseFieldName(nameof(shuiJing))]
        public int shuiJing
        {
            get => GetProperty<int>(nameof(shuiJing));
            set => SetProperty(value, nameof(shuiJing));
        }

        [ParseFieldName(nameof(nameInGame))]
        public string nameInGame
        {
            get => GetProperty<string>(nameof(nameInGame));
            set => SetProperty(value, nameof(nameInGame));
        }
    }
}