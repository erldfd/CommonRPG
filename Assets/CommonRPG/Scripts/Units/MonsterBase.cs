using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CommonRPG
{
    public class MonsterBase : AUnit, IDamageable
    {
        public static event Action<MonsterBase, AUnit> OnKilled;

        [SerializeField]
        protected EMonsterName monsterName = EMonsterName.None;
        public EMonsterName MonsterName
        {
            get { return monsterName; }
            set { monsterName = value; }
        }

        [SerializeField]
        protected float despawnTime = 3;

        /// <summary>
        /// how much time elapsed after death
        /// </summary>
        protected float deathTime = 0;

        protected TimerHandler monsterUITimerHandler = null;

        protected AAIController aiController = null;

        protected override void Awake()
        {
            base.Awake();

            Debug.Assert(base.statComponent);
            Debug.Assert(base.animController);

            aiController = GetComponent<AAIController>();
            Debug.Assert(aiController);
        }

        // Update is called once per frame
        protected override void Update()
        {
            if (base.isDead)
            {
                if (aiController.IsAIActivated)
                {
                    ActivateAI(false);
                }

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

        protected override void OnDestroy()
        {
            if (monsterUITimerHandler != null) 
            {
                monsterUITimerHandler.IsStayingActive = false;
            }

            base.OnDestroy();
        }

        public virtual float TakeDamage(float DamageAmount, AUnit DamageCauser = null)
        {
            if (IsDead)
            {
                return 0f;
            }

            float actualDamageAmount = DamageAmount - statComponent.TotalDefense;
            if (actualDamageAmount < 1)
            {
                actualDamageAmount = 1;
            }

            float beforeHpRatio = statComponent.CurrentHealthPoint / statComponent.TotalHealth;

            statComponent.CurrentHealthPoint -= actualDamageAmount;

            float currentHpRatio = Mathf.Clamp01(statComponent.CurrentHealthPoint / statComponent.TotalHealth);
            float afterimageChangeTime = 0.5f;

            GameManager.InGameUI.DisplayDecrasingMonsterHealthBar(currentHpRatio, beforeHpRatio, afterimageChangeTime, this);
            GameManager.InGameUI.SetMonsterInfoUIVisible(true);
            GameManager.InGameUI.SetMonsterNameText(base.unitName);

            float offsetY = 1;
            Vector3 damageDisplayPosition = new Vector3(transform.position.x, transform.position.y + offsetY, transform.position.z);

            GameManager.InGameUI.FloatDamageNumber(actualDamageAmount, damageDisplayPosition);

            if (monsterUITimerHandler == null)
            {
                monsterUITimerHandler = GameManager.TimerManager.SetTimer(3, 1, 0, () => {

                    GameManager.InGameUI.SetMonsterInfoUIVisible(false);

                }, true);

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

                    float obtainingExp = GameManager.GetMonsterData(monsterName).Data.Exp;
                    float expTolerance = GameManager.GetMonsterData(monsterName).Data.ExpTolerance;

                    float decidedExp = Random.Range(obtainingExp - expTolerance, obtainingExp + expTolerance);
                    character.ObtainExp(decidedExp);

                    int obtainingCoins = GameManager.GetMonsterData(MonsterName).Data.HoldingMoney;
                    int coinTolerance = GameManager.GetMonsterData(MonsterName).Data.MoneyTolerance;

                    GameManager.SetCoins(GameManager.GetCurrentCoins() + Random.Range(obtainingCoins - coinTolerance, obtainingCoins + coinTolerance));
                    GameManager.DropItemFromMonster(MonsterName, transform.position, transform.rotation);
                }
            }

            MonsterAnimController monsterAnimController = (MonsterAnimController)animController;

            if (statComponent.CurrentHealthPoint <= 0)
            {
                monsterAnimController.PlayDeathAnim();
                OnKilled.Invoke(this, DamageCauser);
            }
            else
            {
                monsterAnimController.PlayHitAnim();
            }

            return actualDamageAmount;
        }

        protected void BeKilled()
        {
            base.isDead = true;
            deathTime = 0;
            ActivateAI(false);

            if (monsterUITimerHandler != null) 
            {
                monsterUITimerHandler.IsStayingActive = false;
            }
        }

        public void ActivateAI(bool shouldActivate)
        {
            aiController.IsAIActivated = shouldActivate;
        }
    }
}
