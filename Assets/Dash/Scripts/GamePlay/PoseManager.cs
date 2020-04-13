using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Dash.Scripts.Config;
using Dash.Scripts.GamePlay.Spine;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Dash.Scripts.GamePlay
{
    public class PoseManager : MonoBehaviour
    {
        [Header("Display")] public Animator animator;
        public SkeletonMecanim skeletonMecanim;
        [Header("Assets")] public RuntimeAnimatorController framework;
        public RuntimeAnimatorController source;

        private static readonly ConditionalWeakTable<RuntimeAnimatorController, Dictionary<string, AnimationClip>>
            clipsCache
                = new ConditionalWeakTable<RuntimeAnimatorController, Dictionary<string, AnimationClip>>();

        private readonly List<KeyValuePair<AnimationClip, AnimationClip>> temp =
            new List<KeyValuePair<AnimationClip, AnimationClip>>(10);

        private static Dictionary<string, AnimationClip> GetClips(RuntimeAnimatorController controller)
        {
            return clipsCache.GetValue(controller,
                key => { return key.animationClips.ToDictionary(i => i.name, i => i); });
        }

        private AnimatorOverrideController GetController(string weaponType)
        {
            var sourceClips = GetClips(source);
            var controller = new AnimatorOverrideController(framework);
            var idleClip = sourceClips[weaponType + "_idle"];
            var runClip = sourceClips[weaponType + "_run"];
            var kaiQiangClip = sourceClips[weaponType + "_kaiqiang"];
            var taoQiangClip = sourceClips[weaponType + "_taoqiang"];
            var runKaiQiangClip = sourceClips[weaponType + "_run_kaiqiang"];
            var runTaoQiangClip = sourceClips[weaponType + "_run_taoqiang"];
            var hurtClip = sourceClips["hurt1"];
            var runHurtClip = sourceClips["run_hurt1"];
            var hitClip = sourceClips["emotion_Hit0"];
            var dieClip = sourceClips["die"];
            temp.Clear();
            temp.Add(new KeyValuePair<AnimationClip, AnimationClip>(controller["temp_die"], dieClip));
            temp.Add(new KeyValuePair<AnimationClip, AnimationClip>(controller["temp_die"], dieClip));
            temp.Add(new KeyValuePair<AnimationClip, AnimationClip>(controller["temp_hit"], hitClip));
            temp.Add(new KeyValuePair<AnimationClip, AnimationClip>(controller["temp_run_hurt"], runHurtClip));
            temp.Add(new KeyValuePair<AnimationClip, AnimationClip>(controller["temp_hurt"], hurtClip));
            temp.Add(new KeyValuePair<AnimationClip, AnimationClip>(controller["temp_run"], runClip));
            temp.Add(new KeyValuePair<AnimationClip, AnimationClip>(controller["temp_idle"], idleClip));
            temp.Add(new KeyValuePair<AnimationClip, AnimationClip>(controller["temp_kaiqiang"], kaiQiangClip));
            temp.Add(new KeyValuePair<AnimationClip, AnimationClip>(controller["temp_taoqiang"], taoQiangClip));
            temp.Add(new KeyValuePair<AnimationClip, AnimationClip>(controller["temp_run_kaiqiang"], runKaiQiangClip));
            temp.Add(new KeyValuePair<AnimationClip, AnimationClip>(controller["temp_run_taoqiang"], runTaoQiangClip));
            controller.ApplyOverrides(temp);
            temp.Clear();
            return controller;
        }

        public void SetPose(WeaponInfoAsset weaponInfoAsset)
        {
            animator.runtimeAnimatorController = GetController(weaponInfoAsset.weaponType.matchName);
            var list = SpineUtils.GenerateSpineReplaceInfo(weaponInfoAsset, skeletonMecanim.Skeleton);
            foreach (var item in list)
            {
                Equip(item.slotIndex, item.name, item.attachment);
            }
        }

        private void Equip(int slotIndex, string attachmentName, Attachment attachment)
        {
            Skin equipsSkin = new Skin("Equips");
            var templateSkin = skeletonMecanim.Skeleton.Data.DefaultSkin;
            if (templateSkin != null)
            {
                equipsSkin.AddSkin(templateSkin);
            }

            equipsSkin.SetAttachment(slotIndex, attachmentName, attachment);
            skeletonMecanim.Skeleton.SetSkin(equipsSkin);
            skeletonMecanim.Skeleton.SetSlotsToSetupPose();
            skeletonMecanim.Translator.Apply(skeletonMecanim.Skeleton);
        }
    }
}