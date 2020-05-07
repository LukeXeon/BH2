using System.Collections;
using Dash.Scripts.Gameplay.Levels;
using Photon.Pun;
using UnityEngine;

namespace Dash.Scripts.Gameplay.View
{
    public class BulletView : MonoBehaviourPun, IPoolLifecycle, IPunInstantiateMagicCallback
    {
        public new Rigidbody rigidbody;
        public new BoxCollider collider;
        private Coroutine coroutine;
        private int targetLayer;
        private int viewId;
        private int damage;

        public void Initialize(int id, int layer, int damage)
        {
            viewId = id;
            this.damage = damage;
            targetLayer = layer;
            if (photonView.IsMine)
            {
                collider.enabled = true;

                IEnumerator AutoDestroy()
                {
                    yield return new WaitForSeconds(3);
                    PhotonNetwork.Destroy(gameObject);
                }

                coroutine = StartCoroutine(AutoDestroy());
            }
            else
            {
                collider.enabled = false;
            }
        }


        private void OnDestroy()
        {
            Recycle();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (photonView.IsMine && other.gameObject.layer == targetLayer)
            {
                var view = other.GetComponent<ActorView>();
                if (view && !view.isDie)
                {
                    view.photonView.RPC(nameof(view.OnDamage), RpcTarget.All, viewId, damage);
                }

                StopCoroutine(coroutine);
                PhotonNetwork.Destroy(gameObject);
            }
        }

        public void Reusing()
        {
            rigidbody.angularVelocity = Vector3.zero;
            rigidbody.velocity = Vector3.zero;
        }

        public void Recycle()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            rigidbody.AddForce((Vector3) info.photonView.InstantiationData[0]);
        }
    }
}