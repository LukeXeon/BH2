using System.Collections.Generic;
using Dash.Scripts.Levels.View;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Dash.Scripts.UIManager
{
    public class PlayerEquipsUIManager : MonoBehaviour
    {
        public SkeletonAnimation skeletonAnimation;

        [SpineSkin] public string templateSkinName;
        

        public Material runtimeMaterial;
        public Texture2D runtimeAtlas;

        private void Awake()
        {
            if (skeletonAnimation == null)
            {
                skeletonAnimation = GetComponent<SkeletonAnimation>();
            }
        }


        public void Equip(List<SpineReplaceInfo> info)
        {
            Skin equipsSkin = new Skin("Equips");
            var templateSkin = skeletonAnimation.Skeleton.Data.DefaultSkin;
            if (templateSkin != null)
            {
                equipsSkin.AddSkin(templateSkin);
            }
            foreach (var spineReplaceInfo in info)
            {
                equipsSkin.SetAttachment(spineReplaceInfo.slotIndex, spineReplaceInfo.name, spineReplaceInfo.attachment);
            }
            skeletonAnimation.Skeleton.SetSkin(equipsSkin);
            skeletonAnimation.Skeleton.SetSlotsToSetupPose();
            skeletonAnimation.AnimationState.Apply(skeletonAnimation.Skeleton);
        }
    }
}