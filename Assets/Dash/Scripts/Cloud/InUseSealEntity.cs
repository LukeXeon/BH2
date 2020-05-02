using Parse;

namespace Dash.Scripts.Cloud
{
    [ParseClassName("InUseSeal")]
    public class InUseSealEntity : ParseObject
    {
        [ParseFieldName(nameof(player))]
        public PlayerEntity player
        {
            get => GetProperty<PlayerEntity>(nameof(player));
            set => SetProperty(value, nameof(player));
        }

        [ParseFieldName(nameof(index))]
        public int index
        {
            get => GetProperty<int>(nameof(index));
            set => SetProperty(value, nameof(index));
        }
        
        [ParseFieldName(nameof(seal))]
        public SealEntity seal
        {
            get => GetProperty<SealEntity>(nameof(seal));
            set => SetProperty(value, nameof(seal));
        }

        [ParseFieldName(nameof(user))]
        public ParseUser user
        {
            get => GetProperty<ParseUser>(nameof(user));
            set => SetProperty(value, nameof(user));
        }
    }
}