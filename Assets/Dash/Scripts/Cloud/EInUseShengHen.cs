using LeanCloud;

namespace Dash.Scripts.Cloud
{
    [AVClassName("InUseShengHen")]
    public class EInUseShengHen : AVObject
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


        [AVFieldName(nameof(shengHen))]
        public EShengHen shengHen
        {
            get => GetProperty<EShengHen>(nameof(shengHen));
            set => SetProperty(value, nameof(shengHen));
        }

        [AVFieldName(nameof(user))]
        public AVUser user
        {
            get => GetProperty<AVUser>(nameof(user));
            set => SetProperty(value, nameof(user));
        }
    }
}