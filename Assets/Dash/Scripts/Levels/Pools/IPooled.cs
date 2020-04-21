namespace Dash.Scripts.Levels.Pools
{
    public interface IPooled
    {
        uint MaxPool { get; }
        uint PreAlloc { get; }
        void Reusing();
        void Recycle();
    }
}