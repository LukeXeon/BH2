using System;
using Dash.Scripts.Config;
using Dash.Scripts.Cloud;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dash.Scripts.UIManager
{
    public class WeaponCardUIManager : MonoBehaviour
    {
        public TextMeshProUGUI level;
        public TextMeshProUGUI displayName;
        public Image image;
        public GameObject markInUse;
        public Button button;

        public void Apply(EWeapon weapon, Action<EWeapon> callback)
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