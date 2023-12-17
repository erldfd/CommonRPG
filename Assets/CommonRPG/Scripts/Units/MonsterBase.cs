using UnityEngine;

namespace CommonRPG
{
    public class MonsterBase : AUnit, IDamageable
    {
        [SerializeField]
        protected EMonsterName monsterName = EMonsterName.None;
        public EMonsterName MonsterName
        {
            get { return monsterName; }
            set { monsterName = value; }
        }

        [SerializeField]
        protected float attackRange = 2;

        [SerializeField]
        protected float despawnTime = 3;

        /// <summary>
        /// how much time elapsed after death
        /// </summary>
        protected float deathTime = 0;

        protected AIController aiController = null;

        protected TimerHandler monsterUITimerHandler = null;
        
        protected override void Awake()
        {
            base.Awake();

            aiController = GetComponent<AIController>();
            Debug.Assert(aiController);
        }

        // Update is called once per frame
        protected override void Update()
        {
            if (base.isDead)
            {
                if (deathTime < despawnTime)
                {
                    deathTime += Time.deltaTime;
                }
                else
                {
                    //Destroy(gameObject);
                    GameManager.DeactiveMonster(this);
                }
            }

            animController.CurrentMoveSpeed = aiController.CurrentSpeed;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            MonsterAnimController monsterAnimController = (MonsterAnimController)animController;
            Debug.Assert(monsterAnimController);

            monsterAnimController.OnAttackCheck += DoDamage;

            aiController.OnAttack += Attack;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            MonsterAnimController monsterAnimController = (MonsterAnimController)animController;
            Debug.Assert(monsterAnimController);

            monsterAnimController.OnAttackCheck -= DoDamage;

            aiController.OnAttack -= Attack;
        }

        public virtual float TakeDamage(float DamageAmount, AUnit DamageCauser = null)
        {
            if (IsDead) 
            {
                return 0f;
            }

            statComponent.CurrentHealthPoint -= DamageAmount;
            //Debug.Log($"Damage is Taked : {DamageAmount}, CurrentHp : {statComponent.CurrentHealthPoint}");

            float currentHpRatio = Mathf.Clamp01(statComponent.CurrentHealthPoint / statComponent.TotalHealth);
            GameManager.SetMonsterHealthBarFillRatio(currentHpRatio);
            GameManager.SetMonsterInfoUIVisible(true);
            GameManager.SetMonsterNameText(base.unitName);

            if (monsterUITimerHandler == null)
            {
                monsterUITimerHandler = GameManager.SetTimer(3, 1, 0, () => { GameManager.SetMonsterInfoUIVisible(false); }, true);
                monsterUITimerHandler.IsStayingActive = true;
            }
            else
            {
                monsterUITimerHandler.RestartTimer();
            }

            if (currentHpRatio <= 0 && base.isDead == false)
            {
                BeKilled();

                if (DamageCauser is ACharacter)
                {
                    ACharacter character = (ACharacter)DamageCauser;

                    float obatiningExp = GameManager.GetMonsterData(monsterName).Data.Exp;
                    float expTolerance = GameManager.GetMonsterData(monsterName).Data.ExpTolerance;

                    character.ObtainExp(Random.Range(obatiningExp - expTolerance, obatiningExp + expTolerance));
                    Debug.Log($"exp obtained : {Random.Range(obatiningExp - expTolerance, obatiningExp + expTolerance)}");
                }
            }

            MonsterAnimController monsterAnimController = (MonsterAnimController)animController;
            Debug.Assert(monsterAnimController);

            if (statComponent.CurrentHealthPoint <= 0)
            {
                monsterAnimController.PlayDeathAnim();
            }
            else
            {
                monsterAnimController.PlayHitAnim();
            }

            return DamageAmount;
        }

        protected virtual void DoDamage(bool isStartingAttackCheck)
        {
            LayerMask layerMask = LayerMask.GetMask("Character");
            float radius = 0.5f;
            Collider[] hitColliders = Physics.OverlapCapsule(transform.position, transform.position + transform.forward * attackRange, radius, layerMask);

            if (hitColliders.Length > 0) 
            {
                IDamageable damageableTarget = hitColliders[0].transform.GetComponent<IDamageable>();
                if(damageableTarget == null)
                {
                    return;
                }

                damageableTarget.TakeDamage(StatComponent.BaseAttackPower, this);
            }
        }

        protected virtual void Attack(Transform targetTransform)
        {
            MonsterAnimController monsterAnimController = (MonsterAnimController)animController;
            Debug.Assert(monsterAnimController);

            if (isDead)
            {
                aiController.IsAIActivated = false;
                return;
            }

            if (monsterAnimController.IsHit)
            {
                return;
            }

            Vector3 LookTargetVector = targetTransform.position - transform.position;
            transform.forward = LookTargetVector;

            monsterAnimController.PlayAttackAnim();
        }

        protected void BeKilled()
        {
            base.isDead = true;
            deathTime = 0;
            ActivateAI(false);
        }

        public void ActivateAI(bool shouldActivate)
        {
            aiController.IsAIActivated = shouldActivate;
        }
    }

}
