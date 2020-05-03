using System;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_STANDALONE_WIN||UNITY_EDITOR_WIN
using ZenFulcrum.EmbeddedBrowser;

#endif

namespace Dash.Scripts.UIManager
{
    public class Win32WebUIManager : MonoBehaviour, IWebUIManager
    {
        public void Initialize(string url, Action back)
        {
#if UNITY_STANDALONE_WIN||UNITY_EDITOR_WIN
            browser.LoadURL(url, true);
            backBtn.onClick.AddListener(() => back());
#endif
        }
#if UNITY_STANDALONE_WIN||UNITY_EDITOR_WIN
        public Browser browser;
        public Button backBtn;
#endif
    }
}