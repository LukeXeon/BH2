using System.Collections;
using Dash.Scripts.GamePlay.Levels;
using Photon.Pun;
using UnityEngine;

namespace Dash.Scripts.GamePlay.View
{
    public class BulletView : MonoBehaviour, IPoolLifecycle
    {
        public PhotonView photonView;
        public new Rigidbody rigidbody;
        public new BoxCollider collider;
        private Coroutine coroutine;
        private int targetLayer;
        private int viewId;
        private int damage;

        public void RunTheBullet(int id, Vector3 speed, int layer, int damage)
        {
            viewId = id;
            this.damage = damage;
            rigidbody.AddForce(speed);
            targetLayer = layer;
            if (photonView.IsMine)
            {
                collider.enabled = true;
                coroutine = StartCoroutine(AutoDestroy());
            }
            else
            {
                collider.enabled = false;
            }
        }

        private IEnumerator AutoDestroy()
        {
            yield return new WaitForSeconds(3);
            PhotonNetwork.Destroy(gameObject);
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
                if (view)
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
    }
}