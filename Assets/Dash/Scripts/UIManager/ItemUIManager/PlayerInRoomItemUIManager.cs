using Dash.Scripts.Config;
using Dash.Scripts.Levels.View;
using Photon.Realtime;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace Dash.Scripts.UIManager.ItemUIManager
{
    public class PlayerInRoomItemUIManager : MonoBehaviour
    {
        private int currentPlayerId;
        private int currentWeaponTypeId;
        public TextMeshProUGUI displayName;
        public GameObject isMaster;
        public GameObject isReady;
        public PlayerEquipsUIManager playerEquipsUiManager;
        public SkeletonAnimation skeletonAnimation;

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
                skeletonAnimation.skeletonDataAsset = GameConfigManager.playerTable[(int) playerTypeId].skel;
                skeletonAnimation.Initialize(true);
                if (weaponTypeId != null && (int) weaponTypeId != currentWeaponTypeId)
                {
                    currentWeaponTypeId = (int) weaponTypeId;
                    var weapon = GameConfigManager.weaponTable[(int) weaponTypeId];
                    var list = SpineUtils.GenerateSpineReplaceInfo(weapon, skeletonAnimation.Skeleton);
                    playerEquipsUiManager.Equip(list);

                    skeletonAnimation.AnimationState.SetAnimation(0, weapon.weaponType.matchName + "_idle", true);
                }
            }

            displayName.text = displayNameValue as string ?? "...";
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