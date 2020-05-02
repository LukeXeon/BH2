using System;
using Dash.Scripts.Cloud;
using Dash.Scripts.Config;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dash.Scripts.UIManager
{
    public class WeaponCardUIManager : MonoBehaviour
    {
        public Button button;
        public TextMeshProUGUI displayName;
        public Image image;
        public TextMeshProUGUI level;
        public GameObject markInUse;

        public void Apply(WeaponEntity weapon, Action<WeaponEntity> callback)
        {
            var info = GameConfigManager.weaponTable[weapon.typeId];
            image.sprite = info.sprite;
            level.text = "LV " + GameConfigManager.GetWeaponLevel(weapon.exp).count;
            displayName.text = info.displayName;
            markInUse.SetActive(weapon.player != null);
            button.onClick.AddListener(() => callback(weapon));
        }
    }
}