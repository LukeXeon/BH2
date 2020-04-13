using LeanCloud;

namespace Dash.Scripts.Cloud
{
    [AVClassName("Weapon")]
    public class EWeapon : AVObject
    {
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


        [AVFieldName(nameof(user))]
        public AVUser user
        {
            get { return GetProperty<AVUser>(nameof(user)); }
            set { SetProperty(value, nameof(user)); }
        }


        [AVFieldName(nameof(player))]
        public EPlayer player
        {
            get { return GetProperty<EPlayer>(nameof(player)); }
            set { SetProperty(value, nameof(player)); }
        }
    }
}