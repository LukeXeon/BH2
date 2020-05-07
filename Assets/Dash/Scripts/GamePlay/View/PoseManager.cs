using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Dash.Scripts.Setting;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Dash.Scripts.GamePlay.View
{
    public class PoseManager : MonoBehaviour
    {
        private static readonly ConditionalWeakTable<RuntimeAnimatorController, Dictionary<string, AnimationClip>>
            clipsCache
                = new ConditionalWeakTable<RuntimeAnimatorController, Dictionary<string, AnimationClip>>();

        private readonly Dictionary<WeaponInfoAsset, Skin> skinCache = new Dictionary<WeaponInfoAsset, Skin>();

        private readonly List<KeyValuePair<AnimationClip, AnimationClip>> temp =
            new List<KeyValuePair<AnimationClip, AnimationClip>>(10);

        [Header("Display")] public Animator animator;
        [Header("Assets")] public RuntimeAnimatorController framework;

        public float shootSpeed = 1;
        public SkeletonMecanim skeletonMecanim;
        public RuntimeAnimatorController source;

        private static Dictionary<string, AnimationClip> GetClips(RuntimeAnimatorController controller)
        {
            return clipsCache.GetValue(controller,
                key => { return key.animationClips.ToDictionary(i => i.name, i => i); });
        }

        private AnimatorOverrideController GetController(WeaponInfoAsset weaponInfoAsset)
        {
            var weaponType = weaponInfoAsset.weaponType.matchName;
            var controller = new AnimatorOverrideController(framework);
            var sourceClips = GetClips(source);
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
            temp.Add(new KeyValuePair<AnimationClip, AnimationClip>(controller["temp_hit"], hitClip));
            temp.Add(new KeyValuePair<AnimationClip, AnimationClip>(controller["temp_run_hurt"], runHurtClip));
            temp.Add(new KeyValuePair<AnimationClip, AnimationClip>(controller["temp_hurt"], hurtClip));
            temp.Add(new KeyValuePair<AnimationClip, AnimationClip>(controller["temp_run"], runClip));
            temp.Add(new KeyValuePair<AnimationClip, AnimationClip>(controller["temp_idle"], idleClip));
            if (weaponInfoAsset.weaponType.canLianShe)
                temp.Add(new KeyValuePair<AnimationClip, AnimationClip>(controller["temp_kaiqiang"],
                    kaiQiangClip));
            else
                temp.Add(new KeyValuePair<AnimationClip, AnimationClip>(controller["temp_kaiqiang_single"],
                    kaiQiangClip));

            temp.Add(new KeyValuePair<AnimationClip, AnimationClip>(controller["temp_taoqiang"], taoQiangClip));
            if (weaponInfoAsset.weaponType.canLianShe)
                temp.Add(new KeyValuePair<AnimationClip, AnimationClip>(controller["temp_run_kaiqiang"],
                    runKaiQiangClip));
            else
                temp.Add(new KeyValuePair<AnimationClip, AnimationClip>(controller["temp_run_kaiqiang_single"],
                    runKaiQiangClip));

            temp.Add(new KeyValuePair<AnimationClip, AnimationClip>(controller["temp_run_taoqiang"], runTaoQiangClip));
            controller.ApplyOverrides(temp);
            temp.Clear();
            if (weaponInfoAsset.weaponType.canLianShe)
            {
                shootSpeed = 1;
            }
            else
            {
                var time = 1f / weaponInfoAsset.sheSu;
                shootSpeed = kaiQiangClip.length / time;
            }

            return controller;
        }

        public void SetPose(WeaponInfoAsset weaponInfoAsset)
        {
            animator.runtimeAnimatorController = GetController(
                weaponInfoAsset
            );
            skinCache.TryGetValue(weaponInfoAsset, out var skin);
            if (skin != null)
            {
                skeletonMecanim.Skeleton.SetSkin(skin);
                skeletonMecanim.Skeleton.SetSlotsToSetupPose();
                skeletonMecanim.Translator.Apply(skeletonMecanim.Skeleton);
            }
            else
            {
                var list = SpineUtils.GenerateSpineReplaceInfo(weaponInfoAsset, skeletonMecanim.Skeleton);
                var equipsSkin = new Skin("Equips");
                var templateSkin = skeletonMecanim.Skeleton.Data.DefaultSkin;
                if (templateSkin != null) equipsSkin.AddSkin(templateSkin);

                foreach (var spineReplaceInfo in list)
                    equipsSkin.SetAttachment(spineReplaceInfo.slotIndex, spineReplaceInfo.name,
                        spineReplaceInfo.attachment);

                skeletonMecanim.Skeleton.SetSkin(equipsSkin);

                skeletonMecanim.Skeleton.SetSlotsToSetupPose();
                skeletonMecanim.Translator.Apply(skeletonMecanim.Skeleton);
                skinCache[weaponInfoAsset] = equipsSkin;
            }
        }
    }
}