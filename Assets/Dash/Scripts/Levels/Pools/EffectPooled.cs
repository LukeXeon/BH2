using UnityEngine;

namespace Dash.Scripts.Levels.Pools
{
    public class EffectPooled : MonoBehaviour, IPooled
    {
        private ParticleSystem[] particleSystems;

        private void Awake()
        {
            particleSystems = GetComponentsInChildren<ParticleSystem>(true);
        }

        public uint MaxPool => 20;
        public uint PreAlloc => 10;

        public void Reusing()
        {
            gameObject.SetActive(true);
            foreach (var system in particleSystems)
            {
                system.Simulate(0f);
                system.Play();
            }
        }

        public void Recycle()
        {
            gameObject.SetActive(false);
        }
    }
}