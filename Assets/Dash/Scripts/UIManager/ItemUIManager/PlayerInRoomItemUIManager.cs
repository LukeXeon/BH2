using Photon.Realtime;
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
        public GameObject isMaster;
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

        public void Clear()
        {
            renderCamera.enabled = false;
            skeletonAnimation.enabled = false;
            image.enabled = false;
            isReady.SetActive(false);
            isMaster.SetActive(false);
            displayName.text = null;
        }

        public void Apply(Player player)
        {
            renderCamera.enabled = true;
            skeletonAnimation.enabled = true;
            image.enabled = true;
            player.CustomProperties.TryGetValue("displayName", out var displayNameValue);
            player.CustomProperties.TryGetValue("isReady", out var isReadyValue);
            this.displayName.text = displayNameValue as string ?? "...";
            if (player.IsMasterClient)
            {
                isMaster.SetActive(true);
                isReady.SetActive(false);
            }
            else
            {
                isMaster.SetActive(false);
                isReady.SetActive(isReadyValue as bool? ?? false);
            }
        }
    }
}
