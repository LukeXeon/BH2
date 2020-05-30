using System;
using Dash.Scripts.Core;
using Dash.Scripts.Gameplay.Setting;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dash.Scripts.Gameplay.View
{
    public class RPGView : WeaponView
    {
        public GuidIndexer bullet;
        public Transform locator;
        private int npc;
        private int player;

        private void Awake()
        {
            npc  = LayerMask.NameToLayer("NPC");
            player = LayerMask.NameToLayer("Player");
        }

        protected override void OnFire()
        {
            
            base.OnFire();
            var go = PhotonNetwork.Instantiate(
                bullet.guid,
                locator.position,
                locator.rotation,
                data: new object[]
                {
                    -playerView.flipX * Random.Range(300, 500) * Vector3.left,
                    player
                }
            );
            var bulletView = go.GetComponent<BulletView>();
            bulletView.Initialize(
                playerView.photonView.ViewID,
                npc,
                LocalPlayer.gongJiLi
            );
        }
    }
}