using LeanCloud;

namespace Dash.Scripts.Network.Cloud
{
    [AVClassName("Player")]
    public class EPlayer : AVObject
    {
        [AVFieldName(nameof(user))]
        public AVUser user
        {
            get { return GetProperty<AVUser>(nameof(user)); }
            set { SetProperty(value, nameof(user)); }
        }

        [AVFieldName(nameof(typeId))]
        public int typeId
        {
            get { return GetProperty<int>(nameof(typeId)); }
            set { SetProperty(value, nameof(typeId)); }
        }

        [AVFieldName(nameof(exp))]
        public int exp
        {
            get { return GetProperty<int>(nameof(exp)); }
            set { SetProperty(value, nameof(exp)); }
        }
    }
}