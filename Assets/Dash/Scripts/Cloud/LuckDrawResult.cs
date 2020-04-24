namespace Dash.Scripts.Cloud
{
    public class LuckDrawResult
    {
        public enum Type
        {
            UnLockPlayer,
            AddPlayerExp,
            Weapon,
            ShengHen,
            ExpAssets
        }

        public Type resultType;
        public int typeId;
    }
}