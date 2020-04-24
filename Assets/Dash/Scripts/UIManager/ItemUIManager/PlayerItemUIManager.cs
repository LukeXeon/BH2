using System;
using Dash.Scripts.Cloud;
using Dash.Scripts.Config;
using UnityEngine;
using UnityEngine.UI;

namespace Dash.Scripts.UIManager
{
    public class PlayerItemUIManager : MonoBehaviour
    {
        public Button button;
        public Image image;
        public GameObject markInLock;

        public void Init(PlayerInfoAsset infoAsset)
        {
            image.sprite = infoAsset.icon;
            infoAsset.skel.GetSkeletonData(true);
        }

        public void Apply(EPlayer player, Action onLock, Action onUnLock)
        {
            markInLock.SetActive(player == null);
            button.onClick.RemoveAllListeners();
            if (player != null)
                button.onClick.AddListener(() => { onUnLock(); });
            else
                button.onClick.AddListener(() => onLock());
        }
    }
}