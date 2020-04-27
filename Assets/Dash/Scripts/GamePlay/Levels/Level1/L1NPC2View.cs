using System;
using System.Collections.Generic;
using Dash.Scripts.Config;
using Dash.Scripts.GamePlay.View;
using Photon.Pun;
using Spine.Unity;
using UnityEngine;
using UnityEngine.AI;

namespace Dash.Scripts.GamePlay.Levels.Level1
{
    public class L1NPC2View : NpcView, IPunObservable
    {
        private static readonly int BOMB = Animator.StringToHash("bomb");
        private static readonly int IS_RUN = Animator.StringToHash("is_run");
        private static readonly int HIT = Animator.StringToHash("hit");
        public Animator animator;
        public NavMeshAgent agent;
        public SkeletonMecanim mecanim;
        public AudioSource bombSource;

        [Serializable]
        public struct NPC2Config
        {
        }

        public NPC2Config config1;
        [Header("Sync")] private int flipX = -1;

        //
        private HashSet<int> viewIds;
        private bool isBusy;
        private bool isBombed;
        private int targetLayer;

        protected override void Awake()
        {
            base.Awake();
            viewIds = new HashSet<int>();
            targetLayer = LayerMask.NameToLayer("Player");
        }

        private void UpdateMovement()
        {
            if (isBusy || isBombed)
            {
                agent.enabled = false;
            }
            else
            {
                if (photonView.IsMine)
                {
                    if (target == null)
                    {
                        RequestTargetView();
                    }

                    agent.enabled = true;
                    agent.speed = config.moveSpeed;
                    if (target != null)
                    {
                        var position = target.transform.position;
                        agent.SetDestination(position);
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
        }

        private void UpdateFlipX()
        {
            mecanim.Skeleton.ScaleX = flipX;
        }

        private void Update()
        {
            animator.speed = Mathf.Max(1 + 1 - (float) hp / config.shengMingZhi * 2, 1);
            UpdateMovement();
            UpdateFlipX();
        }

        public void OnResetBusy()
        {
            if (!isBombed)
            {
                isBusy = false;
            }
        }

        public void OnBombAnimationCallback()
        {
            if (photonView.IsMine)
            {
                foreach (var id in viewIds)
                {
                    var view = PhotonView.Find(id);
                    var actor = view.GetComponent<ActorView>();
                    if (actor)
                    {
                        view.RPC(nameof(actor.OnDamage), RpcTarget.All, photonView.ViewID, 1000);
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (photonView.IsMine && other.gameObject.layer == targetLayer)
            {
                var view = other.GetComponent<ActorView>();
                if (view)
                {
                    viewIds.Remove(view.photonView.ViewID);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (photonView.IsMine && other.gameObject.layer == targetLayer)
            {
                if (!isBombed)
                {
                    isBombed = true;
                    photonView.RPC(nameof(OnSyncBomb), RpcTarget.All);
                }

                var view = other.GetComponent<ActorView>();
                if (view)
                {
                    viewIds.Add(view.photonView.ViewID);
                }
            }
        }

        [PunRPC]
        public void OnSyncBomb()
        {
            bombSource.time = 0;
            bombSource.Play();
            isBusy = true;
            animator.SetTrigger(BOMB);
        }

        [PunRPC]
        public override void OnDamage(int viewId, int value)
        {
            if (isBombed)
            {
                return;
            }

            isBusy = true;
            animator.SetTrigger(HIT);

            var damage = Mathf.Max(0,
                value - GameConfigManager.GetDamageReduction(config.fangYuLi, config.shengMingZhi));
            hp -= damage;
            isBusy = true;
            if (hp <= 0)
            {
                isBombed = true;
                animator.SetTrigger(BOMB);
            }
            else
            {
                animator.SetTrigger(HIT);
            }

            onActorDamageEvent.Invoke(this, damage);
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

        public void OnDieAnimationCallback()
        {
            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}