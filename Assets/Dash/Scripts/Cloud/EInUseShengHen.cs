using LeanCloud;

namespace Dash.Scripts.Cloud
{
    [AVClassName("InUseShengHen")]
    public class EInUseShengHen: AVObject
    {
        [AVFieldName(nameof(player))]
        public EPlayer player
        {
            get { return GetProperty<EPlayer>(nameof(player)); }
            set { SetProperty(value, nameof(player)); }
        }

        [AVFieldName(nameof(index))]
        public int index
        {
            get { return GetProperty<int>(nameof(index)); }
            set { SetProperty(value, nameof(index)); }
        }


        [AVFieldName(nameof(shengHen))]
        public EShengHen shengHen
        {
            get { return GetProperty<EShengHen>(nameof(shengHen)); }
            set { SetProperty(value, nameof(shengHen)); }
        }
        
        [AVFieldName(nameof(user))]
        public AVUser user
        {
            get { return GetProperty<AVUser>(nameof(user)); }
            set { SetProperty(value, nameof(user)); }
        }
    }
}