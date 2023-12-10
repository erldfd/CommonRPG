using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EAIState
{
    None,
    Chase,
    Patrol,
    Attack
}

public class AIController : MonoBehaviour
{
    /// <summary>
    /// arg : Transform targetTranform
    /// </summary>
    public event Action<Transform> OnAttack = null;

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
    
    [SerializeField]
    private float detectRange = 10;
    private float elapsedTime_Detect = 0;

    [SerializeField]
    private float patrolRadius = 5;

    [SerializeField]
    private float patrolInterval = 3;
    private float elapsedTime_Patrol = 0;

    [SerializeField]
    private float attackInterval = 3;
    private float elapsedTime_Attack = 0;

    [SerializeField]
    private Transform destination = null;
    private NavMeshAgent agent = null;

    private EAIState currentAIState = EAIState.None;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        Debug.Assert(agent);
        Debug.Assert(destination);
    }

    private void Update()
    {
        if (isAIActivated == false)
        {
            return;
        }

        Detect();
        Chase();
        Patrol();
        Attack();
    }

    private bool TryDetectPlayer()
    {
        //temp code
       float distanceToPlayer = Vector3.Distance(tempTargetTransform.position, transform.position);

        return (distanceToPlayer <= detectRange);
    }

    private void Detect()
    {
        elapsedTime_Detect += Time.deltaTime;

        if (elapsedTime_Detect < detectInterval) 
        {
            return;
        }

        elapsedTime_Detect = 0;

        if (currentAIState == EAIState.Attack) 
        {
            Debug.Log($"In Detect : {currentAIState}");
            return;
        }

        if (TryDetectPlayer()) 
        {
            currentAIState = EAIState.Chase;
            elapsedTime_Patrol = 0;
        }
        else
        {
            currentAIState = EAIState.Patrol;
        }
    }

    private void Chase()
    {
        if (currentAIState != EAIState.Chase) 
        {
            return;
        }

        agent.SetDestination(tempTargetTransform.position);

        float actualRemainingDistance = Vector3.Distance(tempTargetTransform.position, transform.position);
        if (actualRemainingDistance > agent.stoppingDistance) 
        {
            return;
        }
        //Debug.Log($"remainingDistance : {agent.remainingDistance}, actual remainingDistance : {Vector3.Distance(tempTargetTransform.position, transform.position)}, stoppingDistance : {agent.stoppingDistance}, destination : {tempTargetTransform.position}, currentPos : {transform.position}");
        currentAIState = EAIState.Attack;
    }

    private void Patrol()
    {
        if (currentAIState != EAIState.Patrol) 
        {
            return;
        }

        if (agent.remainingDistance > agent.stoppingDistance)
        {
            return;
        }

        elapsedTime_Patrol += Time.deltaTime;

        if (elapsedTime_Patrol < patrolInterval)
        {
            return;
        }

        elapsedTime_Patrol = 0;

        Vector3 targetPos = transform.position;

        float xPosInCircleRadius = UnityEngine.Random.Range(-patrolRadius, patrolRadius);
        float patrolRadiusZ = Mathf.Sqrt((patrolRadius + xPosInCircleRadius) * (patrolRadius - xPosInCircleRadius));
        float zPosInCircleRadius = UnityEngine.Random.Range(-patrolRadiusZ, patrolRadiusZ);

        targetPos.x = xPosInCircleRadius;
        targetPos.z = zPosInCircleRadius;

        agent.SetDestination(targetPos);
        Debug.Log($"PatrolPos : {targetPos}");
    }

    private void Attack()
    {
        elapsedTime_Attack += Time.deltaTime;

        if (currentAIState != EAIState.Attack) 
        {
            if (elapsedTime_Attack > attackInterval) 
            {
                elapsedTime_Attack = attackInterval;
            }

            return;
        }

        if (elapsedTime_Attack < attackInterval)
        {
            return;
        }

        //TODO::Attack Player
        Debug.Log("Attack Player");
        OnAttack.Invoke(tempTargetTransform);

        elapsedTime_Attack = 0;
        currentAIState = EAIState.None;
    }
}
