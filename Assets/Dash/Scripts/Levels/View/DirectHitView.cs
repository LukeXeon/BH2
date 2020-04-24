using Dash.Scripts.Levels.Config;
using Photon.Pun;
using UnityEngine;

namespace Dash.Scripts.Levels.View
{
    public class DirectHitView : WeaponView
    {
        [Header("Assets")] public AudioClip audioClip;
        [Header("Com")] public AudioSource audioSource;
        public LineRenderer shootLine;
        private ParticleSystem[] particleSystems;
        private static readonly int KAIQIANG = Animator.StringToHash("kaiqiang");
        private Ray shootRay;
        private RaycastHit shootHit;

        private void Awake()
        {
            audioSource.clip = audioClip;
            particleSystems = GetComponentsInChildren<ParticleSystem>(true);
            foreach (var system in particleSystems)
            {
                system.Stop();
            }
        }

        public override void OnFire()
        {
            if (isMine)
            {
                var transform1 = shootLine.transform;
                shootRay.origin = transform1.position;
                shootRay.direction = transform1.forward;
                if (Physics.Raycast(shootRay, out shootHit, 100f, targetMask))
                {
                    var actorView = shootHit.collider.GetComponent<ActorView>();
                    var index = LevelConfigManager.currentWeaponIndex;
                    var info = LevelConfigManager.weaponInfos[index];
                    var value = info.Item2.gongJiLi;
                    actorView.photonView.RPC(nameof(actorView.OnDamage), RpcTarget.All, value);
                }

                RpcInPlayerView(nameof(OnSync), Vector3.zero);
            }
            else
            {
                Debug.Log("逻辑有错误");
            }
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