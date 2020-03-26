using System;
using UnityEngine;
using UnityEngine.UI;

namespace Dash.Scripts.UIManager
{
    [RequireComponent(typeof(RectTransform), typeof(GridLayoutGroup))]
    public class GridLayoutGroupAutoExpand : MonoBehaviour
    {
        public int rowCount = 2;

        private RectTransform rectTransform;
        private GridLayoutGroup gridLayoutGroup;

        private void Awake()
        {
            rectTransform  = transform as RectTransform;
            gridLayoutGroup = GetComponent<GridLayoutGroup>();
        }

        private void Start()
        {
            Apply();
        }

        private void OnEnable()
        {
            Apply();
        }
        
        void OnRectTransformDimensionsChange()
        {
            Awake();
            Apply();
        }


        private void OnValidate()
        {
            Awake();
            Apply();
        }

        public void Apply()
        {
            gridLayoutGroup.cellSize = new Vector2(
                rectTransform.rect.width / rowCount
                - gridLayoutGroup.spacing.x * Mathf.Max(0, rowCount - 1) / 2f
                - gridLayoutGroup.padding.horizontal / 2f,
                gridLayoutGroup.cellSize.y
            );
        }
    }
}