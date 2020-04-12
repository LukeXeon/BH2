using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dash.Scripts.UIManager.ItemUIManager
{
    public class RoomTypeItemUIManager : MonoBehaviour
    {
        public Button button;
        public TextMeshProUGUI title;

        public void Apply(string text, Action callback)
        {
            title.text = text;
            button.onClick.AddListener(() => callback());
        }
    }
}