using System.Linq;
using Photon.Pun;
using Spine.Unity;
using UnityEngine;
using UnityEngine.AI;

namespace Dash.Scripts.GamePlay.LevelManager.Level1
{
    public class L1NPC1Controller : MonoBehaviour, IPunObservable
    {
        public PhotonView target;
        [Header("Com")] public PhotonView photonView;
        public NavMeshAgent agent;
        public Animator animator;
        public SkeletonMecanim mecanim;
        [Header("Config")] public float distance;
        public float suoDiBanJing;


        private int IS_RUN;
        private int playerLayerMask;
        private Collider[] targetCollider;

        [Header("Sync")] private int lastTargetViewId = int.MinValue;
        private int flipX = -1;

        private void Awake()
        {
            IS_RUN = Animator.StringToHash("is_run");
            playerLayerMask = 1 << LayerMask.NameToLayer("Player");
            targetCollider = new Collider[1];
        }

        private void Update()
        {
            if (photonView.IsMine)
            {
                if (target == null && lastTargetViewId != int.MinValue)
                {
                    target = PhotonView.Find(lastTargetViewId);
                }
                else
                {
                    Physics.OverlapSphereNonAlloc(
                        transform.position,
                        suoDiBanJing,
                        targetCollider,
                        playerLayerMask
                    );
                    var c = targetCollider.FirstOrDefault();
                    if (c != null)
                    {
                        target = c.GetComponent<PhotonView>();
                    }
                }

                agent.enabled = true;
                if (target != null)
                {
                    lastTargetViewId = target.ViewID;
                    var position = target.transform.position;
                    agent.destination = position;
                }
                else
                {
                    lastTargetViewId = int.MinValue;
                }

                animator.SetBool(IS_RUN, agent.velocity != Vector3.zero);
                if (agent.velocity.x > 0)
                {
                    flipX = 1;
                }
                else if (agent.velocity.x < 0)
                {
                    flipX = -1;
                }
            }
            else
            {
                agent.enabled = false;
            }

            mecanim.Skeleton.ScaleX = flipX;
        }


        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                if (target == null)
                {
                    stream.SendNext(int.MaxValue);
                }
                else
                {
                    stream.SendNext(target.ViewID);
                }

                stream.SendNext(flipX);
            }
            else
            {
                lastTargetViewId = (int) stream.ReceiveNext();
                flipX = (int) stream.ReceiveNext();
            }
        }
    }
}