using Dash.Scripts.Core;
using Dash.Scripts.GamePlay.Setting;
using Photon.Pun;
using UnityEngine;

namespace Dash.Scripts.GamePlay.View
{
    public class PlaceBatteryView : WeaponView
    {
        public GuidIndexer batteryView;
        public Transform locator;

        protected override void OnFire()
        {
            var go = PhotonNetwork.Instantiate(batteryView.guid, locator.position, locator.rotation,
                data: new object[] {transform.localScale.x, 5});
            var view = go.GetComponent<BatteryView>();
            view.Initialize(LocalPlayer.gongJiLi);
        }
    }
}