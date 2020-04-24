using LeanCloud;

namespace Dash.Scripts.Cloud
{
    [AVClassName("ShengHen")]
    public class EShengHen : AVObject
    {
        [AVFieldName(nameof(typeId))]
        public int typeId
        {
            get => GetProperty<int>(nameof(typeId));
            set => SetProperty(value, nameof(typeId));
        }

        [AVFieldName(nameof(exp))]
        public int exp
        {
            get => GetProperty<int>(nameof(exp));
            set => SetProperty(value, nameof(exp));
        }


        [AVFieldName(nameof(user))]
        public AVUser user
        {
            get => GetProperty<AVUser>(nameof(user));
            set => SetProperty(value, nameof(user));
        }

        [AVFieldName(nameof(player))]
        public EPlayer player
        {
            get => GetProperty<EPlayer>(nameof(player));
            set => SetProperty(value, nameof(player));
        }
    }
}