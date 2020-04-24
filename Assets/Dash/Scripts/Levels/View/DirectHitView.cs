using System;
using Dash.Scripts.Levels.Config;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dash.Scripts.Levels.View
{
    public class DirectHitView : WeaponView
    {
        [Header("Assets")] public AudioClip audioClip;
        public Transform[] shootLocators;
        [Header("Com")] public AudioSource audioSource;
        public LineRenderer shootLine;
        private ParticleSystem[] particleSystems;
        private static readonly int KAIQIANG = Animator.StringToHash("kaiqiang");

        private float timer;
        private const float effectsDisplayTime = 0.2f;

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
                var locator = shootLocators[Random.Range(0, shootLocators.Length)];
                var position = locator.position;
                var index = LocalPlayerDynamicInfo.currentWeaponIndex;
                var (info, data) = LocalPlayerInfo.weaponInfos[index];
                var range = info.sheCheng;
                Ray shootRay = new Ray
                {
                    origin = position,
                    direction = Vector3.right * Mathf.Sign(transform.localScale.x)
                };
                if (Physics.Raycast(shootRay, out var shootHit, range, targetMask))
                {
                    var actorView = shootHit.collider.GetComponent<ActorView>();
                    Debug.Log(actorView.gameObject);
                    if (actorView)
                    {
                        Debug.Log(actorView.gameObject);
                        var value = data.gongJiLi;
                        actorView.photonView.RPC(nameof(actorView.OnDamage), RpcTarget.All, value);
                        RpcInPlayerView(nameof(OnSync), position, shootHit.point);
                        return;
                    }
                }

                RpcInPlayerView(nameof(OnSync), position,
                    Mathf.Sign(transform.localScale.x) * range * Vector3.right);
            }
            else
            {
                Debug.LogError("客户端逻辑有错误");
            }
        }

        private void Update()
        {
            timer += Time.deltaTime;
            var index = LocalPlayerDynamicInfo.currentWeaponIndex;
            var info = LocalPlayerInfo.weaponInfos[index];
            var timeBetweenBullets = Mathf.Min(1f / info.Item1.sheShu, 0.15f);
            if (timer >= timeBetweenBullets * effectsDisplayTime)
            {
                shootLine.gameObject.SetActive(false);
            }
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
            shootLine.SetPosition(0, vector1);
            shootLine.SetPosition(1, vector2);
        }
    }
}