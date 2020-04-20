using System;
using System.Linq;
using Dash.Scripts.Config;
using Dash.Scripts.Levels.Config;
using Photon.Pun;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;

namespace Dash.Scripts.Levels.View
{
    public class PlayerView : MonoBehaviour, IPunObservable
    {
        public float speed;
        public PoseManager poseManager;
        public SkeletonMecanim mecanim;
        public Animator animator;
        public OnPlayerLoadedEvent onPlayerLoadedEvent;
        private static readonly int IS_RUN = Animator.StringToHash("is_run");
        private static readonly int TAOQIANG = Animator.StringToHash("taoqiang");
        private static readonly int KAIQIANG = Animator.StringToHash("kaiqiang");
        private static readonly int RUN_KAIQIANG = Animator.StringToHash("run_kaiqiang");
        private PhotonView photonView;
        private new Rigidbody rigidbody;
        private WeaponInfoAsset weaponInfoAsset;

        private int flipX = 1;


        private void Awake()
        {
            if (onPlayerLoadedEvent == null)
            {
                onPlayerLoadedEvent = new OnPlayerLoadedEvent();
            }

            rigidbody = GetComponent<Rigidbody>();
            photonView = GetComponent<PhotonView>();
        }

        private void Start()
        {
            photonView.RPC(
                nameof(LoadPlayer),
                RpcTarget.All,
                InLevelConfigManager.playerInfo.Item1.typeId,
                InLevelConfigManager.weaponInfos.First().Item1.typeId
            );
        }

        [Serializable]
        public class OnPlayerLoadedEvent : UnityEvent
        {
        }

        [PunRPC]
        public void LoadPlayer(int playerTypeId, int weaponTypeId)
        {
            var info = GameConfigManager.playerTable[playerTypeId];
            mecanim.skeletonDataAsset = info.skel;
            mecanim.Initialize(true);
            weaponInfoAsset = GameConfigManager.weaponTable[weaponTypeId];
            poseManager.SetPose(weaponInfoAsset);
            onPlayerLoadedEvent.Invoke();
        }

        [PunRPC]
        public void WeaponChanged(int typeId)
        {
            weaponInfoAsset = GameConfigManager.weaponTable[typeId];
            poseManager.SetPose(weaponInfoAsset);
            animator.SetTrigger(TAOQIANG);
        }

        [PunRPC]
        public void KaiQiang()
        {
            animator.SetTrigger(KAIQIANG);
            
        }

        private void Update()
        {
            if (photonView.IsMine)
            {
                if (weaponInfoAsset.weaponType.canLianShe)
                {
                    if (ETCInput.GetButton("kaiqiang"))
                    {
                        photonView.RPC(nameof(KaiQiang), RpcTarget.All);
                    }
                }
                else
                {
                    var state = animator.GetCurrentAnimatorStateInfo(0);
                    if (!state.IsName("kaiqiang"))
                    {
                        state = animator.GetCurrentAnimatorStateInfo(1);
                        if (!state.IsName("run_kaiqiang"))
                        {
                            if (ETCInput.GetButtonDown("kaiqiang"))
                            {
                                photonView.RPC(nameof(KaiQiang), RpcTarget.All);
                            }
                        }
                    }
                }

                if (ETCInput.GetButtonDown("skill"))
                {
                }
            }
        }

        public void OnKaiQiang()
        {
            Debug.Log(nameof(OnKaiQiang));
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

        public void OnLocalWeaponChanged(int typeId)
        {
            photonView.RPC(nameof(WeaponChanged), RpcTarget.All, typeId);
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