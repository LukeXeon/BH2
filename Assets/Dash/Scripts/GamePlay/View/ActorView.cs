using System;
using Dash.Scripts.Gameplay.UIManager;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace Dash.Scripts.Gameplay.View
{
    public abstract class ActorView : MonoBehaviour
    {
        public ActorDamageEvent onActorDamageEvent;
        public ActorEvent onActorDie;
        internal PhotonView photonView;
        public int flipX;
        public bool isDie;

        [Serializable]
        public class ActorEvent : UnityEvent
        {
        }

        protected virtual void Awake()
        {
            flipX = -1;
            if (onActorDie == null) onActorDie = new ActorEvent();
            if (onActorDamageEvent == null) onActorDamageEvent = new ActorDamageEvent();
            photonView = GetComponent<PhotonView>();
            onActorDamageEvent.AddListener(FindObjectOfType<LevelUIManager>().OnShowDamage);
        }

        [PunRPC]
        public virtual void OnDamage(int viewId, int value)
        {
        }

        protected virtual void OnDestroy()
        {
        }

        [Serializable]
        public class ActorDamageEvent : UnityEvent<ActorView, int>
        {
        }
    }
}