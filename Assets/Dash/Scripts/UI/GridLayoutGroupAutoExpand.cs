using UnityEngine;
using UnityEngine.UI;

namespace Dash.Scripts.UIManager
{
    [RequireComponent(typeof(RectTransform), typeof(GridLayoutGroup))]
    public class GridLayoutGroupAutoExpand : MonoBehaviour
    {
        private GridLayoutGroup gridLayoutGroup;

        private RectTransform rectTransform;
        public int rowCount = 2;

        private void Awake()
        {
            rectTransform = transform as RectTransform;
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

        private void OnRectTransformDimensionsChange()
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