﻿using System.Collections;
using Dash.Scripts.Core;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace Dash.Scripts.Gameplay.View
{
    public class BatteryView : MonoBehaviour
    {
        private PhotonView photonView;
        private ParticleSystem[] particleDes;
        private ParticleSystem[] particleFire;
        public Transform bulletLocator;
        public Transform bombRoot;
        public Transform gunRoot;
        public GuidIndexer bullet;
        public TextMeshPro text;
        public Transform fireRoot;
        public AudioClip clip;
        private int damage;
        private AudioView audioView;
        private int npc;
        private int player;
        

        public void Initialize(int damage)
        {
            this.damage = damage;
        }

        private void Awake()
        {
            audioView = AudioView.Create(transform);
            particleDes = bombRoot.GetComponentsInChildren<ParticleSystem>(true);
            particleFire = fireRoot.GetComponentsInChildren<ParticleSystem>(true);
            foreach (var particleSystemsDe in particleDes)
            {
                particleSystemsDe.Stop();
            }

            foreach (var system in particleFire)
            {
                system.Stop();
            }
            npc  = LayerMask.NameToLayer("NPC");
            player = LayerMask.NameToLayer("Player");

            photonView = GetComponent<PhotonView>();
        }

        private void Start()
        {
            var transform1 = gunRoot;
            var local = transform1.localScale;
            local.x = (float) photonView.InstantiationData[0];
            transform1.localScale = local;
            var time = (int) photonView.InstantiationData[1];

            IEnumerator Wait()
            {
                var y = new WaitForSeconds(1);
                for (int i = time; i >= 1; i--)
                {
                    text.text = i.ToString();
                    yield return y;
                }

                photonView.RPC(nameof(DestroyEffect), RpcTarget.All);
                if (photonView.IsMine)
                {
                    PhotonNetwork.Destroy(gameObject);
                }
            }

            StartCoroutine(Wait());
        }
        
        public void OnFire()
        {
            photonView.RPC(nameof(BatteryFire), RpcTarget.All);
        }

        [PunRPC]
        public void BatteryFire()
        {
            foreach (var system in particleFire)
            {
                system.Simulate(0);
                system.Play();
            }
            var source = audioView.GetOrCreateSource();
            source.clip = clip;
            source.time = 0;
            source.Play();
            if (photonView.IsMine)
            {
                var flipX = gunRoot.localScale.x;
                var go = PhotonNetwork.Instantiate(
                    bullet.guid,
                    bulletLocator.position,
                    bulletLocator.rotation,
                    data: new object[]
                    {
                        -flipX * Random.Range(2000, 3000) * Vector3.left,
                        player
                    }
                );
                var bulletView = go.GetComponent<BulletView>();
                bulletView.Initialize(
                    photonView.ViewID,
                    npc,
                    damage
                );
            }
        }

        [PunRPC]
        public void DestroyEffect()
        {
            bombRoot.SetParent(null);
            bombRoot.gameObject.SetActive(true);
            foreach (var particleSystemsDe in particleDes)
            {
                particleSystemsDe.Simulate(0);
                particleSystemsDe.Play();
            }
            
            Destroy(bombRoot.gameObject, 2);
        }
    }
}