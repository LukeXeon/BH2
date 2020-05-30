using System;
using System.Linq;
using Dash.Scripts.Setting;
using Dash.Scripts.Core;
using Dash.Scripts.Gameplay.Levels;
using Dash.Scripts.Gameplay.Setting;
using Dash.Scripts.Gameplay.View;
using Michsky.UI.ModernUIPack;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Dash.Scripts.Gameplay.UIManager
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
        public ModalWindowManager stopWindow;
        public TextMeshProUGUI stopContent;
        public Button stopSubmit;
        private MonoBehaviour[] uiGroup;
        private bool UIEnable;
        private bool isStop;

        private void Awake()
        {
            uiGroup = new MonoBehaviour[] {joystick, fire0, fire1, leftWeapon, rightWeapon};
            var player = PlayerConfigManager.playerInfo.Item1;
            icon.sprite = player.icon;
            playerName.text = player.displayName;

            var sprite = PlayerConfigManager.weaponInfos.First().Item1.sprite;
            
            
            var rectTransform = weapon.rectTransform;
            var v2 = rectTransform.sizeDelta;
            var rate = sprite.rect.width / sprite.rect.height;
            v2.x = v2.y * rate;
            rectTransform.sizeDelta = v2;
            weapon.sprite = sprite;
            
            
            if (weaponChanged == null) weaponChanged = new OnWeaponChangedEvent();

            leftWeapon.onClick.AddListener(() =>
            {
                var time = Time.time;
                if (time - lastQieQiang < qieQiangJianGe) return;

                var last = LocalPlayer.weaponIndex - 1;
                if (last < 0) last = PlayerConfigManager.weaponInfos.Count - 1;
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


            weaponChanged.AddListener(Call);
            back.onClick.AddListener(() =>
            {
                EnableUI(false);
                dialog.OpenWindow();
            });
            cancelBack.onClick.AddListener(() => { EnableUI(true); });
            var x = new UnityAction(() =>
            {
                mask.SetActive(true);
                PhotonNetwork.LeaveRoom();
                SceneManager.LoadSceneAsync("Desktop");
            });
            submitBack.onClick.AddListener(x);
            stopSubmit.onClick.AddListener(x);
        }

        private void Start()
        {
            RefreshPlayerUI();
        }

        private void EnableUI(bool value)
        {
            UIEnable = value;
            foreach (var monoBehaviour in uiGroup)
            {
                monoBehaviour.enabled = value;
            }
        }
        
        private void Call(WeaponInfoAsset info)
        {
            var s = info.sprite;
            var rectTransform2 = weapon.rectTransform;
            var v22 = rectTransform2.sizeDelta;
            var rate2 = s.rect.width / s.rect.height;
            v22.x = v22.y * rate2;
            rectTransform2.sizeDelta = v22;
            weapon.sprite = s;
        }

        private void Update()
        {
            if (UIEnable)
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

        public void OnAllPlayerDie()
        {
            StopThGame("团长，你在干什么啊团长？（指你们都死了）\n\n\n");
        }

        private void StopThGame(string text)
        {
            if (isStop)
            {
                return;
            }

            isStop = true;
            EnableUI(false);
            stopContent.text = text;
            stopWindow.OpenWindow();
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
            xueTiao.fillAmount =
                (float) Mathf.Max(LocalPlayer.hp, 0) / PlayerConfigManager.playerInfo.Item2.shengMingZhi;
            lanTiao.fillAmount =
                (float) Mathf.Max(LocalPlayer.mp, 0) / PlayerConfigManager.playerInfo.Item2.nengLiangZhi;
            xueText.text = Mathf.Max(LocalPlayer.hp, 0) + "/" + PlayerConfigManager.playerInfo.Item2.shengMingZhi;
            lanText.text = Mathf.Max(LocalPlayer.mp, 0) + "/" + PlayerConfigManager.playerInfo.Item2.nengLiangZhi;
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            StopThGame("由于主客户端玩家失去连接，然后写游戏的程序是屑，懒得写这块的同步代码，所以团长你现在必须要停下来了。\n\n\n");
        }

        [Serializable]
        public class OnWeaponChangedEvent : UnityEvent<WeaponInfoAsset>
        {
        }
    }
}