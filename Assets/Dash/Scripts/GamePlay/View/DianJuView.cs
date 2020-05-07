using System;
using Dash.Scripts.GamePlay.Setting;
using Photon.Pun;
using UnityEngine;

namespace Dash.Scripts.GamePlay.View
{
    public class DianJuView : WeaponView
    {
        public Transform rayBegin;
        public AudioSource idle;
        public AudioSource kaiqiang;
        private static readonly int IS_LIANSHE = Animator.StringToHash("kaiqiang_lianshe");

        protected override void OnFire()
        {
            var from = rayBegin.position;
            var index = LocalPlayer.weaponIndex;
            var (info, _) = PlayerConfigManager.weaponInfos[index];
            var range = info.sheCheng;
            if (Physics.BoxCast(
                from,
                Vector3.one * 2f,
                Vector3.right * Mathf.Sign(transform.localScale.x),
                out var shootHit,
                Quaternion.identity,
                range,
                targetMask
            ))
            {
                var actorView = shootHit.collider.GetComponent<ActorView>();
                if (actorView && !actorView.isDie)
                {
                    var value = LocalPlayer.gongJiLi;
                    actorView.photonView.RPC(
                        nameof(actorView.OnDamage),
                        RpcTarget.All,
                        playerView.PhotonView.ViewID,
                        value
                    );
                }
            }
        }

        private void Update()
        {
            var animator = playerView.animator;
            var isFire = animator.GetBool(IS_LIANSHE);
            if (idle.isPlaying && isFire)
            {
                idle.Stop();
            }
            if (!idle.isPlaying && !isFire)
            {
                idle.Play();
            }
            if (kaiqiang.isPlaying && !isFire)
            {
                kaiqiang.Stop();
            }
            if (!kaiqiang.isPlaying && isFire)
            {
                kaiqiang.Play();
            }
        }
    }
}