using System;
using UnityEngine;
using UnityEngine.UI;

namespace Dash.Scripts.UIManager
{
    public class MobileWebUIManager : MonoBehaviour, IWebUIManager
    {
        public Button button;
        public UniWebView webView;

        public void Init(string url, Action back)
        {
            webView.Load(url);
            button.onClick.AddListener(() => back());
        }

        private void Start()
        {
            webView.SetBackButtonEnabled(false);
        }
    }
}