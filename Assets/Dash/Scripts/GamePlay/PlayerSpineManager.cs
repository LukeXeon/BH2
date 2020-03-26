using Dash.Scripts.Assets;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Dash.Scripts.GamePlay
{
    [RequireComponent(typeof(SkeletonAnimation))]
    public class PlayerSpineManager : MonoBehaviour
    {
        private SkeletonAnimation skeletonAnimation;
        private MainState? previousMainState;
        private string previousWeaponTypeName;
        public MainState mainState;
        public WeaponType weaponType = WeaponType.QingShouQiang;

        public enum FaceState
        {
            None,
            Hit
        }

        public enum MainState
        {
            Idle,
            Run
        }

        private void Awake()
        {
            skeletonAnimation = GetComponent<SkeletonAnimation>();
        }

        public bool flip
        {
            get { return skeletonAnimation.Skeleton.FlipX; }
            set { skeletonAnimation.Skeleton.FlipX = value; }
        }

        private void Update()
        {
            var myWeaponTypeName = WeaponTypeEx.weaponNames[this.weaponType];
            var newMainState = mainState;

            if (newMainState != previousMainState || myWeaponTypeName != previousWeaponTypeName)
            {
                string myStateName;
                if (mainState == MainState.Idle)
                {
                    myStateName = myWeaponTypeName + "_idle";
                }
                else
                {
                    myStateName = myWeaponTypeName + "_run";
                    skeletonAnimation.AnimationState.SetEmptyAnimation(1, 0.2f);
                }

                var track = skeletonAnimation.AnimationState.SetAnimation(0, myStateName, true);
                track.MixDuration = 0.2f;
                track.MixBlend = MixBlend.Replace;
                previousMainState = newMainState;
            }

            previousWeaponTypeName = myWeaponTypeName;
        }

        public void TaoQiang()
        {
            var myWeaponTypeName = WeaponTypeEx.weaponNames[this.weaponType];
            var myStateName = myWeaponTypeName;
            if (mainState == MainState.Run)
            {
                myStateName += "_run";
            }
            var track = skeletonAnimation.AnimationState.SetAnimation(1, myStateName + "_taoqiang",
                false);
//            track.MixBlend = MixBlend.Replace;
            track.AttachmentThreshold = 100f;
//            track.MixDuration = 0f;
            var empty = skeletonAnimation.state.AddEmptyAnimation(1, 0.2f, 0);
            //empty.AttachmentThreshold = 1f;
            //empty.MixBlend = MixBlend.Replace;
        }

        public void KaiQiang()
        {
            var myWeaponTypeName = WeaponTypeEx.weaponNames[this.weaponType];
            var myStateName = myWeaponTypeName;
            if (mainState == MainState.Run)
            {
                myStateName += "_run";
            }
            var track = skeletonAnimation.AnimationState.SetAnimation(1, myStateName + "_kaiqiang",
                false);
            track.AttachmentThreshold = 100f;
            var empty = skeletonAnimation.state.AddEmptyAnimation(1, 0.2f, 0);
            //empty.AttachmentThreshold = 1f;
        }
    }
}