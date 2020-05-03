using System;
using System.Collections;
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
            IEnumerator Wait()
            {
                yield return new WaitForEndOfFrame();
                webView.SetBackButtonEnabled(false);
                webView.UpdateFrame();
                webView.Load(url);
            }
            StartCoroutine(Wait());
            button.onClick.AddListener(() => back());
        }
    }
}