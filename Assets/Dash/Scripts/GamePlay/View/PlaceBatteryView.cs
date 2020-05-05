using Dash.Scripts.Core;
using Dash.Scripts.GamePlay.Config;
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
                data: new object[] {transform.localScale.x});
            var view = go.GetComponent<BatteryView>();
            view.StartWork(LocalPlayer.gongJiLi, 5);
        }
    }
}