using System;
using Dash.Scripts.Cloud;
using Dash.Scripts.Config;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dash.Scripts.UIManager.ItemUIManager
{
    public class ShengHenItemUIManager : MonoBehaviour
    {
        public Button button;
        public TextMeshProUGUI displayName;
        public Image image;
        public GameObject imageRoot;
        public TextMeshProUGUI level;
        public Button tiHuan;
        public GameObject unSetRoot;

        private void Awake()
        {
            Clear();
        }

        public void Apply(SealEntity shengHen, Action onShow, Action onTiHuan)
        {
            if (shengHen != null)
            {
                var info = GameConfigManager.shengHenTable[shengHen.typeId];
                level.text = "LV " + GameConfigManager.GetShengHenLevel(shengHen.exp).count;
                image.sprite = info.image;
                displayName.text = info.displayName;
                imageRoot.SetActive(true);
                unSetRoot.SetActive(false);
                button.onClick.AddListener(() => onShow());
            }
            else
            {
                level.text = "LV N/A";
                displayName.text = "N/A";
                unSetRoot.SetActive(true);
                imageRoot.SetActive(false);
            }

            tiHuan.onClick.AddListener(() => onTiHuan());
        }

        public void Clear()
        {
            image.sprite = null;
            tiHuan.onClick.RemoveAllListeners();
            button.onClick.RemoveAllListeners();
        }
    }
}