using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Dash.Scripts.UIManager.ItemUIManager
{
    public class StoreTitleUIManager : MonoBehaviour
    {
        public Image image;
        public Button button;
        public TextMeshProUGUI text;
        [Header("Assets")] 
        public Color selectColor;
        public Color unselectColor;
        
        private void Awake()
        {
            DoUnSelect();
        }

        public void Apply(string text, UnityAction action)
        {
            button.onClick.AddListener(action);
            this.text.text = text;
        }

        public void DoSelect()
        {
            image.color = selectColor;
            text.color = Color.white;
        }

        public void DoUnSelect()
        {
            image.color = unselectColor;
            text.color = Color.black;
        }

        public void Select()
        {
            button.onClick.Invoke();
        }
    }
}