using System;
using System.Collections.Generic;
using System.Reflection;
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
        public Transform bulletLocator;
        [Header("Event")] public OnPlayerLoadedEvent onPlayerLoadedEvent;
        private static readonly int IS_RUN = Animator.StringToHash("is_run");
        private static readonly int TAOQIANG = Animator.StringToHash("taoqiang");
        private static readonly int KAIQIANG = Animator.StringToHash("kaiqiang");
        private static readonly int LIANSHE = Animator.StringToHash("lianshe");

        private static readonly Dictionary<(Type, string), MethodInfo> methodInfos =
            new Dictionary<(Type, string), MethodInfo>();

        private new Rigidbody rigidbody;

        private int flipX = 1;

        //Weapon
        public WeaponInfoAsset weaponInfoAsset { get; private set; }
        private WeaponView weaponView;
        private float timeBetweenBullets;
        private float lastShoot;
        private static readonly int KAIQIANG_SPEED = Animator.StringToHash("kaiqiang_speed");
        private static readonly int HIT = Animator.StringToHash("hit");


        [Serializable]
        public class OnPlayerLoadedEvent : UnityEvent
        {
        }

        protected override void Awake()
        {
            base.Awake();
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
            OnWeaponChanged(weaponTypeId);
            onPlayerLoadedEvent.Invoke();
        }

        [PunRPC]
        public void OnWeaponChanged(int typeId)
        {
            if (weaponView)
            {
                Destroy(weaponView.gameObject);
            }

            weaponInfoAsset = GameConfigManager.weaponTable[typeId];
            poseManager.SetPose(weaponInfoAsset);
            animator.SetTrigger(TAOQIANG);
            animator.SetBool(LIANSHE, false);
            animator.ResetTrigger(KAIQIANG);
            animator.SetFloat(KAIQIANG_SPEED, poseManager.shootSpeed);
            var go = Instantiate(weaponInfoAsset.weaponView.gameObject, transform);
            weaponView = go.GetComponent<WeaponView>();
            weaponView.OnInitialize(this);
            lastShoot = 0;
            timeBetweenBullets = 1f / weaponInfoAsset.sheShu;
        }
        
        public void OnSingleFireAnimationCallback()
        {
            if (photonView.IsMine)
            {
                weaponView.OnFire();
            }
        }

        [PunRPC]
        public void OnSyncSingleFireAnimation()
        {
            animator.SetTrigger(KAIQIANG);
        }

        public override void OnDamage(int value)
        {
            base.OnDamage(value);
            animator.SetTrigger(HIT);
        }

        [PunRPC]
        public void OnChildRpc(string method, object[] args)
        {
            if (weaponView)
            {
                var type = weaponView.GetType();
                methodInfos.TryGetValue((type, method), out var methodInfo);
                if (methodInfo == null)
                {
                    methodInfo = type.GetMethod(method);
                    methodInfos[(type, method)] = methodInfo;
                }

                if (methodInfo != null)
                {
                    methodInfo.Invoke(weaponView, args);
                }
            }
        }

        private void Update()
        {
            if (photonView.IsMine)
            {
                var isKaiQiangPressed = ETCInput.GetButton("kaiqiang");
                //先更新动画，如果能够连射确保动画状态同步
                if (weaponInfoAsset.weaponType.canLianShe)
                {
                    animator.SetBool(LIANSHE, isKaiQiangPressed);
                }

                var time = Time.time;
                //如果上次射击时间间隔已过
                if (time - lastShoot >= timeBetweenBullets)
                {
                    //如果是连射的且按键按下，则发射一颗子弹
                    if (weaponInfoAsset.weaponType.canLianShe && isKaiQiangPressed)
                    {
                        weaponView.OnFire();
                        lastShoot = time;
                    }
                    //否则，该帧如果按下键才会发射单射武器，需要先同步动画
                    else if (ETCInput.GetButtonDown("kaiqiang"))
                    {
                        photonView.RPC(nameof(OnSyncSingleFireAnimation), RpcTarget.All);
                        lastShoot = time;
                    }
                }

                if (ETCInput.GetButtonDown("skill"))
                {
                }
            }

            var transform1 = weaponView.transform;
            var local = transform1.localScale;
            local.x = flipX;
            transform1.localScale = local;
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