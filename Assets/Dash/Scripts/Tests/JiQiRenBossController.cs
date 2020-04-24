using UnityEngine;

namespace Dash.Scripts.Tests
{
    public class JiQiRenBossController : MonoBehaviour
    {
        public Animator animator;
        public Transform leftX;
        public Transform leftY;
        [Range(-180, 180)] public float leftYRange;
        public bool rangeEnable;
        public Transform rightX;
        public Transform rightY;
        [Range(-180, 180)] public float rightYRange;
        [Range(-180, 180)] public float rootXRange;
        public Transform rootY;

        [Range(-180, 180)] public float rootYRange;

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