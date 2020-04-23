namespace Dash.Scripts.Levels.Core
{
    public interface IPoolLifecycle
    {
        void Reusing();

        void Recycle();
    }
}