using System;
using UnityEngine;
using UnityEngine.Events;

namespace Dash.Scripts.Gameplay.Levels
{
    public class TriggerEvent : MonoBehaviour
    {
        public OnTriggerEvent onTriggerEnter;
        public OnTriggerEvent onTriggerExit;

        
        [Serializable]
        public class OnTriggerEvent : UnityEvent<Collider>
        {
            
        }

        private void OnTriggerEnter(Collider other)
        {
            onTriggerEnter?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            onTriggerExit?.Invoke(other);
        }
    }
}