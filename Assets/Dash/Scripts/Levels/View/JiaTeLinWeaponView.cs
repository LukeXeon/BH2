using UnityEngine;

namespace Dash.Scripts.Levels.View
{
    public class JiaTeLinWeaponView : WeaponView
    {
        public Transform muzzleFlare;
        public Transform shellLocator;
        public Transform bulletLocator;
        [Header("Assets")] public GameObject shellPrefab;
        public GameObject bulletPrefab;
        private ParticleSystem[] particleSystems;

        private void Awake()
        {
            particleSystems = GetComponentsInChildren<ParticleSystem>(true);
        }

        public override void FireBullet()
        {
            foreach (var system in particleSystems)
            {
                system.Simulate(0);
                system.Play();
            }

            Instantiate(shellPrefab, shellLocator.position, shellLocator.rotation);
            var go = Instantiate(bulletPrefab, bulletLocator.position, bulletLocator.rotation);
            var rigid = go.GetComponent<Rigidbody>();
            rigid.AddForce(bulletLocator.forward * 2800);
            Destroy(go, 3);
        }
    }
}