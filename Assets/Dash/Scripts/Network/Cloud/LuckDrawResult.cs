namespace Dash.Scripts.Network.Cloud
{
    public class LuckDrawResult
    {
        public int typeId;
        public Type resultType;

        public enum Type
        {
            UnLockPlayer,
            AddPlayerExp,
            Weapon,
            ShengHen,
            ExpAssets
        }
    }
}