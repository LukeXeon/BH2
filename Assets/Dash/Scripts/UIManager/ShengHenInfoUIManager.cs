using System;
using Dash.Scripts.Cloud;
using Dash.Scripts.Config;
using Dash.Scripts.Levels.Config;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dash.Scripts.UIManager
{
    public class ShengHenInfoUIManager : MonoBehaviour
    {
        public Animator animator;
        public Button back;
        public TextMeshProUGUI btnText;
        public Button button;
        public TextMeshProUGUI displayName;
        public Image exp;
        public TextMeshProUGUI expText;
        public TextMeshProUGUI fangYuLi;
        public Image image;
        public TextMeshProUGUI levelText;
        public TextMeshProUGUI nengLiangZhi;
        public GameObject root;
        public TextMeshProUGUI shengMingZhi;
        public TextMeshProUGUI xiaoGuo;

        public void Open(string btnTextValue, EShengHen shengHen, Action<EShengHen> callback, Action onClose)
        {
            animator.Play("Fade-in");
            if (btnTextValue != null)
            {
                button.gameObject.SetActive(true);
                button.onClick.AddListener(() => { callback(shengHen); });
                btnText.text = btnTextValue;
            }
            else
            {
                button.gameObject.SetActive(false);
            }

            if (onClose != null) back.onClick.AddListener(() => { onClose(); });

            back.onClick.AddListener(Close);
            root.SetActive(true);
            var info = GameConfigManager.shengHenTable[shengHen.typeId];
            image.sprite = info.image;
            displayName.text = info.displayName;
            var runtime = RuntimeShengHenInfo.Build(shengHen);
            fangYuLi.text = runtime.fangYuLi.ToString();
            shengMingZhi.text = runtime.shengMingZhi.ToString();
            nengLiangZhi.text = runtime.shengMingZhi.ToString();
            if (info.xiaoGuo != null)
                xiaoGuo.text = info.xiaoGuo.text;
            else
                xiaoGuo.text = "全部木大";

            var l = GameConfigManager.GetShengHenLevel(shengHen.exp);
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