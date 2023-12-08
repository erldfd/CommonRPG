using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    [SerializeField]
    private Transform tempTargetTransform = null;

    [SerializeField]
    private bool isAIActivated = false;
    public bool IsAIActivated
    {
        get { return isAIActivated; }
        set { isAIActivated = value; }
    }

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

    [SerializeField]
    private float detectInterval = 1;
    private float elapsedTime = 0;

    [SerializeField]
    private float detectRange = 10;

    [SerializeField]
    private float patrolRadius = 5;

    [SerializeField]
    private Transform destination = null;
    private NavMeshAgent agent = null;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        Debug.Assert(agent);
        Debug.Assert(destination);
        //Vector3 newDestination = destination.position;
        //newDestination.y = transform.position.y;
        //agent.SetDestination(newDestination);
    }

    private void Update()
    {
        if (isAIActivated == false)
        {
            return;
        }

        elapsedTime += Time.deltaTime;

        if (elapsedTime < detectInterval)
        {
            return;
        }

        elapsedTime = 0;

        bool isSucceededDetectTarget = TryDetectPlayer();
        Vector3 targetPos = transform.position;
        if (isSucceededDetectTarget)
        {
            targetPos = tempTargetTransform.position;
            Debug.Log($"Target Pos : {targetPos}");
        }
        else
        {
            // if agent didnt arrive at previous destination, dont set destination.
            if (agent.remainingDistance > agent.stoppingDistance) 
            {
                return;
            }

            //patrol
            float xPosInCircleRadius = Random.Range(-patrolRadius, patrolRadius);
            float patrolRadiusZ = Mathf.Sqrt((patrolRadius + xPosInCircleRadius) * (patrolRadius - xPosInCircleRadius));
            float zPosInCircleRadius = Random.Range(-patrolRadiusZ, patrolRadiusZ);

            targetPos.x = xPosInCircleRadius;
            targetPos.z = zPosInCircleRadius;
            Debug.Log($"Patrol Pos : {targetPos}"); 
        }

        agent.SetDestination(targetPos);

        if (isSucceededDetectTarget && agent.remainingDistance <= agent.stoppingDistance)
        {
            Debug.Log("Attack Player");
            return;
        }
    }

    private void AIMoveTo(Vector3 target)
    {
        if (agent.isOnNavMesh == false) 
        {
            Debug.LogWarning($"{agent.name} is not on navMesh");
            return;
        }

        agent.SetDestination(target);
    }

    private bool TryDetectPlayer()
    {
        //temp code
       float distanceToPlayer = Vector3.Distance(tempTargetTransform.position, transform.position);

        return (distanceToPlayer <= detectRange);
    }
}
