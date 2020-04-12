using System;
using Dash.Scripts.Config;
using Dash.Scripts.GamePlay;
using Dash.Scripts.GamePlay.Info;
using Dash.Scripts.Network.Cloud;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dash.Scripts.UIManager
{
    public class ShengHenInfoUIManager : MonoBehaviour
    {
        public Image image;
        public GameObject root;
        public TextMeshProUGUI displayName;
        public TextMeshProUGUI fangYuLi;
        public TextMeshProUGUI shengMingZhi;
        public TextMeshProUGUI nengLiangZhi;
        public TextMeshProUGUI xiaoGuo;
        public Button button;
        public TextMeshProUGUI btnText;
        public Button back;
        public Image exp;
        public TextMeshProUGUI expText;
        public TextMeshProUGUI levelText;
        public Animator animator;

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

            if (onClose != null)
            {
                back.onClick.AddListener(() => { onClose(); });
            }

            back.onClick.AddListener(Close);
            root.SetActive(true);
            var info = GameInfoManager.shengHenTable[shengHen.typeId];
            image.sprite = info.image;
            displayName.text = info.displayName;
            var runtime = RuntimeShengHenInfo.Build(shengHen);
            fangYuLi.text = runtime.fangYuLi.ToString();
            shengMingZhi.text = runtime.shengMingZhi.ToString();
            nengLiangZhi.text = runtime.shengMingZhi.ToString();
            if (info.xiaoGuo != null)
            {
                xiaoGuo.text = info.xiaoGuo.text;
            }
            else
            {
                xiaoGuo.text = "全部木大";
            }

            var l = GameInfoManager.GetShengHenLevel(shengHen.exp);
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