using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CommonRPG
{
    public abstract class AAIController : MonoBehaviour
    {
        protected Transform targetTransform = null;

        [SerializeField]
        protected bool isAIActivated = false;
        public bool IsAIActivated
        {
            get { return isAIActivated; }
            set
            {
                isAIActivated = value;

                if (agent && agent.isOnNavMesh)
                {
                    agent.isStopped = (value == false);
                }
            }
        }

        public virtual float CurrentSpeed
        {
            get
            {
                return 0;
            }
        }

        protected NavMeshAgent agent = null;

        protected virtual void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            Debug.Assert(agent);
        }
    }
}
