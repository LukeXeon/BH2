using System;
using UnityEngine;
using UnityEngine.UI;
using ZenFulcrum.EmbeddedBrowser;

namespace Dash.Scripts.UIManager
{
    public class Win32WebUIManager : MonoBehaviour, IWebUIManager
    {
        public Browser browser;
        public Button backBtn;

        public void Init(string url, Action back)
        {
            browser.LoadURL(url, true);
            backBtn.onClick.AddListener(() => back());
        }
    }
}