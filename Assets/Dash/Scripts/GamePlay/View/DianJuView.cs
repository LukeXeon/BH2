using UnityEngine;

namespace Dash.Scripts.GamePlay.View
{
    public class DianJuView : WeaponView
    {
        private int flipX;
        public AudioClip audioClip;
        public Transform rayBegin;
        
        protected override void OnFire()
        {
            base.OnFire();
            
        }
        
        
        
    }
}