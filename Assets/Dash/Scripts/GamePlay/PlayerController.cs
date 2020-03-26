using System;
using UnityEngine;

namespace Dash.Scripts.GamePlay
{
    public class PlayerController : MonoBehaviour
    {
        public float speed = 0.01f;
        public PlayerSpineManager manager;

        private void Update()
        {
            var h = ETCInput.GetAxis("Horizontal");
            var v = ETCInput.GetAxis("Vertical");
            var move = new Vector3(Mathf.Abs(h) > 0 ? 1 * Mathf.Sign(h) : 0, 0,
                           Mathf.Abs(v) > 0 ? Mathf.Sign(v) : 0) * speed;
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

            transform.position += move;
        }
    }
}