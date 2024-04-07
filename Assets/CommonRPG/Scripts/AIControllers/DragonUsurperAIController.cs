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
            Attack,
            Fly

        }

        public event Action OnWakeUpStartDelegate = null;
        public event Action OnAttackWithMouthDelegate = null;
        public event Action OnAttackWithHandDelgate = null;
        /// <summary>
        /// Transform : targetTransform
        /// </summary>
        public event Action<Transform> OnAttackFlameGroundDelgate = null;

        /// <summary>
        /// Transform : TargetTransform
        /// </summary>
        public event Action<Transform> OnAttackAirFlameDelegate = null;

        private bool isMoveing = false;
        public bool IsMoving 
        { 
            
            get
            {
                return isMoveing || isMoveingSteadily;
            }
            private set
            {
                isMoveing = value;
            } 
        }

        private bool isMoveingSteadily = false;

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
        private float walkSpeed = 3.5f;

        [SerializeField]
        private float runSpeed = 7;

        [SerializeField]
        private float flyingSpeed = 10;

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

        [SerializeField]
        private float groundFlameAttackRange = 20;

        [SerializeField]
        private float groundFlameAttackInterval = 20;
        private float elapsedTime_GroundFlameAttack = 0;

        [SerializeField]
        private float airFlameAttackInterval = 20;
        private float elapsedTime_AirFlameAttack = 0;

        [SerializeField]
        private List<Transform> flameAttackGroundTransformList;

        [SerializeField]
        private List<Transform> flyingStartTransformList;

        private float rotationSpeed = 10;

        private EAIState currentAIState = EAIState.None;
        public EAIState CurrentAIState { get { return currentAIState; } set { currentAIState = value; } }

        private bool isAttacking = false;
        public bool IsAttacking { get { return isAttacking; } 
            set 
            { 
                isAttacking = value;
            } 
        }

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
                RunPhase2();
            }
            else if (base.currentPhase == EAIPhase.Phase3)
            {
                RunPhase3();
            }

            if (mouthAttackInterval > elapsedTime_MouthAttack) 
            {
                elapsedTime_MouthAttack += Time.deltaTime;
            }

            if (handAttackInterval > elapsedTime_HandAttack) 
            {
                elapsedTime_HandAttack += Time.deltaTime;
            }

            if (groundFlameAttackInterval > elapsedTime_GroundFlameAttack)
            {
                elapsedTime_GroundFlameAttack += Time.deltaTime;
            }

            if (airFlameAttackInterval > elapsedTime_AirFlameAttack)
            {
                elapsedTime_AirFlameAttack += Time.deltaTime;
            }

            float distanceToTarget = Vector3.Distance(targetTransform.position, transform.position);

            if (distanceToTarget <= agent.stoppingDistance)
            {
                IsMoving = false;
            }
        }

        public void SetFlyingDestination(Vector3 destination)
        {
            agent.SetDestination(destination);
            agent.isStopped = false;
            agent.speed = flyingSpeed;
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

        private void RunPhase2()
        {
            if (groundFlameAttackInterval <= elapsedTime_GroundFlameAttack)
            {
                // go to some place and flame attack
                int randomTransformIndex = Random.Range(0, flameAttackGroundTransformList.Count);

                if (RunSteadily(flameAttackGroundTransformList[randomTransformIndex].position))
                {
                    int lookTransformIndex = Random.Range(0, flameAttackGroundTransformList.Count - 1);

                    if (lookTransformIndex >= randomTransformIndex)
                    {
                        lookTransformIndex++;
                    }

                    AttackFlameGround(flameAttackGroundTransformList[lookTransformIndex]);
                }
            }
            else if (Run(targetTransform.position)) 
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

        private void RunPhase3()
        {
            if (airFlameAttackInterval <= elapsedTime_AirFlameAttack)
            {
                // go to some place and flame attack
                int randomTransformIndex = Random.Range(0, flameAttackGroundTransformList.Count);

                if (RunSteadily(flyingStartTransformList[randomTransformIndex].position))
                {
                    int lookTransformIndex = Random.Range(0, flameAttackGroundTransformList.Count - 1);

                    if (lookTransformIndex >= randomTransformIndex) 
                    {
                        lookTransformIndex++;
                    }

                    StartAirFlameAttack(flyingStartTransformList[lookTransformIndex]);
                }
            }
            else if (Run(targetTransform.position))
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

            //if (agent.isStopped == false)
            //{
            //    return false;
            //}

            agent.isStopped = false;
            agent.SetDestination(destination);
            agent.speed = walkSpeed;

            IsMoving = true;

            float actualRemainingDistance = Vector3.Distance(destination, transform.position);
            if (actualRemainingDistance > agent.stoppingDistance)
            {
                return false;
            }

            agent.isStopped = true;
            IsMoving = false;

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
                agent.isStopped = true;
                return false;
            }

            agent.isStopped = false;
            agent.SetDestination(destination);
            agent.speed = runSpeed;

            IsMoving = true;

            float actualRemainingDistance = Vector3.Distance(destination, transform.position);
            if (actualRemainingDistance > agent.stoppingDistance)
            {
                return false;
            }

            IsMoving = false;

            currentAIState = EAIState.Attack;

            return true;
        }

        /// <summary>
        /// can't change destination. only can change destination when arrived at destination..
        /// </summary>
        /// <returns> arrived at destination?</returns>
        private bool RunSteadily(Vector3 destination)
        {
            if (currentAIState != EAIState.Run)
            {
                return false;
            }

            if (IsAttacking)
            {
                agent.isStopped = true;
                return false;
            }

            if (isMoveingSteadily == false) 
            {
                isMoveingSteadily = true;
                agent.SetDestination(destination);
                agent.speed = runSpeed;
                return false;
            }

            if (agent.isStopped)
            {
                agent.isStopped = false;
            }

            IsMoving = true;
            
            float actualRemainingDistance = Vector3.Distance(destination, transform.position);
            if (actualRemainingDistance > agent.stoppingDistance)
            {
                return false;
            }

            agent.isStopped = true;
            IsMoving = false;
            isMoveingSteadily = false;

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

            //transform.LookAt(targetTransform, transform.up);
            //transform.LookAt();
            LookTarget(rotationSpeed);

            if (CurrentPhase == EAIPhase.Phase1)
            {
                currentAIState = EAIState.Walk;
            }
            else if (CurrentPhase > EAIPhase.Phase1)
            {
                currentAIState = EAIState.Run;
            }

            if (elapsedTime_MouthAttack < mouthAttackInterval)
            {
                return;
            }

            IsAttacking = true;

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

            //transform.LookAt(targetTransform, transform.up);

            LookTarget(rotationSpeed);

            if (CurrentPhase == EAIPhase.Phase1)
            {
                currentAIState = EAIState.Walk;
            }
            else if (CurrentPhase > EAIPhase.Phase1)
            {
                currentAIState = EAIState.Run;
            }

            if (elapsedTime_HandAttack < handAttackInterval)
            {
                return;
            }

            IsAttacking = true;

            elapsedTime_HandAttack = 0;
            OnAttackWithHandDelgate?.Invoke();
            //Debug.LogWarning($"Attack Mouth attacking : {IsAttacking}");
        }


        private void AttackFlameGround(Transform lookTransform)
        {
            if (currentAIState != EAIState.Attack)
            {
                return;
            }

            if (IsAttacking)
            {
                return;
            }

            if (elapsedTime_GroundFlameAttack < groundFlameAttackInterval)
            {
                return;
            }

            //transform.LookAt(lookTransform, transform.up);
            LookTarget(rotationSpeed);

            if (CurrentPhase == EAIPhase.Phase1)
            {
                currentAIState = EAIState.Walk;
            }
            else if (CurrentPhase > EAIPhase.Phase1)
            {
                currentAIState = EAIState.Run;
            }

            IsAttacking = true;

            elapsedTime_GroundFlameAttack = 0;
            OnAttackFlameGroundDelgate?.Invoke(lookTransform);
        }

        private void StartAirFlameAttack(Transform lookTransform)
        {
            if (currentAIState != EAIState.Attack)
            {
                return;
            }

            if (IsAttacking)
            {
                return;
            }

            if (elapsedTime_AirFlameAttack < airFlameAttackInterval)
            {
                return;
            }

            //transform.LookAt(lookTransform, transform.up);

            currentAIState = EAIState.Fly;

            IsAttacking = true;

            elapsedTime_AirFlameAttack = 0;
            OnAttackAirFlameDelegate?.Invoke(lookTransform);
        }

        private void LookTarget(float rotationSpeed)
        {
            Vector3 actualForward = Vector3.Lerp(transform.forward, (targetTransform.position - transform.position).normalized, Time.deltaTime * rotationSpeed);

            transform.forward = actualForward;
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
         * hp 1 ~ 0.8
         * 
         * phase 2
         * 
         * 1. run -> player in attack range(short) -> attack mouth
         * 2. run -> player in attack range(pretty long) -> attack hand
         * 3. run to some place -> screem (near by stunning) -> attack flame (ground)
         * hp 0.8 ~ 0.4
         * phase 3
         * 
         * 1. fly -> go to some place -> FlyFlame with moving -> land
         * 2. run -> player in attack range(short) -> attack mouth
         * 3. run -> player in attack range(pretty long) -> attack hand
         * 4. run to some place -> screem (near by stunning) -> attack flame (ground)
         * hp 0.4 ~ 0
         * 
         * 
         * */

    }
}
