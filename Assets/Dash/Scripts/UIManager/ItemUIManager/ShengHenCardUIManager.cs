using System;
using System.Linq;
using Dash.Scripts.Config;
using Dash.Scripts.Cloud;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dash.Scripts.UIManager
{
    public class ShengHenCardUIManager : MonoBehaviour
    {
        public TextMeshProUGUI level;
        public Image image;
        public TextMeshProUGUI displayName;
        public GameObject markInUse;
        public Button button;

        public void Apply(EShengHen shengHen, Action<EShengHen> callack)
        {
            var info = GameGlobalInfoManager.shengHenTable[shengHen.typeId];
            image.sprite = info.image;
            level.text = "LV " + GameGlobalInfoManager.GetShengHenLevel(shengHen.exp).count;
            displayName.text = info.displayName;
            markInUse.SetActive(shengHen.player != null);
            button.onClick.AddListener(() => callack(shengHen));
        }
    }
}