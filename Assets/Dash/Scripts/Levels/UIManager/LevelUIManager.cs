using System;
using System.Linq;
using Dash.Scripts.Config;
using Dash.Scripts.Core;
using Dash.Scripts.Levels.Config;
using Dash.Scripts.Levels.Core;
using Michsky.UI.ModernUIPack;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Dash.Scripts.Levels.UIManager
{
    public class LevelUIManager : MonoBehaviour
    {
        public Image xueTiao;
        public Image lanTiao;
        public TextMeshProUGUI xueText;
        public TextMeshProUGUI lanText;
        public Button leftWeapon;
        public Button rightWeapon;
        public Image weapon;
        public Image icon;
        public TextMeshProUGUI playerName;
        public Button back;
        public Button submitBack;
        public ModalWindowManager dialog;
        public GameObject mask;
        public GuidIndexer damageText;
        public OnWeaponChangedEvent weaponChanged;
        private const float qieQiangJianGe = 0.3f;
        private float lastQieQiang;

        [Serializable]
        public class OnWeaponChangedEvent : UnityEvent<WeaponInfoAsset>
        {
        }

        private void Awake()
        {
            var player = LocalPlayerInfo.playerInfo.Item1;
            icon.sprite = player.icon;
            playerName.text = player.displayName;
            weapon.sprite = LocalPlayerInfo.weaponInfos.First().Item1.sprite;
            if (weaponChanged == null)
            {
                weaponChanged = new OnWeaponChangedEvent();
            }

            leftWeapon.onClick.AddListener(() =>
            {
                var time = Time.time;
                if (time - lastQieQiang < qieQiangJianGe)
                {
                    return;
                }

                var last = LocalPlayerDynamicInfo.currentWeaponIndex - 1;
                if (last < 0)
                {
                    last = LocalPlayerInfo.weaponInfos.Count - 1;
                }

                Debug.Log(last);
                if (last == LocalPlayerDynamicInfo.currentWeaponIndex)
                {
                    return;
                }

                var info = LocalPlayerInfo.weaponInfos[last].Item1;
                LocalPlayerDynamicInfo.currentWeaponIndex = last;
                weaponChanged.Invoke(info);
                lastQieQiang = time;
            });
            rightWeapon.onClick.AddListener(() =>
            {
                var time = Time.time;
                if (time - lastQieQiang < qieQiangJianGe)
                {
                    return;
                }

                var last = LocalPlayerDynamicInfo.currentWeaponIndex;
                LocalPlayerDynamicInfo.currentWeaponIndex = (last + 1) % LocalPlayerInfo.weaponInfos.Count;
                if (last == LocalPlayerDynamicInfo.currentWeaponIndex)
                {
                    return;
                }

                var info = LocalPlayerInfo.weaponInfos[LocalPlayerDynamicInfo.currentWeaponIndex].Item1;
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

        public void OnShowDamage(Transform pos, int value)
        {
            var go = ObjectPool.GlobalObtain(damageText.guid, Vector3.zero, Quaternion.identity, false);
            go.transform.SetParent(icon.canvas.transform);
            go.GetComponent<DamageTextUIManager>().Initialize(pos, value);
        }
    }
}