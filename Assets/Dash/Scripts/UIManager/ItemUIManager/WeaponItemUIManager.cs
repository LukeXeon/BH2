using System;
using Dash.Scripts.Cloud;
using Dash.Scripts.Setting;
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

        public void Apply(WeaponEntity weapon, Action onShow, Action onChaKan, Action onTihuan)
        {
            if (weapon != null)
            {
                var info = GameSettingManager.weaponTable[weapon.typeId];
                image.gameObject.SetActive(true);
                var rectTransform = image.rectTransform;
                var v2 = rectTransform.sizeDelta;
                var rate = info.sprite.rect.width / info.sprite.rect.height;
                v2.x = v2.y * rate;
                rectTransform.sizeDelta = v2;
                image.sprite = info.sprite;
                button.onClick.AddListener(() => onShow());
                chaKan.onClick.AddListener(() => onChaKan());
                displayName.text = info.displayName;
                level.text = "LV " + GameSettingManager.GetWeaponLevel(weapon.exp).count;
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