using System;
using System.Linq;
using Dash.Scripts.Config;
using Dash.Scripts.Core;
using Dash.Scripts.GamePlay.Config;
using Dash.Scripts.GamePlay.Levels;
using Dash.Scripts.GamePlay.View;
using Michsky.UI.ModernUIPack;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Dash.Scripts.GamePlay.UIManager
{
    public class LevelUIManager : MonoBehaviourPunCallbacks
    {
        private const float qieQiangJianGe = 0.3f;
        public Button back;
        public GuidIndexer damageText;
        public ModalWindowManager dialog;
        public Image icon;
        public TextMeshProUGUI lanText;
        public Image lanTiao;
        private float lastQieQiang;
        public Button leftWeapon;
        public GameObject mask;
        public TextMeshProUGUI playerName;
        public Button rightWeapon;
        public Button submitBack;
        public Image weapon;
        public OnWeaponChangedEvent weaponChanged;
        public TextMeshProUGUI xueText;
        public Image xueTiao;
        public Button cancelBack;
        public ETCJoystick joystick;
        public ETCButton fire0;
        public ETCButton fire1;
        public RectTransform damageTextRoot;
        private MonoBehaviour[] uiGroup;
        private bool isLockUI;

        private void Awake()
        {
            uiGroup = new MonoBehaviour[] {joystick, fire0, fire1, leftWeapon, rightWeapon};
            var player = PlayerConfigManager.playerInfo.Item1;
            icon.sprite = player.icon;
            playerName.text = player.displayName;
            weapon.sprite = PlayerConfigManager.weaponInfos.First().Item1.sprite;
            if (weaponChanged == null) weaponChanged = new OnWeaponChangedEvent();

            leftWeapon.onClick.AddListener(() =>
            {
                var time = Time.time;
                if (time - lastQieQiang < qieQiangJianGe) return;

                var last = LocalPlayer.weaponIndex - 1;
                if (last < 0) last = PlayerConfigManager.weaponInfos.Count - 1;

                Debug.Log(last);
                if (last == LocalPlayer.weaponIndex) return;

                var info = PlayerConfigManager.weaponInfos[last].Item1;
                LocalPlayer.weaponIndex = last;
                weaponChanged.Invoke(info);
                lastQieQiang = time;
            });
            rightWeapon.onClick.AddListener(() =>
            {
                var time = Time.time;
                if (time - lastQieQiang < qieQiangJianGe) return;

                var last = LocalPlayer.weaponIndex;
                LocalPlayer.weaponIndex = (last + 1) % PlayerConfigManager.weaponInfos.Count;
                if (last == LocalPlayer.weaponIndex) return;

                var info = PlayerConfigManager.weaponInfos[LocalPlayer.weaponIndex].Item1;
                weaponChanged.Invoke(info);
                lastQieQiang = time;
            });
            weaponChanged.AddListener(info => { weapon.sprite = info.sprite; });
            back.onClick.AddListener(() =>
            {
                LockUI(true);
                dialog.OpenWindow();
            });
            cancelBack.onClick.AddListener(() =>
            {
                LockUI(false);
            });
            submitBack.onClick.AddListener(async () =>
            {
                mask.SetActive(true);
                PhotonNetwork.LeaveRoom();
                var op = SceneManager.LoadSceneAsync("Desktop");
                await op;
            });
        }

        private void Start()
        {
            RefreshPlayerUI();
        }

        private void LockUI(bool value)
        {
            isLockUI = value;
            foreach (var monoBehaviour in uiGroup)
            {
                monoBehaviour.enabled = value;
            }
        }

        private void Update()
        {
            if (!isLockUI)
            {
                if (Input.GetKeyDown(KeyCode.U))
                {
                    leftWeapon.onClick.Invoke();
                }

                if (Input.GetKeyDown(KeyCode.I))
                {
                    rightWeapon.onClick.Invoke();
                }
            }
        }

        public void OnShowDamage(ActorView pos, int value)
        {
            var go = ObjectPool.GlobalObtain(damageText.guid, Vector3.zero, Quaternion.identity, false);
            go.transform.SetParent(damageTextRoot);
            go.GetComponent<DamageTextUIManager>().Initialize(pos.transform, value);
            if (pos.photonView.IsMine)
            {
                RefreshPlayerUI();
            }
        }

        private void RefreshPlayerUI()
        {
            lanTiao.fillAmount = (float) LocalPlayer.hp / PlayerConfigManager.playerInfo.Item2.shengMingZhi;
            xueTiao.fillAmount = (float) LocalPlayer.mp / PlayerConfigManager.playerInfo.Item2.nengLiangZhi;
            xueText.text = LocalPlayer.hp + "/" + PlayerConfigManager.playerInfo.Item2.shengMingZhi;
            lanText.text = LocalPlayer.mp + "/" + PlayerConfigManager.playerInfo.Item2.nengLiangZhi;
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
        }

        [Serializable]
        public class OnWeaponChangedEvent : UnityEvent<WeaponInfoAsset>
        {
        }
    }
}