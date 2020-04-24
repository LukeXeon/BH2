using Photon.Pun;
using UnityEngine;

namespace Dash.Scripts.Levels.View
{
    public abstract class WeaponView : MonoBehaviour
    {
        protected PlayerView playerView;

        public void OnInitialize(PlayerView view)
        {
            this.playerView = view;
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