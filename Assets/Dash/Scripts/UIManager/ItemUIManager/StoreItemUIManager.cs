using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Dash.Scripts.UIManager.ItemUIManager
{
    public class StoreItemUIManager : MonoBehaviour
    {
        public TextMeshProUGUI title;
        public TextMeshProUGUI cost;
        public Button button;
        public GameObject outline;
        public GameObject noBuyMask;
        public Image sealImage;
        public Image otherImage;

        private void Awake()
        {
            outline.SetActive(false);
            noBuyMask.SetActive(false);
        }

        public void Apply(
            string title,
            string cost,
            Sprite sprite,
            bool isSeal,
            bool isBuy,
            UnityAction action)
        {
            button.onClick.AddListener(action);
            if (!isSeal)
            {
                otherImage.gameObject.SetActive(true);
                sealImage.gameObject.SetActive(false);
                var rectTransform = otherImage.rectTransform;
                var v2 = rectTransform.sizeDelta;
                var rate = sprite.rect.height / sprite.rect.width;
                v2.y = v2.x * rate;
                rectTransform.sizeDelta = v2;
                otherImage.sprite = sprite;
            }
            else
            {
                otherImage.gameObject.SetActive(false);
                sealImage.gameObject.SetActive(true);
                sealImage.sprite = sprite;
            }

            noBuyMask.SetActive(!isBuy);

            this.cost.text = cost;
            this.title.text = title;
        }

        public void DoSelect()
        {
            outline.SetActive(true);
        }

        public void DoUnSelect()
        {
            outline.SetActive(false);
        }
    }
}