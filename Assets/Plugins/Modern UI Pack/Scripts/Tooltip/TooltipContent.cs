using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace Michsky.UI.ModernUIPack
{
    public class TooltipContent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("CONTENT")]
        [TextArea] public string description;

        [Header("RESOURCES")]
        public GameObject tooltipObject;
        public TextMeshProUGUI descriptionText;

        private Animator tooltipAnimator;

        void Start()
        {
            if (tooltipObject == null)
            {
                try
                {
                    tooltipObject = GameObject.Find("Tooltip");
                    descriptionText = tooltipObject.transform.GetComponentInChildren<TextMeshProUGUI>();
                }

                catch
                {
                    Debug.LogError("No Tooltip object assigned.");
                }
            }

            if (tooltipObject != null)
                tooltipAnimator = tooltipObject.GetComponent<Animator>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (tooltipObject.activeSelf && tooltipObject != null)
            {
                descriptionText.text = description;
                tooltipAnimator.Play("In");
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (tooltipObject.activeSelf && tooltipObject != null)
                tooltipAnimator.Play("Out");
        }
    }
}