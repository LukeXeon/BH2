using System;
using System.Globalization;
using Dash.Scripts.Config;
using Dash.Scripts.Levels;
using Dash.Scripts.Cloud;
using Dash.Scripts.Levels.Config;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Dash.Scripts.UIManager
{
    public class WeaponInfoUIManager : MonoBehaviour
    {
        public Image image;
        public GameObject root;
        public TextMeshProUGUI displayName;
        public TextMeshProUGUI gongJiLi;
        public TextMeshProUGUI sheSu;
        public TextMeshProUGUI xiaoGuo;
        public Button button;
        public TextMeshProUGUI btnText;
        public Button back;
        public Image exp;
        public TextMeshProUGUI expText;
        public TextMeshProUGUI levelText;
        public Animator animator;
        public void Open(string btnTextValue, EWeapon weapon, Action<EWeapon> callback, Action onClose)
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

            if (onClose != null)
            {
                back.onClick.AddListener(() => { onClose(); });
            }

            back.onClick.AddListener(Close);
            var info = GameConfigManager.weaponTable[weapon.typeId];
            image.sprite = info.sprite;
            image.SetNativeSize();
            displayName.text = info.displayName;
            var runtime = RuntimeWeaponInfo.Build(weapon);
            gongJiLi.text = runtime.gongJiLi.ToString();
            sheSu.text = runtime.sheSu.ToString(CultureInfo.InvariantCulture);
            if (info.xiaoGuo != null)
            {
                xiaoGuo.text = info.xiaoGuo.text;
            }
            else
            {
                xiaoGuo.text = "全部木大";
            }

            var l = GameConfigManager.GetWeaponLevel(weapon.exp);
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