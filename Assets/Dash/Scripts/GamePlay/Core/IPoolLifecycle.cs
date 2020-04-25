namespace Dash.Scripts.GamePlay.Core
{
    public interface IPoolLifecycle
    {
        void Reusing();

        void Recycle();
    }
}