﻿using System;
using System.Linq;
using Dash.Scripts.Config;
using Dash.Scripts.Levels.Config;
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
        public OnWeaponChangedEvent weaponChanged;
        private int currentWeaponIndex;

        [Serializable]
        public class OnWeaponChangedEvent : UnityEvent<WeaponInfoAsset>
        {
        }

        private void Awake()
        {
            weapon.sprite = InLevelConfigManager.weaponInfos.First().Item1.sprite;
            if (weaponChanged == null)
            {
                weaponChanged = new OnWeaponChangedEvent();
            }
            leftWeapon.onClick.AddListener(() =>
            {
                var last = currentWeaponIndex;
                if (last - 1 < 0)
                {
                    last = InLevelConfigManager.weaponInfos.Count - 1;
                }
                if (last == currentWeaponIndex)
                {
                    return;
                }

                var info = InLevelConfigManager.weaponInfos[last].Item1;
                currentWeaponIndex = last;
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
    }
}