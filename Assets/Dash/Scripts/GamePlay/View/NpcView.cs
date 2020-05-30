using System;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dash.Scripts.Gameplay.View
{
    public class NpcView : ActorView
    {
        public int hp;
        public NpcConfig config;
        private Collider[] targetCollider;
        protected int targetLayerMask;
        [HideInInspector] public PhotonView target;

        public ActorView targetActor;

        protected override void Awake()
        {
            base.Awake();
            hp = Mathf.Max(config.shengMingZhi, 1);
            targetLayerMask = LayerMask.GetMask("Player");
            targetCollider = new Collider[20];
        }

        [Serializable]
        public struct NpcConfig
        {
            public float findPlayerRange;
            public int gongJiLi;
            public int fangYuLi;
            public int shengMingZhi;
            public float moveSpeed;
        }

        protected void RequestTargetView()
        {
            var count = Physics.OverlapSphereNonAlloc(
                transform.position,
                config.findPlayerRange,
                targetCollider,
                targetLayerMask
            );
            Collider c;
            do
            {
                var index = Random.Range(0, count);
                c = targetCollider[index];
                if (c)
                {
                    var Actor = c.GetComponent<ActorView>();
                    if (c.gameObject.CompareTag("Player") && Actor && !Actor.isDie)
                    {
                        target = Actor.photonView;
                        targetActor = Actor;
                        return;
                    }
                }
            } while (c == null);


            targetActor = null;
            target = null;
        }
    }
}