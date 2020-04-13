using LeanCloud;

namespace Dash.Scripts.Cloud
{
    [AVClassName("InUseWeapon")]
    public class EInUseWeapon : AVObject
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


        [AVFieldName(nameof(weapon))]
        public EWeapon weapon
        {
            get { return GetProperty<EWeapon>(nameof(weapon)); }
            set { SetProperty(value, nameof(weapon)); }
        }
        
        [AVFieldName(nameof(user))]
        public AVUser user
        {
            get { return GetProperty<AVUser>(nameof(user)); }
            set { SetProperty(value, nameof(user)); }
        }
    }
}