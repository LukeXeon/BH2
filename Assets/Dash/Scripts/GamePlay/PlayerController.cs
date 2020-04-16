using System;
using Dash.Scripts.Config;
using Dash.Scripts.GamePlay.View;
using Photon.Pun;
using Spine.Unity;
using UnityEngine;

namespace Dash.Scripts.GamePlay
{
    public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
    {
        public float speed;
        public PoseManager poseManager;
        public SkeletonMecanim mecanim;
        public Animator animator;
        private static readonly int IS_RUN = Animator.StringToHash("is_run");
        [Header("Test")] public WeaponInfoAsset weapon;
        public bool inTest;

        private int flipX = 1;

        private void Awake()
        {
            FindObjectOfType<GameplayManager>().players.Add(this.gameObject);
            poseManager.SetPose(weapon);
        }

        private void OnDestroy()
        {
            FindObjectOfType<GameplayManager>()?.players?.Remove(this.gameObject);
        }

        private void FixedUpdate()
        {
            if (photonView.IsMine || Application.isEditor && inTest)
            {
                var h = ETCInput.GetAxis("Horizontal");
                var v = ETCInput.GetAxis("Vertical");
                var move = speed * Time.fixedDeltaTime * new Vector3(Mathf.Abs(h) > 0 ? 1 * Mathf.Sign(h) : 0, 0,
                               Mathf.Abs(v) > 0 ? Mathf.Sign(v) : 0);
                if (move != Vector3.zero)
                {
                    animator.SetBool(IS_RUN, true);
                }
                else
                {
                    animator.SetBool(IS_RUN, false);
                }

                if (h > 0)
                {
                    flipX = 1;
                }
                else if (h < 0)
                {
                    flipX = -1;
                }

                //transform.position += move;
                GetComponent<Rigidbody>().MovePosition(transform.position += move);
            }


            mecanim.Skeleton.ScaleX = flipX;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(flipX);
            }
            else
            {
                flipX = (int) stream.ReceiveNext();
            }
        }
    }
}