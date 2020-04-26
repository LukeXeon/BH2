using System;
using System.Linq;
using Dash.Scripts.Core;
using Dash.Scripts.GamePlay.View;
using Photon.Pun;
using Spine.Unity;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Dash.Scripts.GamePlay.Levels.Level1
{
    public class L1NPC1View : NpcView, IPunObservable
    {
        private static readonly int ATTACK1 = Animator.StringToHash("attack1");
        private static readonly int IS_RUN = Animator.StringToHash("is_run");
        private static readonly int HIT = Animator.StringToHash("hit");
        private static readonly int ATTACK = Animator.StringToHash("attack");

        [Serializable]
        public struct L1N1Config
        {
            [Header("近战攻击范围")] public float distance;
            [Header("远程攻击间隔")] public float remoteTime;
            [Header("近战攻击间隔")] public float closeTime;
            [Header("发射子弹的预制体")] public GuidIndexer bullet;
            [Header("子弹发射位置")] public Transform[] locators;
            public Transform bulletLocatorRoot;
            public Transform center;
        }

        public L1N1Config config1;
        [Header("Com")] public NavMeshAgent agent;
        public AudioSource audioSourceRemote;
        public AudioSource audioSourceClose;
        public Animator animator;
        public SkeletonMecanim mecanim;

        //

        private ParticleSystem[] particleSystems;
        [Header("Sync")] private int flipX = -1;
        private int lastTargetViewId = int.MinValue;
        private float lastCloseAttackTime;
        private float lastRemoteAttackTime;
        private Collider[] colliders;

        //使能开关
        private bool isBusy;

        protected override void Awake()
        {
            base.Awake();
            particleSystems = config1.locators.SelectMany(l => l.GetComponentsInChildren<ParticleSystem>(true))
                .ToArray();
            foreach (var system in particleSystems)
            {
                system.Stop();
            }

            colliders = new Collider[1];
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(target == null ? int.MinValue : target.ViewID);
                stream.SendNext(flipX);
                stream.SendNext(lastRemoteAttackTime);
                stream.SendNext(lastCloseAttackTime);
            }
            else
            {
                lastTargetViewId = (int) stream.ReceiveNext();
                flipX = (int) stream.ReceiveNext();
                lastRemoteAttackTime = (float) stream.ReceiveNext();
                lastCloseAttackTime = (float) stream.ReceiveNext();
            }
        }

        private void UpdateMovement()
        {
            if (isBusy)
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
                    else if (target == null)
                    {
                        RequestTargetView();
                    }

                    agent.enabled = true;
                    agent.speed = config.moveSpeed;
                    if (target != null)
                    {
                        lastTargetViewId = target.ViewID;
                        var position = target.transform.position;
                        agent.stoppingDistance = 1;
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
        }

        private void UpdateAttack()
        {
            if (!isBusy && photonView.IsMine)
            {
                float distance = float.MaxValue;
                if (target)
                {
                    var hasHit = Physics.BoxCast(
                        config1.center.position,
                        Vector3.one,
                        Vector3.right * flipX,
                        out var hit,
                        Quaternion.identity,
                        config.findPlayerRange,
                        targetLayerMask
                    );
                    if (hasHit)
                    {
                        distance = hit.distance;
                    }

                    if (agent.velocity == Vector3.zero)
                    {
                        if (!hasHit)
                        {
                            hasHit = Physics.BoxCast(
                                config1.center.position,
                                Vector3.one,
                                Vector3.right * -flipX,
                                out hit,
                                Quaternion.identity,
                                config.findPlayerRange,
                                targetLayerMask
                            );
                            if (hasHit)
                            {
                                distance = hit.distance;
                                flipX = -flipX;
                            }
                            else
                            {
                                colliders[0] = null;
                                Physics.OverlapSphereNonAlloc(config1.center.position, config1.distance, colliders,
                                    targetLayerMask);
                                if (colliders[0] != null)
                                {
                                    hasHit = true;
                                    distance = Vector3.Distance(config1.center.position,
                                        colliders[0].transform.position);
                                    colliders[0] = null;
                                }
                            }
                        }
                    }


                    if (hasHit)
                    {
                        if (distance <= config1.distance &&
                            PhotonNetwork.Time - lastCloseAttackTime >= config1.closeTime)
                        {
                            lastCloseAttackTime = (float) PhotonNetwork.Time;
                            photonView.RPC(nameof(OnSyncFire1), RpcTarget.All);
                        }
                        else if (PhotonNetwork.Time - lastRemoteAttackTime >= config1.remoteTime)
                        {
                            lastRemoteAttackTime = (float) PhotonNetwork.Time;
                            photonView.RPC(nameof(OnSyncFire0), RpcTarget.All);
                        }
                    }
                }
            }
        }

        private void UpdateFlipX()
        {
            mecanim.Skeleton.ScaleX = flipX;
            var scale = config1.bulletLocatorRoot.localScale;
            scale.x = flipX;
            config1.bulletLocatorRoot.localScale = scale;
        }

        private void Update()
        {
            UpdateMovement();
            UpdateAttack();
            UpdateFlipX();
        }

        [PunRPC]
        private void OnSyncFire1()
        {
            isBusy = true;
            animator.SetTrigger(ATTACK1);
        }

        [PunRPC]
        private void OnSyncFire0()
        {
            isBusy = true;
            animator.SetTrigger(ATTACK);
        }

        public void OnFire0AnimationCallback()
        {
            audioSourceRemote.time = 0;
            audioSourceRemote.Play();
            foreach (var system in particleSystems)
            {
                system.Simulate(0);
                system.Play();
            }

            if (photonView.IsMine)
            {
                foreach (var config1Locator in config1.locators)
                {
                    var go = PhotonNetwork.InstantiateSceneObject(
                        config1.bullet.guid,
                        config1Locator.position,
                        config1Locator.rotation
                    );
                    if (go)
                    {
                        var view = go.GetComponent<BulletView>();
                        view.RunTheBullet(
                            this,
                            -flipX * 2000 * Vector3.left,
                            LayerMask.NameToLayer("Player")
                        );
                    }
                }
            }
        }

        public void OnFire1AnimationCallback()
        {
            audioSourceClose.time = 0;
            audioSourceClose.Play();
            if (photonView.IsMine)
            {
                colliders[0] = null;
                Physics.OverlapSphereNonAlloc(config1.center.position, config1.distance, colliders, targetLayerMask);
                if (colliders[0] != null)
                {
                    var view = colliders[0].GetComponent<ActorView>();
                    if (view)
                    {
                        view.photonView.RPC(
                            nameof(view.OnDamage),
                            RpcTarget.All,
                            photonView.ViewID,
                            1000
                        );
                    }
                }

                colliders[0] = null;
            }
        }

        public void OnResetIsBusy()
        {
            isBusy = false;
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
            isBusy = true;
        }
    }
}