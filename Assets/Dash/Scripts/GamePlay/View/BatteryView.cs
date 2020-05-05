using System.Collections;
using Dash.Scripts.Core;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace Dash.Scripts.GamePlay.View
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
        private int damage;

        public void StartWork(int damage, int time)
        {
            this.damage = damage;

            IEnumerator Wait()
            {
                var y = new WaitForSeconds(1);
                for (int i = time; i >= 1; i--)
                {
                    text.text = i.ToString();
                    yield return y;
                }

                photonView.RPC(nameof(SyncBatteryDestroy), RpcTarget.All);
                if (photonView.IsMine)
                {
                    PhotonNetwork.Destroy(gameObject);
                }
            }

            StartCoroutine(Wait());
        }

        private void Awake()
        {
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

            photonView = GetComponent<PhotonView>();
        }

        private void Start()
        {
            var transform1 = gunRoot;
            var local = transform1.localScale;
            local.x = (float) photonView.InstantiationData[0];
            transform1.localScale = local;
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

            if (photonView.IsMine)
            {
                var flipX = gunRoot.localScale.x;
                var go = PhotonNetwork.Instantiate(
                    bullet.guid,
                    bulletLocator.position,
                    bulletLocator.rotation,
                    data: new object[]
                    {
                        -flipX * Random.Range(2000, 3000) * Vector3.left
                    }
                );
                var bulletView = go.GetComponent<BulletView>();
                bulletView.Initialize(photonView.ViewID,
                    LayerMask.NameToLayer("NPC"),
                    damage);
            }
        }

        [PunRPC]
        public void SyncBatteryDestroy()
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