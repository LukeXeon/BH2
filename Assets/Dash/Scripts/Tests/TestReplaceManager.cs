using System.Collections;
using Dash.Scripts.Config;
using Dash.Scripts.Levels.View;
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
        public WeaponView OnFire;

        private IEnumerator Start()
        {
            var w = GameConfigManager.weaponTable[0];
            var list = SpineUtils.GenerateSpineReplaceInfo(w, animation.Skeleton);
            manager.Equip(list);
            Debug.Log(list.ToStringFull());
            while (true)
            {
                yield return new WaitForSeconds(0.1f);
                OnFire.OnFire();
            }
        }
    }
}