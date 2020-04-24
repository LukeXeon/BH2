﻿using System;
using System.Linq;
using Dash.Scripts.Config;
using Dash.Scripts.Core;
using Dash.Scripts.Levels.Config;
using Michsky.UI.ModernUIPack;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
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
        public Image icon;
        public TextMeshProUGUI playerName;
        public Button back;
        public Button submitBack;
        public ModalWindowManager dialog;
        public GameObject mask;
        public OnWeaponChangedEvent weaponChanged;
        private int currentWeaponIndex;
        private const float qieQiangJianGe = 0.3f;
        private float lastQieQiang;

        [Serializable]
        public class OnWeaponChangedEvent : UnityEvent<WeaponInfoAsset>
        {
        }

        private void Awake()
        {
            var player = InLevelConfigManager.playerInfo.Item1;
            icon.sprite = player.icon;
            playerName.text = player.displayName;
            weapon.sprite = InLevelConfigManager.weaponInfos.First().Item1.sprite;
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

                var last = currentWeaponIndex - 1;
                if (last < 0)
                {
                    last = InLevelConfigManager.weaponInfos.Count - 1;
                }

                Debug.Log(last);
                if (last == currentWeaponIndex)
                {
                    return;
                }

                var info = InLevelConfigManager.weaponInfos[last].Item1;
                currentWeaponIndex = last;
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

                var last = currentWeaponIndex;
                currentWeaponIndex = (last + 1) % InLevelConfigManager.weaponInfos.Count;
                if (last == currentWeaponIndex)
                {
                    return;
                }

                var info = InLevelConfigManager.weaponInfos[currentWeaponIndex].Item1;
                weaponChanged.Invoke(info);
                lastQieQiang = time;
            });
            weaponChanged.AddListener(info => { weapon.sprite = info.sprite; });
            back.onClick.AddListener(() =>
            {
                dialog.OpenWindow();
            });
            submitBack.onClick.AddListener(async () =>
            {
                mask.SetActive(true);
                PhotonNetwork.LeaveRoom();
                var op = SceneManager.LoadSceneAsync("Desktop");
                await op;
            });
        }
    }
}