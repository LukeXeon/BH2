using UnityEngine;

namespace Dash.Scripts.Levels.View
{
    public class DirectHitView : WeaponView
    {
        public LineRenderer shootLine;
        private ParticleSystem[] particleSystems;
        private static readonly int KAIQIANG = Animator.StringToHash("kaiqiang");
        
        

        private void Awake()
        {
            particleSystems = GetComponentsInChildren<ParticleSystem>(true);
            foreach (var system in particleSystems)
            {
                system.Stop();
            }
        }

        public override void OnFire()
        {
            
            RpcInPlayerView(nameof(OnSync), Vector3.zero);
        }
        

        public void OnSync(Vector3 vector3)
        {
            foreach (var system in particleSystems)
            {
                system.Simulate(0);
                system.Play();
            }
        }
    }
}