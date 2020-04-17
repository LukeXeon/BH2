﻿using Dash.Scripts.Config;
using Dash.Scripts.GamePlay.View;
using Photon.Realtime;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace Dash.Scripts.UIManager.ItemUIManager
{
    public class PlayerInRoomItemUIManager : MonoBehaviour
    {
        public GameObject isReady;
        public GameObject isMaster;
        public TextMeshProUGUI displayName;
        public SkeletonAnimation skeletonAnimation;
        public PlayerEquipsUIManager playerEquipsUiManager;
        private int currentWeaponTypeId;
        private int currentPlayerId;

        public void Clear()
        {
            skeletonAnimation.gameObject.SetActive(false);
            isReady.SetActive(false);
            isMaster.SetActive(false); 
            displayName.text = null;
            currentWeaponTypeId = int.MinValue;
            currentPlayerId = int.MinValue;
        }

        public void Apply(Player player)
        {
            skeletonAnimation.gameObject.SetActive(true);
            player.CustomProperties.TryGetValue("displayName", out var displayNameValue);
            player.CustomProperties.TryGetValue("isReady", out var isReadyValue);
            player.CustomProperties.TryGetValue("playerTypeId", out var playerTypeId);
            player.CustomProperties.TryGetValue("weaponTypeId", out var weaponTypeId);
            if (playerTypeId != null && (int) playerTypeId != currentPlayerId)
            {
                currentPlayerId = (int) playerTypeId;
                skeletonAnimation.skeletonDataAsset = GameGlobalInfoManager.playerTable[(int) playerTypeId].skel;
                skeletonAnimation.Initialize(true);
                if (weaponTypeId != null && (int) weaponTypeId != currentWeaponTypeId)
                {
                    currentWeaponTypeId = (int) weaponTypeId;
                    var weapon = GameGlobalInfoManager.weaponTable[(int) weaponTypeId];
                    var list = SpineUtils.GenerateSpineReplaceInfo(weapon, skeletonAnimation.Skeleton);
                    foreach (var item in list)
                    {
                        playerEquipsUiManager.Equip(item.slotIndex, item.name, item.attachment);
                    }

                    skeletonAnimation.AnimationState.SetAnimation(0, weapon.weaponType.matchName + "_idle", true);
                }
            }

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