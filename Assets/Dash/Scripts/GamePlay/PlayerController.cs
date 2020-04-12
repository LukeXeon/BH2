using UnityEngine;

namespace Dash.Scripts.GamePlay
{
    public class PlayerController : MonoBehaviour
    {
        public float speed;
        public PlayerSpineManager manager;
        public new Rigidbody rigidbody;

        private void FixedUpdate()
        {
            var h = ETCInput.GetAxis("Horizontal");
            var v = ETCInput.GetAxis("Vertical");
            var move = speed * Time.fixedDeltaTime * new Vector3(Mathf.Abs(h) > 0 ? 1 * Mathf.Sign(h) : 0, 0,
                           Mathf.Abs(v) > 0 ? Mathf.Sign(v) : 0);
            if (move != Vector3.zero)
            {
                manager.mainState = PlayerSpineManager.MainState.Run;
            }
            else
            {
                manager.mainState = PlayerSpineManager.MainState.Idle;
            }

            if (h > 0)
            {
                manager.flip = false;
            }
            else if (h < 0)
            {
                manager.flip = true;
            }
            rigidbody.MovePosition(rigidbody.position + move);
        }
    }
}