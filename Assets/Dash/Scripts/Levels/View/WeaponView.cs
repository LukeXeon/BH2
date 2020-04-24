using Photon.Pun;
using UnityEngine;

namespace Dash.Scripts.Levels.View
{
    public abstract class WeaponView : MonoBehaviour
    {
        protected PlayerView playerView;
        protected int targetMask;
        protected bool isMine => playerView.photonView.IsMine;
        private Animator cameraAnim;
        private static readonly int CAMERA_SHAKE_TRIGGER = Animator.StringToHash("CameraShakeTrigger");

        public void OnInitialize(PlayerView view)
        {
            playerView = view;
            if (playerView.gameObject.layer == LayerMask.NameToLayer("Player"))
                targetMask = LayerMask.GetMask("NPC");
            else
                targetMask = LayerMask.GetMask("Player");

            var cam = Camera.main;
            if (cam != null)
            {
                cameraAnim = cam.GetComponent<Animator>();
            }
        }

        public virtual void OnFire()
        {
            if (isMine && cameraAnim)
            {
                cameraAnim.SetTrigger(CAMERA_SHAKE_TRIGGER);
            }
        }

        public void RpcInPlayerView(string method, params object[] objects)
        {
            playerView.photonView.RPC(
                nameof(playerView.OnChildRpc),
                RpcTarget.All,
                method,
                objects
            );
        }
    }
}