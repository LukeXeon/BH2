using System;
using Dash.Scripts.Assets;
using Dash.Scripts.Network.Cloud;
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
            var info = GameInfoManager.weaponTable[weapon.typeId];
            image.sprite = info.sprite;
            level.text = "LV " + GameInfoManager.GetWeaponLevel(weapon.exp).count;
            displayName.text = info.displayName;
            markInUse.SetActive(weapon.player != null);
            button.onClick.AddListener(() => callback(weapon));
        }
    }
}