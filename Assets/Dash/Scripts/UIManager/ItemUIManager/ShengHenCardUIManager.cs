using System;
using Dash.Scripts.Cloud;
using Dash.Scripts.Config;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dash.Scripts.UIManager
{
    public class ShengHenCardUIManager : MonoBehaviour
    {
        public Button button;
        public TextMeshProUGUI displayName;
        public Image image;
        public TextMeshProUGUI level;
        public GameObject markInUse;

        public void Apply(EShengHen shengHen, Action<EShengHen> callack)
        {
            var info = GameConfigManager.shengHenTable[shengHen.typeId];
            image.sprite = info.image;
            level.text = "LV " + GameConfigManager.GetShengHenLevel(shengHen.exp).count;
            displayName.text = info.displayName;
            markInUse.SetActive(shengHen.player != null);
            button.onClick.AddListener(() => callack(shengHen));
        }
    }
}