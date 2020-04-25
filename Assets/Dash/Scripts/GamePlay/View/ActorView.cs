using System;
using Dash.Scripts.GamePlay.UIManager;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace Dash.Scripts.GamePlay.View
{
    public abstract class ActorView : MonoBehaviour
    {
        public OnActorDamageEvent onActorDamageEvent;
        internal PhotonView photonView;

        protected virtual void Awake()
        {
            if (onActorDamageEvent == null) onActorDamageEvent = new OnActorDamageEvent();

            photonView = GetComponent<PhotonView>();
            onActorDamageEvent.AddListener(FindObjectOfType<LevelUIManager>().OnShowDamage);
        }

        [PunRPC]
        public virtual void OnDamage(int value)
        {
        }

        protected virtual void OnDestroy()
        {
        }

        [Serializable]
        public class OnActorDamageEvent : UnityEvent<ActorView, int>
        {
        }
    }
}