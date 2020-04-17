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


        public void Equip(int slotIndex, string attachmentName, Attachment attachment)
        {
            Skin equipsSkin = new Skin("Equips");
            var templateSkin = skeletonAnimation.Skeleton.Data.DefaultSkin;
            if (templateSkin != null)
            {
                equipsSkin.AddSkin(templateSkin);
            }
            equipsSkin.SetAttachment(slotIndex, attachmentName, attachment);
            skeletonAnimation.Skeleton.SetSkin(equipsSkin);
            RefreshSkeletonAttachments();
        }

        void RefreshSkeletonAttachments()
        {
            skeletonAnimation.Skeleton.SetSlotsToSetupPose();
            skeletonAnimation.AnimationState.Apply(skeletonAnimation.Skeleton); //skeletonAnimation.Update(0);
        }
    }
}