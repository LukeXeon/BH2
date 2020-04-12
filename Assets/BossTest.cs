using System;
using UnityEngine;
using UnityEngine.AI;

namespace X
{
    public class BossTest : MonoBehaviour
    {
        public NavMeshAgent agent;
        public Transform target;
        public Animator animator;
        public float distance;
        private static readonly int IS_RUN = Animator.StringToHash("isRun");

        private void Awake()
        {
        }

        // Update is called once per frame
        void Update()
        {
            var position = target.position;
            agent.destination = position;

            animator.SetBool(IS_RUN, agent.velocity != Vector3.zero);
        }
    }
    
}