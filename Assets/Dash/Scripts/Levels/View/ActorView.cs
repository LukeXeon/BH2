using System;
using Dash.Scripts.Levels.UIManager;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace Dash.Scripts.Levels.View
{
    public abstract class ActorView : MonoBehaviour
    {
        public OnActorDamageEvent onActorDamageEvent;
        internal PhotonView photonView;

        [Serializable]
        public class OnActorDamageEvent : UnityEvent<Transform, int>
        {
        }

        protected virtual void Awake()
        {
            if (onActorDamageEvent == null)
            {
                onActorDamageEvent = new OnActorDamageEvent();
            }

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
    }
}