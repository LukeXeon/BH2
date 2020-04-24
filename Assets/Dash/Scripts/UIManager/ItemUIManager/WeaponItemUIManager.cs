using System;
using Dash.Scripts.Cloud;
using Dash.Scripts.Config;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dash.Scripts.UIManager.ItemUIManager
{
    public class WeaponItemUIManager : MonoBehaviour
    {
        public Button button;
        public Button chaKan;
        public TextMeshProUGUI displayName;
        public Image image;
        public TextMeshProUGUI level;
        public Button tiHuan;

        private void Awake()
        {
            Clear();
        }

        public void Apply(EWeapon weapon, Action onShow, Action onChaKan, Action onTihuan)
        {
            if (weapon != null)
            {
                var info = GameConfigManager.weaponTable[weapon.typeId];
                image.gameObject.SetActive(true);
                image.sprite = info.sprite;
                button.onClick.AddListener(() => onShow());
                chaKan.onClick.AddListener(() => onChaKan());
                displayName.text = info.displayName;
                level.text = "LV " + GameConfigManager.GetWeaponLevel(weapon.exp).count;
            }
            else
            {
                image.gameObject.SetActive(false);
                displayName.text = "N/A";
                level.text = "LV N/A";
            }

            tiHuan.onClick.AddListener(() => onTihuan());
        }

        public void SetShowTiHuan(bool value)
        {
            tiHuan.gameObject.SetActive(value);
        }

        public void Clear()
        {
            chaKan.onClick.RemoveAllListeners();
            button.onClick.RemoveAllListeners();
            tiHuan.onClick.RemoveAllListeners();
        }
    }
}