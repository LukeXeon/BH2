using System;
using Dash.Scripts.Config;
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

        public void Apply(string name, int typeId, int[] iconTypeIds, Action callback)
        {
            roomByUser.name = name;
            guanQiaMing.text = GameGlobalInfoManager.guanQiaInfoTable[typeId]?.name ?? "";
            for (var i = 0; i < iconTypeIds.Length; i++)
            {
                playerItems[i].sprite = GameGlobalInfoManager.playerTable[iconTypeIds[i]].icon;
            }

            jiaRu.onClick.AddListener(() => callback());
        }
    }
}
