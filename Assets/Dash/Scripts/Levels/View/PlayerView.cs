using System;
using Dash.Scripts.Config;
using Photon.Pun;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;

namespace Dash.Scripts.Levels.View
{
    public class PlayerView : ActorView, IPunObservable
    {
        public float speed;
        public PoseManager poseManager;
        public SkeletonMecanim mecanim;
        public Animator animator;
        public PhotonView photonView;
        public Transform bulletLocator;
        [Header("Event")] public OnPlayerLoadedEvent onPlayerLoadedEvent;
        private static readonly int IS_RUN = Animator.StringToHash("is_run");
        private static readonly int TAOQIANG = Animator.StringToHash("taoqiang");
        private static readonly int IS_KAIQIANG = Animator.StringToHash("is_kaiqiang");

        private new Rigidbody rigidbody;
        private WeaponInfoAsset weaponInfoAsset;
        private WeaponView weaponView;
        private int flipX = 1;


        [Serializable]
        public class OnPlayerLoadedEvent : UnityEvent
        {
        }

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            photonView = GetComponent<PhotonView>();
            if (onPlayerLoadedEvent == null)
            {
                onPlayerLoadedEvent = new OnPlayerLoadedEvent();
            }
        }

        private void Start()
        {
            int playerTypeId = (int) photonView.InstantiationData[0];
            int weaponTypeId = (int) photonView.InstantiationData[1];
            var info = GameConfigManager.playerTable[playerTypeId];
            mecanim.skeletonDataAsset = info.skel;
            mecanim.Initialize(true);
            WeaponChanged(weaponTypeId);
            onPlayerLoadedEvent.Invoke();
        }

        [PunRPC]
        public void WeaponChanged(int typeId)
        {
//            if (weaponView != null)
//            {
//                Destroy(weaponView.gameObject);
//            }

            weaponInfoAsset = GameConfigManager.weaponTable[typeId];
            poseManager.SetPose(weaponInfoAsset);
            animator.SetTrigger(TAOQIANG);
            
//            weaponView = Instantiate(
//                weaponInfoAsset.viewManager.gameObject,
//                Vector3.zero,
//                Quaternion.identity,
//                transform
//            ).GetComponent<WeaponView>();
        }
        
        private void Update()
        {
            if (photonView.IsMine)
            {
                if (weaponInfoAsset.weaponType.canLianShe)
                {
                    if (ETCInput.GetButton("kaiqiang"))
                    {
                        animator.SetBool(IS_KAIQIANG,true);
                    }
                    else
                    {
                        animator.SetBool(IS_KAIQIANG,false);
                    }
                }
                else
                {
                    if (ETCInput.GetButtonDown("kaiqiang"))
                    {
                        animator.SetBool(IS_KAIQIANG,true);
                    }
                    else
                    {
                        animator.SetBool(IS_KAIQIANG,false);
                    }
                }

                if (ETCInput.GetButtonDown("skill"))
                {
                }
            }
        }

        public void OnWeaponFireBullet()
        {
//            weaponView.FireBullet();
        }

        private void FixedUpdate()
        {
            if (photonView.IsMine)
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

                rigidbody.MovePosition(rigidbody.position += move);
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