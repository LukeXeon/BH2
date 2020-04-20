using System;
using System.Linq;
using Dash.Scripts.Config;
using Dash.Scripts.Levels.Config;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Dash.Scripts.Levels.Core
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
        public Image mask;
        public OnWeaponChangedEvent weaponChanged;
        private int currentWeaponIndex = 0;
        private LevelStartupManager levelStartupManager;

        [Serializable]
        public class OnWeaponChangedEvent : UnityEvent<WeaponInfoAsset>
        {
        }

        private void Awake()
        {
            levelStartupManager = FindObjectOfType<LevelStartupManager>();
            weapon.sprite = InLevelConfigManager.weaponInfos.First().Item1.sprite;
            mask.gameObject.SetActive(true);
            var typeId = (int) PhotonNetwork.CurrentRoom.CustomProperties["typeId"];
            mask.sprite = GameConfigManager.guanQiaInfoTable[typeId].image;
            if (weaponChanged == null)
            {
                weaponChanged = new OnWeaponChangedEvent();
            }
            leftWeapon.onClick.AddListener(() =>
            {
                var last = currentWeaponIndex;
                if (currentWeaponIndex < 0)
                {
                    currentWeaponIndex = InLevelConfigManager.weaponInfos.Count - 1;
                }

                --currentWeaponIndex;
                if (last == currentWeaponIndex)
                {
                    return;
                }

                var info = InLevelConfigManager.weaponInfos[currentWeaponIndex].Item1;
                weaponChanged.Invoke(info);
            });
            rightWeapon.onClick.AddListener(() =>
            {
                var last = currentWeaponIndex;
                currentWeaponIndex = (last + 1) % InLevelConfigManager.weaponInfos.Count;
                if (last == currentWeaponIndex)
                {
                    return;
                }

                var info = InLevelConfigManager.weaponInfos[currentWeaponIndex].Item1;
                weaponChanged.Invoke(info);
            });
            weaponChanged.AddListener(info => { weapon.sprite = info.sprite; });
        }

        private void Start()
        {
            levelStartupManager.onLevelLoadedEvent.AddListener(OnLevelPrepared);
        }

        public void OnLevelPrepared()
        {
            mask.gameObject.SetActive(false);
        }
    }
}