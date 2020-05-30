using UnityEngine;

namespace Dash.Scripts.Setting
{
    public interface IBagItem
    {
        string DisplayName { get; }

        Sprite Image { get; }
        
        int TypeId { get; }
    }
}