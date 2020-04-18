using System.Collections.Generic;
using System.Linq;
using Dash.Scripts.Config;
using Spine;
using Spine.Unity.AttachmentTools;
using UnityEngine;

namespace Dash.Scripts.GamePlay.View
{
    public static class SpineUtils
    {
        private static Dictionary<WeaponInfoAsset, List<SpineReplaceInfo>> cachedAttachments =
            new Dictionary<WeaponInfoAsset, List<SpineReplaceInfo>>();
        
        public static List<SpineReplaceInfo> GenerateSpineReplaceInfo(WeaponInfoAsset item, Skeleton asset)
        {
            List<SpineReplaceInfo> li = new List<SpineReplaceInfo>();
            foreach (var s in item.slots)
            {
                var ss = GenerateSpineReplaceInfo(item.weaponType.matchName, s, asset);
                li.AddRange(ss);
            }

            return li;
        }

        private static List<SpineReplaceInfo> GenerateSpineReplaceInfo(
            string type,
            WeaponInfoAsset.SlotItem item,
            Skeleton asset
        )
        {
            int slotIndex = asset.FindSlotIndex(item.name);
            return item.attachments
                .Select(aItem => GenerateSpineReplaceInfo(type, aItem, asset, slotIndex)).ToList();
        }

        private static SpineReplaceInfo GenerateSpineReplaceInfo(
            string type,
            WeaponInfoAsset.AttachmentItem item,
            Skeleton asset,
            int slotIndex
        )
        {
            var sprite = item.sprite;
            var templateSkin = asset.Data.FindSkin("default");
            RegionAttachment templateAttachment = (RegionAttachment) templateSkin.GetAttachment(slotIndex, item.name);
            var m = new Material(Shader.Find("Spine/Skeleton"));
            var h = sprite.texture.height;
            var w = sprite.texture.width;
            var s = Sprite.Create(sprite.texture, new Rect
                {
                    x = 0,
                    y = 0,
                    width = w,
                    height = h
                }, GameGlobalInfoManager.weaponTypeTable[type]?.offset ?? new Vector2(0.5f, 0.5f),
                100,
                1,
                SpriteMeshType.FullRect
            );
            var attachment = (RegionAttachment) templateAttachment.GetRemappedClone(s, m, useOriginalRegionSize: true);
            return new SpineReplaceInfo
            {
                attachment = attachment,
                name = item.name,
                slotIndex = slotIndex
            };
        }
    }
}