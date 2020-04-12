using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dash.Scripts.UIManager.ItemUIManager
{
    public class PlayerInRoomItemUIManager : MonoBehaviour
    {
        public Camera renderCamera;
        public GameObject isReady;
        public TextMeshProUGUI displayName;
        public SkeletonAnimation skeletonAnimation;
        public RawImage image;

        private void Awake()
        {
            var rect = image.rectTransform.rect;
            var rt = new RenderTexture(
                (int) rect.width,
                (int) rect.height,
                24
            );
            renderCamera.targetTexture = rt;
            image.texture = rt;
        }


    }
}
