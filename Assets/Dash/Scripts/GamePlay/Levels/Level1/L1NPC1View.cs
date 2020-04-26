using System;
using Dash.Scripts.GamePlay.View;
using Photon.Pun;
using Spine.Unity;
using UnityEngine;
using UnityEngine.AI;

namespace Dash.Scripts.GamePlay.Levels.Level1
{
    public class L1NPC1View : NpcView, IPunObservable
    {
        private static readonly int IS_RUN = Animator.StringToHash("is_run");
        private static readonly int HIT = Animator.StringToHash("hit");

        [Serializable]
        public struct L1N1Config
        {
            [Header("进入第一攻击范围")] public float distance1;
            [Header("进入第二攻击范围")] public float distance2;
            [Header("攻击间隔")] public float gongJiJianGe;
        }

        public L1N1Config config1;
        [Header("Com")] public NavMeshAgent agent;
        public Animator animator;
        public SkeletonMecanim mecanim;


        //
        private int flipX = -1;

        [Header("Sync")] private int lastTargetViewId = int.MinValue;

        private int lastAttackTime;

        //使能开关
        private bool inHit;

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(target == null ? int.MinValue : target.ViewID);
                stream.SendNext(flipX);
                stream.SendNext(lastAttackTime);
            }
            else
            {
                lastTargetViewId = (int) stream.ReceiveNext();
                flipX = (int) stream.ReceiveNext();
                lastAttackTime = (int) stream.ReceiveNext();
            }
        }

        private void UpdateMovement()
        {
            if (inHit)
            {
                agent.enabled = false;
            }
            else
            {
                if (photonView.IsMine)
                {
                    if (target == null && lastTargetViewId != int.MinValue)
                    {
                        target = PhotonView.Find(lastTargetViewId);
                    }
                    else
                    {
                        RequestTargetView();
                    }

                    agent.enabled = true;
                    agent.stoppingDistance = config1.distance1;
                    if (target != null)
                    {
                        lastTargetViewId = target.ViewID;
                        var position = target.transform.position;
                        agent.SetDestination(position);
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
            }

            mecanim.Skeleton.ScaleX = flipX;
        }

        private void Update()
        {
            UpdateMovement();
        }

        public void OnResetInHit()
        {
            inHit = false;
        }

        [PunRPC]
        public override void OnDamage(int viewId, int value)
        {
            var view = PhotonView.Find(viewId);
            if (view)
            {
                var from = view.transform.position;
                if (from.x > transform.position.x)
                {
                    flipX = 1;
                }
                else if (from.x < transform.position.x)
                {
                    flipX = -1;
                }
            }

            animator.SetTrigger(HIT);
            onActorDamageEvent.Invoke(this, value);
            inHit = true;

        }
    }
}