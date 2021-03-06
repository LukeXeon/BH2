using System;
using System.Globalization;
using Dash.Scripts.Cloud;
using Dash.Scripts.Setting;
using Dash.Scripts.Gameplay.Setting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dash.Scripts.UIManager
{
    public class WeaponInfoUIManager : MonoBehaviour
    {
        public Animator animator;
        public Button back;
        public TextMeshProUGUI btnText;
        public Button button;
        public TextMeshProUGUI displayName;
        public Image exp;
        public TextMeshProUGUI expText;
        public TextMeshProUGUI gongJiLi;
        public Image image;
        public TextMeshProUGUI levelText;
        public GameObject root;
        public TextMeshProUGUI sheSu;
        public TextMeshProUGUI xiaoGuo;

        public void Open(string btnTextValue, WeaponEntity weapon, Action<WeaponEntity> callback, Action onClose)
        {
            animator.Play("Fade-in");
            root.SetActive(true);
            if (btnTextValue != null)
            {
                btnText.text = btnTextValue;
                button.gameObject.SetActive(true);
                button.onClick.AddListener(() => { callback(weapon); });
            }
            else
            {
                button.gameObject.SetActive(false);
            }

            if (onClose != null) back.onClick.AddListener(() => { onClose(); });

            back.onClick.AddListener(Close);
            var info = GameSettingManager.weaponTable[weapon.typeId];

            var rectTransform = image.rectTransform;
            var v2 = rectTransform.sizeDelta;
            var rate = info.sprite.rect.height / info.sprite.rect.width;
            v2.y = v2.x * rate;
            rectTransform.sizeDelta = v2;
            image.sprite = info.sprite;
            
            
            displayName.text = info.displayName;
            var runtime = RuntimeWeaponInfo.Build(weapon);
            gongJiLi.text = runtime.gongJiLi.ToString();
            sheSu.text = info.sheSu.ToString(CultureInfo.InvariantCulture);
            if (info.xiaoGuo != null)
                xiaoGuo.text = info.xiaoGuo.text;
            else
                xiaoGuo.text = "全部木大";

            var l = GameSettingManager.GetWeaponLevel(weapon.exp);
            expText.text = l.currentExp + "/" + l.maxExp;
            exp.fillAmount = (float) l.currentExp / l.maxExp;
            levelText.text = "Lv " + l.count;
        }

        public void Close()
        {
            image.sprite = null;
            root.SetActive(false);
            button.onClick.RemoveAllListeners();
            back.onClick.RemoveAllListeners();
        }
    }
}