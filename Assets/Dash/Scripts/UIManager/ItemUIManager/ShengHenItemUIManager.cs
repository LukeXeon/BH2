using System;
using Dash.Scripts.Assets;
using Dash.Scripts.Network.Cloud;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dash.Scripts.UIManager.ItemUIManager
{
    public class ShengHenItemUIManager : MonoBehaviour
    {
        public Button button;
        public Image image;
        public GameObject imageRoot;
        public GameObject unSetRoot;
        public TextMeshProUGUI displayName;
        public TextMeshProUGUI level;
        public Button tiHuan;

        private void Awake()
        {
            Clear();
        }

        public void Apply(EShengHen shengHen, Action onShow, Action onTiHuan)
        {
            if (shengHen != null)
            {
                var info = GameInfoManager.shengHenTable[shengHen.typeId];
                level.text = "LV " + GameInfoManager.GetShengHenLevel(shengHen.exp).count;
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