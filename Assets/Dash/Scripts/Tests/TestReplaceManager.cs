using System.Collections;
using Dash.Scripts.Setting;
using Dash.Scripts.Gameplay.View;
using Dash.Scripts.UIManager;
using Photon.Realtime;
using Spine.Unity;
using UnityEngine;

namespace Dash.Scripts.Tests
{
    public class TestReplaceManager : MonoBehaviour
    {
        public new SkeletonAnimation animation;
        public PlayerEquipsUIManager manager;
        public WeaponInfoAsset weaponInfoAsset;

        private void Start()
        {
            var list = SpineUtils.GenerateSpineReplaceInfo(weaponInfoAsset, animation.Skeleton);
            manager.Equip(list);
            Debug.Log(list.ToStringFull());
            
        }
    }
}