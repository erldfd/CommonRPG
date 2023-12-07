using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    [SerializeField]
    private Transform destination = null;
    private NavMeshAgent agent = null;

    public float CurrentSpeed
    {
        get 
        {
            float currentSpeed = 0;

            if (agent.isOnNavMesh && agent.stoppingDistance < agent.remainingDistance) 
            {
                currentSpeed = agent.speed;
            }

            return currentSpeed; 
        }
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        Debug.Assert(agent);
        Debug.Assert(destination);
        Vector3 newDestination = destination.position;
        newDestination.y = transform.position.y;
        agent.SetDestination(newDestination);
    }

    private void Update()
    {
        agent.SetDestination(destination.position);
    }

}
