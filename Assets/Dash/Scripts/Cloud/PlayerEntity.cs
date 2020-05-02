using Parse;

namespace Dash.Scripts.Cloud
{
    [ParseClassName("Player")]
    public class PlayerEntity : ParseObject
    {
        [ParseFieldName(nameof(user))]
        public ParseUser user
        {
            get => GetProperty<ParseUser>(nameof(user));
            set => SetProperty(value, nameof(user));
        }

        [ParseFieldName(nameof(typeId))]
        public int typeId
        {
            get => GetProperty<int>(nameof(typeId));
            set => SetProperty(value, nameof(typeId));
        }

        [ParseFieldName(nameof(exp))]
        public int exp
        {
            get => GetProperty<int>(nameof(exp));
            set => SetProperty(value, nameof(exp));
        }
    }
}