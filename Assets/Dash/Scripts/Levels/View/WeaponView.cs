using Photon.Pun;
using UnityEngine;

namespace Dash.Scripts.Levels.View
{
    public abstract class WeaponView : MonoBehaviour
    {
        protected PlayerView playerView;
        protected int targetMask;
        protected bool isMine => playerView.photonView.IsMine;

        public void OnInitialize(PlayerView view)
        {
            this.playerView = view;
            if (playerView.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                targetMask = LayerMask.GetMask("NPC");
            }
            else
            {
                targetMask = LayerMask.GetMask("Player");
            }
        }

        public abstract void OnFire();

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