using System;
using Dash.Scripts.GamePlay.Config;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dash.Scripts.GamePlay.View
{
    public class DirectHitView : WeaponView
    {
        private const float effectsDisplayTime = 0.2f;
        private static readonly int KAIQIANG = Animator.StringToHash("kaiqiang");
        [Header("Assets")] public AudioClip audioClip;
        private ParticleSystem[] particleSystems;
        public Transform rayBegin;
        public LineRenderer shootLine;
        public Transform[] shootLocators;
        private int flipX;

        private float timer;

        private void Awake()
        {
            particleSystems = GetComponentsInChildren<ParticleSystem>(true);
            foreach (var system in particleSystems) system.Stop();
        }

        protected override void OnFire()
        {
            base.OnFire();
            var locator = shootLocators[Random.Range(0, shootLocators.Length)];
            var position = locator.position;
            var index = LocalPlayer.weaponIndex;
            var (info, _) = PlayerConfigManager.weaponInfos[index];
            var range = info.sheCheng;
            var from = position;
            from.x = rayBegin.position.x;
            if (Physics.BoxCast(
                from,
                Vector3.one * 1.5f,
                Vector3.right * Mathf.Sign(transform.localScale.x),
                out var shootHit,
                Quaternion.identity,
                range,
                targetMask
            ))
            {
                var actorView = shootHit.collider.GetComponent<ActorView>();
                if (actorView && !actorView.isDie)
                {
                    var value = LocalPlayer.gongJiLi;
                    actorView.photonView.RPC(
                        nameof(actorView.OnDamage),
                        RpcTarget.All,
                        playerView.PhotonView.ViewID,
                        value
                    );
                    Vector3 from1;
                    var to = position;
                    to.x = actorView.transform.position.x;
                    switch (flipX)
                    {
                        case 1 when actorView.transform.position.x - locator.position.x > 0:
                        case -1 when actorView.transform.position.x - locator.position.x < 0:
                            from1 = position;
                            break;
                        default:
                            from1 = to;
                            break;
                    }

                    RpcInHost(nameof(OnSync), from1, to);
                    return;
                }
            }

            var d = position;
            d.x += Mathf.Sign(transform.localScale.x) * range;
            RpcInHost(nameof(OnSync), position, d);
        }

        private void Update()
        {
            timer += Time.deltaTime;
            var index = LocalPlayer.weaponIndex;
            var info = PlayerConfigManager.weaponInfos[index];
            var timeBetweenBullets = Mathf.Min(1f / info.Item1.sheSu, 0.15f);
            if (timer >= timeBetweenBullets * effectsDisplayTime)
            {
                shootLine.gameObject.SetActive(false);
            }
        }

        public override void SetFlipX(int x)
        {
            base.SetFlipX(x);
            if (flipX != x)
            {
                shootLine.gameObject.SetActive(false);
                foreach (var system in particleSystems)
                {
                    system.Stop();
                }
            }

            flipX = x;
        }

        private void OnEnable()
        {
            foreach (var system in particleSystems) system.Stop();
            shootLine.gameObject.SetActive(false);
        }

        public void OnSync(Vector3 vector1, Vector3 vector2)
        {
            timer = 0;
            shootLine.gameObject.SetActive(true);
            foreach (var system in particleSystems)
            {
                system.Simulate(0);
                system.Play();
            }

            var audioSource = playerView.audioView.GetOrCreateSource();
            audioSource.clip = audioClip;
            audioSource.time = 0;
            audioSource.Play();

            shootLine.SetPosition(0, vector1);
            shootLine.SetPosition(1, vector2);
        }
    }
}