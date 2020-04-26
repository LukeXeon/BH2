using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dash.Scripts.Config;
using Photon.Pun;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;

namespace Dash.Scripts.GamePlay.View
{
    public class PlayerView : ActorView, IPunObservable
    {
        private static readonly int IS_RUN = Animator.StringToHash("is_run");
        private static readonly int TAOQIANG = Animator.StringToHash("taoqiang");
        private static readonly int KAIQIANG = Animator.StringToHash("kaiqiang");
        private static readonly int LIANSHE = Animator.StringToHash("lianshe");

        private static readonly Dictionary<(Type, string), MethodInfo> methodInfos =
            new Dictionary<(Type, string), MethodInfo>();

        private static readonly int KAIQIANG_SPEED = Animator.StringToHash("kaiqiang_speed");
        private static readonly int HIT = Animator.StringToHash("hit");
        public Animator animator;
        public Transform bulletLocator;

        private int flipX = 1;
        public SkeletonMecanim mecanim;
        public PoseManager poseManager;
        [HideInInspector] public AudioView audioView;
        private new Rigidbody rigidbody;
        public float speed;
        private WeaponView weaponView;
        private float timeBetweenBullets;
        private float lastShoot;

        [Serializable]
        public class OnPlayerLoadedEvent : UnityEvent
        {
        }

        //Weapon
        private WeaponInfoAsset weaponInfoAsset;
        private Dictionary<int, WeaponView> weaponViews;

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
                stream.SendNext(flipX);
            else
                flipX = (int) stream.ReceiveNext();
        }

        protected override void Awake()
        {
            base.Awake();
            weaponViews = new Dictionary<int, WeaponView>();
            rigidbody = GetComponent<Rigidbody>();
            photonView = GetComponent<PhotonView>();

            audioView = AudioView.Create(this.transform);
        }

        private void Start()
        {
            var playerTypeId = (int) photonView.InstantiationData[0];
            var weaponTypeIds = (int[]) photonView.InstantiationData[1];
            var info = GameConfigManager.playerTable[playerTypeId];
            mecanim.skeletonDataAsset = info.skel;
            mecanim.Initialize(true);
            foreach (var weaponTypeId in weaponTypeIds)
            {
                weaponInfoAsset = GameConfigManager.weaponTable[weaponTypeId];
                var go = Instantiate(weaponInfoAsset.weaponView, transform);
                go.SetActive(false);
                weaponView = go.GetComponent<WeaponView>();
                weaponView.OnInitialize(this);
                weaponViews.Add(weaponTypeId, weaponView);
            }

            OnWeaponChanged(weaponTypeIds.First());
        }

        [PunRPC]
        public void OnWeaponChanged(int typeId)
        {
            if (weaponView)
            {
                weaponView.gameObject.SetActive(false);
            }

            weaponInfoAsset = GameConfigManager.weaponTable[typeId];
            poseManager.SetPose(weaponInfoAsset);
            animator.SetTrigger(TAOQIANG);
            animator.SetBool(LIANSHE, false);
            animator.ResetTrigger(KAIQIANG);
            animator.SetFloat(KAIQIANG_SPEED, poseManager.shootSpeed);
            weaponView = weaponViews[typeId];
            weaponView.gameObject.SetActive(true);
            timeBetweenBullets = 1f / weaponInfoAsset.sheShu;
        }

        public void OnSingleFireAnimationCallback()
        {
            if (photonView.IsMine)
            {
                weaponView.Fire();
            }
        }

        [PunRPC]
        private void OnFireSingle()
        {
            animator.SetTrigger(KAIQIANG);
        }

        [PunRPC]
        public override void OnDamage(int viewId, int value)
        {
            animator.SetTrigger(HIT);
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

            onActorDamageEvent.Invoke(this, value);
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
                if (time - lastShoot >= timeBetweenBullets)
                {
                    //如果是连射的且按键按下，则发射一颗子弹
                    if (weaponInfoAsset.weaponType.canLianShe && isKaiQiangPressed)
                    {
                        weaponView.Fire();
                        lastShoot = time;
                    }
                    //否则，该帧如果按下键才会发射单射武器，需要先同步动画
                    else if (ETCInput.GetButtonDown("kaiqiang"))
                    {
                        photonView.RPC(nameof(OnFireSingle), RpcTarget.All);
                        lastShoot = time;
                    }
                }

                if (ETCInput.GetButtonDown("skill"))
                {
                }
            }
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
                    rigidbody.MovePosition(rigidbody.position += move);
                }
                else
                {
                    animator.SetBool(IS_RUN, false);
                    rigidbody.velocity = Vector3.zero;
                }

                if (h > 0)
                {
                    flipX = 1;
                }
                else if (h < 0)
                {
                    flipX = -1;
                }
            }

            mecanim.Skeleton.ScaleX = flipX;
            weaponView.SetFlipX(flipX);
        }
    }
}