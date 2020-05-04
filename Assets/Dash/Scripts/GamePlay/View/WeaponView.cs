using Photon.Pun;
using UnityEngine;

namespace Dash.Scripts.GamePlay.View
{
    public abstract class WeaponView : MonoBehaviour
    {
        protected PlayerView playerView;
        protected int targetMask;
        protected bool isMine => playerView.PhotonView.IsMine;
        private Animator cameraAnim;
        private static readonly int CAMERA_SHAKE_TRIGGER = Animator.StringToHash("CameraShakeTrigger");

        public virtual void SetFlipX(int x)
        {
            var transform1 = transform;
            var local = transform1.localScale;
            local.x = x;
            transform1.localScale = local;
        }

        public void OnInitialize(PlayerView view)
        {
            playerView = view;
            targetMask = LayerMask.GetMask("NPC");
            var cam = Camera.main;
            if (view.PhotonView.IsMine && cam != null)
            {
                cameraAnim = cam.GetComponent<Animator>();
            }
        }
        
        public void Fire()
        {
            if (isMine)
            {
                OnFire();
            }
            else
            {
                Debug.LogError("客户端逻辑有错误");
            }
        }

        protected virtual void OnFire()
        {
            if (cameraAnim)
            {
                cameraAnim.SetTrigger(CAMERA_SHAKE_TRIGGER);
            }
        }
        
        public void RpcInHost(string method, params object[] objects)
        {
            playerView.PhotonView.RPC(
                nameof(playerView.OnChildRpc),
                RpcTarget.All,
                method,
                objects
            );
        }
    }
}