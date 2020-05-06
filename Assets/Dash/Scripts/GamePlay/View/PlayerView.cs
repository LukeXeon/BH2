using System.Collections.Generic;
using System.Linq;
using Dash.Scripts.Config;
using Dash.Scripts.Core;
using Dash.Scripts.GamePlay.Config;
using Photon.Pun;
using Spine.Unity;
using UnityEngine;

namespace Dash.Scripts.GamePlay.View
{
    public class PlayerView : ActorView, IPunObservable, IHostView
    {
        private static readonly int IS_RUN = Animator.StringToHash("is_run");
        private static readonly int TAOQIANG = Animator.StringToHash("taoqiang");
        private static readonly int SINGLE = Animator.StringToHash("kaiqiang_single");
        private static readonly int LIANSHE = Animator.StringToHash("kaiqiang_lianshe");
        private static readonly int IS_LIVE = Animator.StringToHash("is_live");
        private static readonly int KAIQIANG_SPEED = Animator.StringToHash("kaiqiang_speed");
        private static readonly int HIT = Animator.StringToHash("hit");


        public Animator animator;
        public Transform bulletLocator;

        public SkeletonMecanim mecanim;
        public PoseManager poseManager;
        public ActorEvent onPlayerRelive;
        private new Rigidbody rigidbody;
        public float speed;
        private WeaponView weaponView;
        private float timeBetweenBullets;
        private float lastShoot;
        public AudioView audioView;


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
            audioView = AudioView.Create(transform);
            if (onPlayerRelive == null)
            {
                onPlayerRelive = new ActorEvent();
            }

            weaponViews = new Dictionary<int, WeaponView>();
            rigidbody = GetComponent<Rigidbody>();
            photonView = GetComponent<PhotonView>();
            flipX = 1;
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
            animator.ResetTrigger(SINGLE);
            animator.SetFloat(KAIQIANG_SPEED, poseManager.shootSpeed);
            weaponView = weaponViews[typeId];
            weaponView.gameObject.SetActive(true);
            timeBetweenBullets = 1f / weaponInfoAsset.sheSu;
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
            animator.SetBool(SINGLE, true);
        }

        [PunRPC]
        public override void OnDamage(int viewId, int value)
        {
            if (isDie)
            {
                return;
            }

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

            if (photonView.IsMine)
            {
                var damage2 = Mathf.Max(0, value - GameConfigManager.GetDamageReduction(
                                               PlayerConfigManager.playerInfo.Item2.fangYuLi));
                LocalPlayer.hp -= damage2;
                if (LocalPlayer.hp <= 0)
                {
                    photonView.RPC(nameof(OnDie), RpcTarget.All);
                }

                photonView.RPC(nameof(OnSyncDamageText), RpcTarget.All, value);
            }
        }


        [PunRPC]
        private void OnDie()
        {
            isDie = true;
            animator.ResetTrigger(TAOQIANG);
            animator.ResetTrigger(SINGLE);
            animator.ResetTrigger(HIT);
            animator.SetBool(IS_RUN, false);
            animator.SetBool(LIANSHE, false);
            animator.SetBool(IS_LIVE, false);
            onActorDie.Invoke();
        }

        [PunRPC]
        private void OnSyncDamageText(int value)
        {
            onActorDamageEvent.Invoke(this, value);
        }

        public PhotonView PhotonView => photonView;

        [PunRPC]
        public void OnChildRpc(string method, object[] args)
        {
            if (weaponView)
            {
                this.HandleChildRpc(weaponView, method, args);
            }
        }

        private void Update()
        {
            if (!isDie && photonView.IsMine)
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
                if (isDie)
                {
                    rigidbody.velocity = Vector3.zero;
                }
                else
                {
                    var h = ETCInput.GetAxis("Horizontal");
                    var v = ETCInput.GetAxis("Vertical");
                    h = h == 0 ? 0 : Mathf.Sign(h);
                    v = v == 0 ? 0 : Mathf.Sign(v);
                    if (new Vector2(h, v) != Vector2.zero)
                    {
                        animator.SetBool(IS_RUN, true);
                        rigidbody.velocity = new Vector3(1 * h * speed, 0, 1 * v * speed);
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
            }

            mecanim.Skeleton.ScaleX = flipX;
            weaponView.SetFlipX(flipX);
        }
    }
}