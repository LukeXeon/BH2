using LeanCloud;

namespace Dash.Scripts.Cloud
{
    [AVClassName("UserMate")]
    public class EUserMate : AVObject
    {
        [AVFieldName(nameof(user))]
        public AVUser user
        {
            get => GetProperty<AVUser>(nameof(user));
            set => SetProperty(value, nameof(user));
        }


        [AVFieldName(nameof(exp))]
        public int exp
        {
            get => GetProperty<int>(nameof(exp));
            set => SetProperty(value, nameof(exp));
        }


        [AVFieldName(nameof(player))]
        public EPlayer player
        {
            get => GetProperty<EPlayer>(nameof(player));
            set => SetProperty(value, nameof(player));
        }

        [AVFieldName(nameof(shuiJing))]
        public int shuiJing
        {
            get => GetProperty<int>(nameof(shuiJing));
            set => SetProperty(value, nameof(shuiJing));
        }

        [AVFieldName(nameof(tiLi))]
        public int tiLi
        {
            get => GetProperty<int>(nameof(tiLi));
            set => SetProperty(value, nameof(tiLi));
        }

        [AVFieldName(nameof(nameInGame))]
        public string nameInGame
        {
            get => GetProperty<string>(nameof(nameInGame));
            set => SetProperty(value, nameof(nameInGame));
        }
    }
}