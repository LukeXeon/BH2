using System;
using System.Linq;
using Photon.Pun;
using UnityEngine;

namespace Dash.Scripts.GamePlay.View
{
    public class NpcView : ActorView
    {
        public NpcConfig config;
        private Collider[] targetCollider;
        protected int targetLayerMask;
        [HideInInspector]
        public PhotonView target;

        protected override void Awake()
        {
            base.Awake();
            targetLayerMask = LayerMask.GetMask("Player");
            targetCollider = new Collider[1];
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
            Physics.OverlapSphereNonAlloc(
                transform.position,
                config.findPlayerRange,
                targetCollider,
                targetLayerMask
            );
            var c = targetCollider.FirstOrDefault();
            if (c != null)
            {
                target = c.GetComponent<PhotonView>();
            }
            else
            {
                target = null;
            }
        }
    }
}