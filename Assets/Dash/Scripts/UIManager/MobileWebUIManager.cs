using System;
using UnityEngine;
using UnityEngine.UI;

namespace Dash.Scripts.UIManager
{
    public class MobileWebUIManager : MonoBehaviour, IWebUIManager
    {
        public Button button;

        private void Awake()
        {
        }

        public void Init(string url, Action back)
        {
            button.onClick.AddListener(() => back());
        }
    }
}
