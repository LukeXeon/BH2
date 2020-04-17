using System;
using Dash.Scripts.Config;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dash.Scripts.UIManager.ItemUIManager
{
    public class RoomItemUIManager : MonoBehaviour
    {
        public TextMeshProUGUI roomByUser;
        public TextMeshProUGUI guanQiaMing;
        public Button jiaRu;
        public Image[] playerItems;

        public void Apply(RoomInfo roomInfo, Action callback)
        {
            roomInfo.CustomProperties.TryGetValue("displayName", out var displayName);
            if (displayName != null)
            {
                roomByUser.text = (string) displayName;
            }
            else
            {
                roomByUser.text = "...";
            }

            roomInfo.CustomProperties.TryGetValue("typeId", out var typeId);
            if (typeId != null)
            {
                guanQiaMing.text = GameGlobalInfoManager.guanQiaInfoTable[(int) typeId]?.displayName ?? "...";
            }

            for (int i = 0; i < 3; i++)
            {
                
                roomInfo.CustomProperties.TryGetValue(i + "playerTypeId", out var playerTypeId);
                Debug.Log(i + "playerTypeId" + playerTypeId);
                if (playerTypeId != null && (int) playerTypeId != -1)
                {
                    playerItems[i].gameObject.SetActive(true);
                    playerItems[i].sprite = GameGlobalInfoManager.playerTable[(int) playerTypeId].icon;
                }
                else
                {
                    playerItems[i].sprite = null;
                    playerItems[i].gameObject.SetActive(false);
                }
            }

            jiaRu.onClick.RemoveAllListeners();
            jiaRu.onClick.AddListener(() => callback());
        }
    }
}