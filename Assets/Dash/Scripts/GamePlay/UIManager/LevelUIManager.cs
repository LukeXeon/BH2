using System;
using System.Linq;
using Dash.Scripts.Config;
using Dash.Scripts.Core;
using Dash.Scripts.GamePlay.Config;
using Dash.Scripts.GamePlay.Core;
using Dash.Scripts.GamePlay.View;
using Michsky.UI.ModernUIPack;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Dash.Scripts.GamePlay.UIManager
{
    public class LevelUIManager : MonoBehaviour
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

        private void Awake()
        {
            var player = GamePlayConfigManager.playerInfo.Item1;
            icon.sprite = player.icon;
            playerName.text = player.displayName;
            weapon.sprite = GamePlayConfigManager.weaponInfos.First().Item1.sprite;
            if (weaponChanged == null) weaponChanged = new OnWeaponChangedEvent();

            leftWeapon.onClick.AddListener(() =>
            {
                var time = Time.time;
                if (time - lastQieQiang < qieQiangJianGe) return;

                var last = LocalPlayer.weaponIndex - 1;
                if (last < 0) last = GamePlayConfigManager.weaponInfos.Count - 1;

                Debug.Log(last);
                if (last == LocalPlayer.weaponIndex) return;

                var info = GamePlayConfigManager.weaponInfos[last].Item1;
                LocalPlayer.weaponIndex = last;
                weaponChanged.Invoke(info);
                lastQieQiang = time;
            });
            rightWeapon.onClick.AddListener(() =>
            {
                var time = Time.time;
                if (time - lastQieQiang < qieQiangJianGe) return;

                var last = LocalPlayer.weaponIndex;
                LocalPlayer.weaponIndex = (last + 1) % GamePlayConfigManager.weaponInfos.Count;
                if (last == LocalPlayer.weaponIndex) return;

                var info = GamePlayConfigManager.weaponInfos[LocalPlayer.weaponIndex].Item1;
                weaponChanged.Invoke(info);
                lastQieQiang = time;
            });
            weaponChanged.AddListener(info => { weapon.sprite = info.sprite; });
            back.onClick.AddListener(() => { dialog.OpenWindow(); });
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

        private void Update()
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

        public void OnShowDamage(ActorView pos, int value)
        {
            var go = ObjectPool.GlobalObtain(damageText.guid, Vector3.zero, Quaternion.identity, false);
            go.transform.SetParent(icon.canvas.transform);
            go.GetComponent<DamageTextUIManager>().Initialize(pos.transform, value);
            if (pos.photonView.IsMine)
            {
                RefreshPlayerUI();
            }
        }

        private void RefreshPlayerUI()
        {
            lanTiao.fillAmount = (float) LocalPlayer.hp / GamePlayConfigManager.playerInfo.Item2.shengMingZhi;
            xueTiao.fillAmount = (float) LocalPlayer.mp / GamePlayConfigManager.playerInfo.Item2.nengLiangZhi;
            xueText.text = LocalPlayer.hp + "/" + GamePlayConfigManager.playerInfo.Item2.shengMingZhi;
            lanText.text = LocalPlayer.mp + "/" + GamePlayConfigManager.playerInfo.Item2.nengLiangZhi;
        }


        [Serializable]
        public class OnWeaponChangedEvent : UnityEvent<WeaponInfoAsset>
        {
        }
    }
}