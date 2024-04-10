using System;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace CommonRPG
{
    public class NormalMonster : MonsterBase
    {
        [SerializeField]
        protected float attackRange = 2;

        ///// <summary>
        ///// how much time elapsed after death
        ///// </summary>
        //protected float deathTime = 0;

        //protected NormalMonsterAIController aiController = null;

        //private TimerHandler monsterUITimerHandler = null;

        //protected float afterImageHpProgressBarFillRatio = 0;
        //public float AfterImageHpProgressBarFillRatio { get; }


        //protected override void Awake()
        //{
        //    base.Awake();

        //    Debug.Assert(base.statComponent);
        //    Debug.Assert(base.animController);

        //    aiController = GetComponent<NormalMonsterAIController>();
        //    Debug.Assert(aiController);
        //}

        //// Update is called once per frame
        //protected override void Update()
        //{
        //    if (base.isDead)
        //    {
        //        if (aiController.IsAIActivated) 
        //        {
        //            ActivateAI(false);
        //        }

        //        if (deathTime < despawnTime)
        //        {
        //            deathTime += Time.deltaTime;
        //        }
        //        else
        //        {
        //            //Destroy(gameObject);
        //            GameManager.DeactiveMonster(this);
        //        }
        //    }

        //    animController.CurrentMoveSpeed = aiController.CurrentSpeed;
        //}

        protected override void OnEnable()
        {
            base.OnEnable();

            MonsterAnimController monsterAnimController = (MonsterAnimController)animController;

            monsterAnimController.OnAttackCheckDelegate += DoDamage;

            NormalMonsterAIController normalAIController = (NormalMonsterAIController)aiController;

            normalAIController.OnAttack += Attack;
        }

        protected override void OnDisable()
        {
            MonsterAnimController monsterAnimController = (MonsterAnimController)animController;

            monsterAnimController.OnAttackCheckDelegate -= DoDamage;

            NormalMonsterAIController normalAIController = (NormalMonsterAIController)aiController;

            normalAIController.OnAttack -= Attack;

            base.OnDisable();
        }

        public override float TakeDamage(float DamageAmount, AUnit DamageCauser = null, ADamageEventInfo damageEventInfo = null)
        {

            //if (IsDead) 
            //{
            //    return 0f;
            //}

            //float actualDamageAmount = DamageAmount - statComponent.TotalDefense;
            //if (actualDamageAmount < 1) 
            //{
            //    actualDamageAmount = 1;
            //}

            //float beforeHpRatio = statComponent.CurrentHealthPoint / statComponent.TotalHealth;

            //statComponent.CurrentHealthPoint -= actualDamageAmount;

            //float currentHpRatio = Mathf.Clamp01(statComponent.CurrentHealthPoint / statComponent.TotalHealth);
            //float afterimageChangeTime = 0.5f;

            //GameManager.InGameUI.DisplayDecrasingMonsterHealthBar(currentHpRatio, beforeHpRatio, afterimageChangeTime, this);
            //GameManager.InGameUI.SetMonsterInfoUIVisible(true);
            //GameManager.InGameUI.SetMonsterNameText(base.unitName);

            //float offsetY = 1;
            //Vector3 damageDisplayPosition = new Vector3(transform.position.x, transform.position.y + offsetY, transform.position.z);

            //GameManager.InGameUI.FloatDamageNumber(actualDamageAmount, damageDisplayPosition);

            //if (monsterUITimerHandler == null)
            //{
            //    monsterUITimerHandler = GameManager.SetTimer(3, 1, 0, () => {

            //        GameManager.InGameUI.SetMonsterInfoUIVisible(false);

            //    }, true);

            //    monsterUITimerHandler.IsStayingActive = true;
            //}
            //else
            //{
            //    monsterUITimerHandler.RestartTimer();
            //}

            //if (currentHpRatio <= 0 && base.isDead == false)
            //{
            //    BeKilled();

            //    if (DamageCauser is ACharacter)
            //    {
            //        ACharacter character = (ACharacter)DamageCauser;

            //        float obtainingExp = GameManager.GetMonsterData(monsterName).Data.Exp;
            //        float expTolerance = GameManager.GetMonsterData(monsterName).Data.ExpTolerance;

            //        float decidedExp = Random.Range(obtainingExp - expTolerance, obtainingExp + expTolerance);
            //        character.ObtainExp(decidedExp);

            //        int obtainingCoins = GameManager.GetMonsterData(MonsterName).Data.HoldingMoney;
            //        int coinTolerance = GameManager.GetMonsterData(MonsterName).Data.MoneyTolerance;

            //        GameManager.SetCoins(GameManager.GetCurrentCoins() + Random.Range(obtainingCoins - coinTolerance, obtainingCoins + coinTolerance));
            //        GameManager.DropItemFromMonster(MonsterName, transform.position, transform.rotation);
            //    }
            //}

            //MonsterAnimController monsterAnimController = (MonsterAnimController)animController;
            //Debug.Assert(monsterAnimController);

            //if (statComponent.CurrentHealthPoint <= 0)
            //{
            //    monsterAnimController.PlayDeathAnim();
            //    OnKilled.Invoke(this, DamageCauser);
            //}
            //else
            //{
            //    monsterAnimController.PlayHitAnim();
            //}

            return base.TakeDamage(DamageAmount, DamageCauser);
        }

        protected virtual void DoDamage(bool isStartingAttackCheck)
        {
            LayerMask layerMask = LayerMask.GetMask("Character");
            float radius = 0.5f;
            float OffsetY = 0.5f;
            Collider[] hitColliders = Physics.OverlapCapsule(transform.position + transform.up * OffsetY, transform.position + transform.up * OffsetY + transform.forward * attackRange, radius, layerMask);
            
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

        //protected void BeKilled()
        //{
        //    base.isDead = true;
        //    deathTime = 0;
        //    ActivateAI(false);
        //}

        //public void ActivateAI(bool shouldActivate)
        //{
        //    aiController.IsAIActivated = shouldActivate;
        //}
    }
}
