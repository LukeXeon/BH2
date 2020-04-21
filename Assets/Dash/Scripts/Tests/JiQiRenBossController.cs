using UnityEngine;

namespace Dash.Scripts.Tests
{
    public class JiQiRenBossController : MonoBehaviour
    {
        public Animator animator;
        public Transform rootY;
        public Transform leftX;
        public Transform leftY;
        public Transform rightX;
        public Transform rightY;

        [Range(-180, 180)] public float rootYRange;
        [Range(-180, 180)] public float rootXRange;
        [Range(-180, 180)] public float leftYRange;
        [Range(-180, 180)] public float rightYRange;
        public bool rangeEnable;

        private void LateUpdate()
        {
            if (rangeEnable)
            {
                var r = rootY.localEulerAngles;
                r.y += rootYRange;
                rootY.localEulerAngles = r;

                r = leftX.localEulerAngles;
                r.x += rootXRange;
                leftX.localEulerAngles = r;
            
                r = rightX.localEulerAngles;
                r.x += rootXRange;
                rightX.localEulerAngles = r;
            
                r = leftY.localEulerAngles;
                r.y += leftYRange;
                leftY.localEulerAngles = r;
            
                r = rightY.localEulerAngles;
                r.y += rightYRange;
                rightY.localEulerAngles = r;
            }
        }
    }
}