using System;
using Dash.Scripts.Config;
using Dash.Scripts.Network.Cloud;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dash.Scripts.UIManager.ItemUIManager
{
    public class WeaponItemUIManager : MonoBehaviour
    {
        public Button button;
        public Image image;
        public TextMeshProUGUI displayName;
        public TextMeshProUGUI level;
        public Button tiHuan;
        public Button chaKan;

        private void Awake()
        {
            Clear();
        }

        public void Apply(EWeapon weapon, Action onShow, Action onChaKan, Action onTihuan)
        {
            if (weapon != null)
            {
                var info = GameInfoManager.weaponTable[weapon.typeId];
                image.gameObject.SetActive(true);
                image.sprite = info.sprite;
                button.onClick.AddListener(call: () => onShow());
                chaKan.onClick.AddListener(call: () => onChaKan());
                displayName.text = info.displayName;
                level.text = "LV " + GameInfoManager.GetWeaponLevel(weapon.exp).count;
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