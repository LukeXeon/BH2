using LeanCloud;

namespace Dash.Scripts.Network.Cloud
{
    [AVClassName("UserMate")]
    public class EUserMate : AVObject
    {
        [AVFieldName(nameof(user))]
        public AVUser user
        {
            get { return GetProperty<AVUser>(nameof(user)); }
            set { SetProperty(value, nameof(user)); }
        }


        [AVFieldName(nameof(exp))]
        public int exp
        {
            get { return GetProperty<int>(nameof(exp)); }
            set { SetProperty(value, nameof(exp)); }
        }


        [AVFieldName(nameof(player))]
        public EPlayer player
        {
            get { return GetProperty<EPlayer>(nameof(player)); }
            set { SetProperty(value, nameof(player)); }
        }

        [AVFieldName(nameof(shuiJing))]
        public int shuiJing
        {
            get { return GetProperty<int>(nameof(shuiJing)); }
            set { SetProperty(value, nameof(shuiJing)); }
        }

        [AVFieldName(nameof(tiLi))]
        public int tiLi
        {
            get { return GetProperty<int>(nameof(tiLi)); }
            set { SetProperty(value, nameof(tiLi)); }
        }
    }
}