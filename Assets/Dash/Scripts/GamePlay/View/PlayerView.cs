using System.Linq;
using Dash.Scripts.Config;
using Dash.Scripts.GamePlay.Info;
using Photon.Pun;
using Spine.Unity;
using UnityEngine;

namespace Dash.Scripts.GamePlay.View
{
    public class PlayerView : MonoBehaviour, IPunObservable
    {
        public float speed;
        public PoseManager poseManager;
        public SkeletonMecanim mecanim;
        public Animator animator;
        private static readonly int IS_RUN = Animator.StringToHash("is_run");
        public PhotonView photonView;

        private int flipX = 1;

        private void Awake()
        {
            photonView = GetComponent<PhotonView>();
        }

        private void Start()
        {
            photonView.RPC(
                nameof(PreparePlayer),
                RpcTarget.All,
                GameplayInfoManager.playerInfo.Item1.typeId,
                GameplayInfoManager.weaponInfos.First().Item1.typeId
            );
        }
        
        [PunRPC]
        public void PreparePlayer(int playerTypeId, int weaponTypeId)
        {
            var info = GameGlobalInfoManager.playerTable[playerTypeId];
            mecanim.skeletonDataAsset = info.skel;
            mecanim.Initialize(true);
            poseManager.SetPose(GameGlobalInfoManager.weaponTable[weaponTypeId]);
            FindObjectOfType<GameplayManager>().PlayerComplete();
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

                //transform.position += move;
                GetComponent<Rigidbody>().MovePosition(transform.position += move);
            }


            mecanim.Skeleton.ScaleX = flipX;
        }

        public void OnLocalWeaponChanged(int typeId)
        {
            photonView.RPC(nameof(WeaponChanged), RpcTarget.All, typeId);
        }

        [PunRPC]
        public void WeaponChanged(int typeId)
        {
            var info = GameGlobalInfoManager.weaponTable[typeId];
            poseManager.SetPose(info);
            animator.SetTrigger("taoqiang");
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