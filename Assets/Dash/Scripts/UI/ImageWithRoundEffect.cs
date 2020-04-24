using UnityEngine;
using UnityEngine.UI;

namespace Dash.Scripts.UI
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Image), typeof(RectTransform))]
    public class ImageWithRoundEffect : MonoBehaviour
    {
        private static readonly int Props = Shader.PropertyToID("_WidthHeightRadius");
        private Image image;
        private Material material;
        public float radius;

        private void Awake()
        {
            InitImage();
        }

        private void InitImage()
        {
            if (image == null) image = GetComponent<Image>();

            if (image == null) image = gameObject.AddComponent<Image>();
        }

        private void Start()
        {
            Refresh();
        }

        private void OnRectTransformDimensionsChange()
        {
            Refresh();
        }


        private void OnValidate()
        {
            Refresh();
        }

        private void Refresh()
        {
            if (Application.isEditor || image == null) InitImage();

            var rect = ((RectTransform) transform).rect;
            if (material == null) material = new Material(Shader.Find("UI/RoundedCorners/RoundedCorners"));

            material.SetVector(Props, new Vector4(rect.width, rect.height, radius, 0));

            image.material = material;
        }
    }
}