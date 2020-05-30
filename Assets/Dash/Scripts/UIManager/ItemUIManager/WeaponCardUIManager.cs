using System;
using Dash.Scripts.Cloud;
using Dash.Scripts.Setting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dash.Scripts.UIManager.ItemUIManager
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
            Debug.Log(weapon.typeId);
            var info = GameSettingManager.weaponTable[weapon.typeId];
            var rectTransform = image.rectTransform;
            var v2 = rectTransform.sizeDelta;
            if (info.sprite)
            {
                var rate = info.sprite.rect.width / info.sprite.rect.height;
                v2.x = v2.y * rate;
                rectTransform.sizeDelta = v2;
                image.sprite = info.sprite;
            }

            level.text = "LV " + GameSettingManager.GetWeaponLevel(weapon.exp).count;
            displayName.text = info.displayName;
            markInUse.SetActive(weapon.player != null);
            button.onClick.AddListener(() => callback(weapon));
        }
    }
}