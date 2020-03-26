using UnityEngine;

namespace Dash.Scripts.GamePlay.Maze
{
    
    [CreateAssetMenu(fileName = "MazeWallInfoAsset",menuName = "Info/MazeWall")]
    public class MazeWallInfoAsset : ScriptableObject
    {
        public GameObject prefab;
        public int typeId;
    }
}