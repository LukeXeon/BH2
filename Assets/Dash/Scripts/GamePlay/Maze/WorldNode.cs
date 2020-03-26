using UnityEngine;

namespace Dash.Scripts.GamePlay.Maze
{
    public class WorldNode : MonoBehaviour
    {
        public GameObject leftC;
        public GameObject rightC;
        public GameObject bottomWall;
        public GameObject leftWallSlot;
        public GameObject rightWallSlot;
        public GameObject topWallSlot;

        public void Install(bool leftCV, bool rightCV, bool bottomWallV, GameObject left, GameObject right,
            GameObject top)
        {
            this.leftC.SetActive(leftCV);
            this.rightC.SetActive(rightCV);
            this.bottomWall.SetActive(bottomWallV);
            left.transform.SetParent(leftWallSlot.transform, false);
            right.transform.SetParent(rightWallSlot.transform, false);
            top.transform.SetParent(top.transform, false);
        }
    }
}