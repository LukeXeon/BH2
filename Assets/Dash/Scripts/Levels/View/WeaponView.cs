using Dash.Scripts.Config;
using Photon.Pun;
using UnityEngine;

namespace Dash.Scripts.Levels.View
{
    public abstract class WeaponView : MonoBehaviour
    {
        protected PlayerView playerView;
        protected int targetMask;
        protected bool isMine => playerView.photonView.IsMine;
        public bool canFire => Time.time - lastShoot >= timeBetweenBullets;
        private Animator cameraAnim;
        private static readonly int CAMERA_SHAKE_TRIGGER = Animator.StringToHash("CameraShakeTrigger");
        private float timeBetweenBullets;
        private float lastShoot;

        public void OnInitialize(PlayerView view, WeaponInfoAsset weaponInfoAsset)
        {
            playerView = view;
            targetMask = LayerMask.GetMask("NPC");
            var cam = Camera.main;
            if (cam != null)
            {
                cameraAnim = cam.GetComponent<Animator>();
            }

            timeBetweenBullets = 1f / weaponInfoAsset.sheShu;
        }

        public virtual void OnFire()
        {
            if (isMine && cameraAnim)
            {
                lastShoot = Time.time;
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