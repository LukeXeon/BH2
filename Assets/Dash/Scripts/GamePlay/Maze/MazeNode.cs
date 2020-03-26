using System;

namespace Dash.Scripts.GamePlay.Maze
{
    [Serializable]
    public class MazeNode
    {
        public bool leftC;
        public bool rightC;
        public bool bottomWall;
        public int leftTypeId;
        public int rightTypeId;
    }
}