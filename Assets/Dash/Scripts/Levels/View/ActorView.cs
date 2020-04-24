using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace Dash.Scripts.Levels.View
{
    public abstract class ActorView : MonoBehaviour
    {
        public OnActorDamageEvent onActorDamageEvent;
        public PhotonView photonView;

        [Serializable]
        public class OnActorDamageEvent : UnityEvent<int>
        {
        }

        protected virtual void Awake()
        {
            if (onActorDamageEvent == null)
            {
                onActorDamageEvent = new OnActorDamageEvent();
            }

            photonView = GetComponent<PhotonView>();
        }


        [PunRPC]
        public virtual void OnDamage(int value)
        {
            onActorDamageEvent.Invoke(value);
        }

        protected virtual void OnDestroy()
        {
        }
    }
}