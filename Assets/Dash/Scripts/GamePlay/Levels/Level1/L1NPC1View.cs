using System;
using System.Linq;
using Dash.Scripts.Config;
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
        private static readonly int DIE = Animator.StringToHash("die");

        [Serializable]
        public struct L1N1Config
        {
            [Header("近战攻击范围")] public float distance;
            [Header("远程攻击间隔")] public float remoteTime;
            [Header("近战攻击间隔")] public float closeTime;
            [Header("发射子弹的预制体")] public GuidIndexer bullet;
            [Header("子弹发射位置")] public Transform[] locators;
            public Transform locatorRoot;
            public Transform bulletRoot;
            public Transform closeRoot;
        }

        public L1N1Config config1;
        [Header("Com")] public NavMeshAgent agent;
        public AudioSource audioSourceRemote;
        public AudioSource audioSourceClose;
        public Animator animator;
        public SkeletonMecanim mecanim;
        public GameObject hitEffect;

        //

        private ParticleSystem[] particleSystems0;

        [Header("Sync")]

        //使能开关
        private bool isBusy;

        private float lastCloseAttackTime;
        private float lastRemoteAttackTime;


        protected override void Awake()
        {
            base.Awake();
            particleSystems0 = config1.locators.SelectMany(l => l.GetComponentsInChildren<ParticleSystem>(true))
                .ToArray();
            foreach (var system in particleSystems0)
            {
                system.Stop();
            }
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

        private void UpdateMovement()
        {
            if (isBusy || isDie)
            {
                agent.enabled = false;
            }
            else
            {
                if (photonView.IsMine)
                {
                    if (target == null || targetActor.isDie)
                    {
                        RequestTargetView();
                    }


                    agent.enabled = true;
                    agent.speed = config.moveSpeed;
                    if (target != null)
                    {
                        var position1 = target.transform.position;
                        var position = position1;
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

                    if (target && agent.velocity == Vector3.zero)
                    {
                        flipX = (int) Mathf.Sign(target.transform.position.x - transform.position.x);
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
            if (!isDie && !isBusy && photonView.IsMine)
            {
                if (target)
                {
                    if (PhotonNetwork.Time - lastCloseAttackTime >= config1.closeTime)
                    {
                        if (Physics.BoxCast(
                                config1.closeRoot.position,
                                new Vector3(1.5f, 1.5f, 1.5f),
                                Vector3.right * flipX,
                                out var hit,
                                Quaternion.identity,
                                config.findPlayerRange,
                                targetLayerMask
                            ) && hit.distance <= config1.distance)
                        {
                            lastCloseAttackTime = (float) PhotonNetwork.Time;
                            photonView.RPC(nameof(OnSyncFire1), RpcTarget.All);
                        }
                    }

                    if (PhotonNetwork.Time - lastRemoteAttackTime >= config1.remoteTime +
                        Random.Range(-config1.remoteTime / 4, config1.remoteTime / 4))
                    {
                        if (Physics.BoxCast(
                            config1.bulletRoot.position,
                            new Vector3(1.5f, 1.5f, 1.5f),
                            Vector3.right * flipX,
                            out var hit,
                            Quaternion.identity,
                            config.findPlayerRange,
                            targetLayerMask
                        ))
                        {
                            var transform1 = hit.collider.transform;
                            var targetPosition = transform1.position;
                            var locatorPosition1 = config1.bulletRoot.position;
                            var bb = (flipX == -1 && locatorPosition1.x - targetPosition.x > 0) ||
                                     (flipX == 1 && locatorPosition1.x - targetPosition.x < 0);
                            if (bb)
                            {
                                lastRemoteAttackTime = (float) PhotonNetwork.Time;
                                photonView.RPC(nameof(OnSyncFire0), RpcTarget.All);
                            }
                        }
                    }
                }
            }
        }

        private void UpdateFlipX()
        {
            mecanim.Skeleton.ScaleX = flipX;
            var scale = config1.locatorRoot.localScale;
            scale.x = flipX;
            config1.locatorRoot.localScale = scale;
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
            foreach (var system in particleSystems0)
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
                            photonView.ViewID,
                            -flipX * Random.Range(2000, 3000) * Vector3.left,
                            LayerMask.NameToLayer("Player"),
                            config.gongJiLi
                        );
                    }
                }
            }
        }

        public void OnFire1AnimationCallback()
        {
            audioSourceClose.time = 0;
            audioSourceClose.Play();
            if (Physics.BoxCast(
                    config1.closeRoot.position,
                    new Vector3(1.5f, 1.5f, 1.5f),
                    Vector3.right * flipX,
                    out var hit,
                    Quaternion.identity,
                    config.findPlayerRange,
                    targetLayerMask
                ) && hit.distance <= config1.distance)
            {
                var view = hit.collider.GetComponent<ActorView>();
                if (view)
                {
                    view.photonView.RPC(
                        nameof(view.OnDamage),
                        RpcTarget.All,
                        photonView.ViewID,
                        config.gongJiLi
                    );
                }
            }
        }

        public void OnResetIsBusy()
        {
            isBusy = false;
        }

        [PunRPC]
        public override void OnDamage(int viewId, int value)
        {
            if (isDie)
            {
                return;
            }

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

            var damage = Mathf.Max(0,
                value - GameConfigManager.GetDamageReduction(config.fangYuLi));
            hp -= damage;
            isBusy = true;
            if (hp <= 0)
            {
                isDie = true;
                animator.SetTrigger(DIE);
            }
            else
            {
                animator.SetTrigger(HIT);
            }

            onActorDamageEvent.Invoke(this, damage);
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