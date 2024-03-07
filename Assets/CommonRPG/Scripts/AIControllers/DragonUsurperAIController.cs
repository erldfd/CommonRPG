using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CommonRPG
{
    public class DragonUsurperAIController : BossAIController
    {
        public enum EAIState
        {
            None,
            Sleep,
            Walk,
            Run,
            Attack

        }

        public event Action OnWakeUpStartDelegate = null;
        public event Action OnAttackWithMouthDelegate = null;
        public event Action OnAttackWithHandDelgate = null;

        public bool IsMoving { get; private set; }

        public override float CurrentSpeed
        {
            get
            {
                float currentSpeed = base.CurrentSpeed;

                if (agent.isOnNavMesh && agent.stoppingDistance < agent.remainingDistance)
                {
                    currentSpeed = agent.speed;
                }

                return currentSpeed;
            }
        }

        [SerializeField]
        private float detectRadius_Sleep = 10;

        [SerializeField]
        private float detectInterval = 1;
        private float elapsedTime_Detect = 0;

        [SerializeField]
        private float mouthAttackRagne = 5;

        [SerializeField]
        private float mouthAttackInterval = 3;
        private float elapsedTime_MouthAttack = 0;

        [SerializeField]
        private float handAttackRange = 10;

        [SerializeField]
        private float handAttackInterval = 5;
        private float elapsedTime_HandAttack = 0;

        private EAIState currentAIState = EAIState.None;
        public EAIState CurrentAIState { get { return currentAIState; } set { currentAIState = value; } }

        private bool isAttacking = false;
        public bool IsAttacking { get { return isAttacking; } set { isAttacking = value; } }

        protected void Awake()
        {
            targetTransform = GameManager.GetPlayerCharacter().transform;

        }

        protected void Update()
        {
            if (base.IsAIActivated == false)
            {
                IsMoving = false;
                return;
            }

            if (base.currentPhase == EAIPhase.None || base.currentPhase == EAIPhase.Dead) 
            {
                IsMoving = false;
                return;
            }

            elapsedTime_Detect += Time.deltaTime;

            if (base.currentPhase == EAIPhase.Sleep)
            {
                Sleep();
            }
            else if (base.currentPhase == EAIPhase.Phase1)
            {
                RunPhase1();
            }
            else if (base.currentPhase == EAIPhase.Phase2)
            {

            }
            else if (base.currentPhase == EAIPhase.Phase3) 
            {

            }

            if (mouthAttackInterval > elapsedTime_MouthAttack) 
            {
                elapsedTime_MouthAttack += Time.deltaTime;
            }

            if (handAttackInterval > elapsedTime_HandAttack) 
            {
                elapsedTime_HandAttack += Time.deltaTime;
            }

            float distanceToTarget = Vector3.Distance(targetTransform.position, transform.position);

            if (distanceToTarget <= agent.stoppingDistance)
            {
                IsMoving = false;
            }
        }

        private void Sleep()
        {
            if (currentAIState != EAIState.Sleep) 
            {
                currentAIState = EAIState.Sleep;
                return;
            }

            if (elapsedTime_Detect < detectInterval)
            {
                return;
            }

            elapsedTime_Detect = 0;

            if (DetectPlayer(detectRadius_Sleep)) 
            {
                WakeUp();
            }
        }

        private void RunPhase1()
        {
            if (Walk(targetTransform.position)) 
            {
                if (Random.Range(0, 2) == 0) 
                {
                    AttackWithMouth();
                }
                else
                {
                    AttackWithHand();
                }
            }
        }

        /// <returns>if succeeded to detect player, return true</returns>
        private bool DetectPlayer(float radius)
        {
            Debug.Assert(targetTransform);

            return (Vector3.Distance(targetTransform.position, transform.position) <= radius);
        }

        private void WakeUp()
        {
            // need to play animation sleep -> idle
            OnWakeUpStartDelegate?.Invoke();
        }

        /// <returns> succeeded to arrive at destination?</returns>
        private bool Walk(Vector3 destination)
        {
            if (currentAIState != EAIState.Walk) 
            {
                return false;
            }

            if (IsAttacking) 
            {
                agent.isStopped = true;

                return false;
            }

            agent.isStopped = false;
            agent.SetDestination(destination);

            IsMoving = true;

            float actualRemainingDistance = Vector3.Distance(targetTransform.position, transform.position);
            if (actualRemainingDistance > agent.stoppingDistance)
            {
                return false;
            }

            currentAIState = EAIState.Attack;

            return true;
        }

        /// <returns> succeeded to arrive at destination?</returns>
        private bool Run(Vector3 destination)
        {
            if (currentAIState != EAIState.Run)
            {
                return false;
            }

            if (IsAttacking)
            {
                return false;
            }

            agent.SetDestination(destination);

            IsMoving = true;

            float actualRemainingDistance = Vector3.Distance(targetTransform.position, transform.position);
            if (actualRemainingDistance > agent.stoppingDistance)
            {
                return false;
            }

            currentAIState = EAIState.Attack;

            return true;
        }

        private void AttackWithMouth()
        {
            if (currentAIState != EAIState.Attack)
            {
                return;
            }

            if (IsAttacking)
            {
                return;
            }

            transform.LookAt(targetTransform, transform.up);

            currentAIState = EAIState.Walk;

            if (elapsedTime_MouthAttack < mouthAttackInterval) 
            {
                return;
            }

            elapsedTime_MouthAttack = 0;
            OnAttackWithMouthDelegate?.Invoke();
        }

        private void AttackWithHand()
        {
            if (currentAIState != EAIState.Attack)
            {
                return;
            }

            if (IsAttacking)
            {
                return;
            }
            transform.LookAt(targetTransform, transform.up);
            currentAIState = EAIState.Walk;

            if (elapsedTime_HandAttack < handAttackInterval)
            {
                return;
            }

            elapsedTime_HandAttack = 0;
            OnAttackWithHandDelgate?.Invoke();
        }

        

        /*
         * sleep
         * 
         * sleep -> player detect -> idle -> plase1
         * 
         * phase 1
         * 
         * 1. walk -> player in attack range(short) -> attack mouth
         * 2. walk -> player in attack range(pretty long) -> attack hand
         * 
         * phase 2
         * 
         * 1. run -> player in attack range(short) -> attack mouth
         * 2. run -> player in attack range(pretty long) -> attack hand
         * 3. run to some place -> screem (near by stunning) -> attack flame (ground)
         * 
         * phase 3
         * 
         * 1. fly -> go to some place -> FlyFlame with moving -> land
         * 2. run -> player in attack range(short) -> attack mouth
         * 3. run -> player in attack range(pretty long) -> attack hand
         * 4. run to some place -> screem (near by stunning) -> attack flame (ground)
         * 
         * 
         * 
         * */

    }
}
