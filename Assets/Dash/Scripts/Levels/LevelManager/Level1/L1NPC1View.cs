using System;
using System.Linq;
using Dash.Scripts.Levels.View;
using Photon.Pun;
using Spine.Unity;
using UnityEngine;
using UnityEngine.AI;

namespace Dash.Scripts.Levels.LevelManager.Level1
{
    public class L1NPC1View : NpcView, IPunObservable
    {
        private static readonly int HIT = Animator.StringToHash("hit");
        [Header("Com")] public NavMeshAgent agent;
        public Animator animator;
        public L1NPC1Config config;
        [Header("Config")] public float distance;
        private int flipX = -1;

        private int IS_RUN;

        [Header("Sync")] private int lastTargetViewId = int.MinValue;
        public SkeletonMecanim mecanim;
        private int playerLayerMask;
        public PhotonView target;
        private Collider[] targetCollider;

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                if (target == null)
                    stream.SendNext(int.MaxValue);
                else
                    stream.SendNext(target.ViewID);

                stream.SendNext(flipX);
            }
            else
            {
                lastTargetViewId = (int) stream.ReceiveNext();
                flipX = (int) stream.ReceiveNext();
            }
        }

        protected override void Awake()
        {
            base.Awake();
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
                        config.suoDiBanJing,
                        targetCollider,
                        playerLayerMask
                    );
                    var c = targetCollider.FirstOrDefault();
                    if (c != null) target = c.GetComponent<PhotonView>();
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
                    flipX = 1;
                else if (agent.velocity.x < 0) flipX = -1;
            }
            else
            {
                agent.enabled = false;
            }

            mecanim.Skeleton.ScaleX = flipX;
        }

        [PunRPC]
        public override void OnDamage(int value)
        {
            animator.SetTrigger(HIT);
            onActorDamageEvent.Invoke(transform, value);
        }

        [Serializable]
        public struct L1NPC1Config
        {
            public float suoDiBanJing;
            public int gongJiLi1;
            public int gongJiLi2;
            public int fangYuLi;
            public int yiDongSuDu;
        }
    }
}