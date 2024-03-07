using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public class DragonUsurper : BossMonster
    {

        private TimerHandler hpUITimerHandler = null;
        protected override void Awake()
        {
            base.Awake();

        }

        protected override void Update()
        {
            base.Update();

            animController.CurrentMoveSpeed = aiController.CurrentSpeed;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            DragonUsurperAIController dragonUsurperAIController = (DragonUsurperAIController)base.aiController;

            dragonUsurperAIController.OnWakeUpStartDelegate += OnWakeUpStart;
            dragonUsurperAIController.OnAttackWithMouthDelegate += OnAttakWithMouth;
            dragonUsurperAIController.OnAttackWithHandDelgate += OnAttackWithHand;

            DragonUsurperAnimController dragonUsurperAnimController = (DragonUsurperAnimController)base.animController;

            dragonUsurperAnimController.OnWakeUpEndedDelegate += OnWakeUpEnded;
            dragonUsurperAnimController.OnAttackCheck += DoDamage;
            dragonUsurperAnimController.OnBeginAttack += CheckBeginAttack;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            DragonUsurperAIController dragonUsurperAIController = (DragonUsurperAIController)base.aiController;

            dragonUsurperAIController.OnWakeUpStartDelegate -= OnWakeUpStart;
            dragonUsurperAIController.OnAttackWithMouthDelegate -= OnAttakWithMouth;
            dragonUsurperAIController.OnAttackWithHandDelgate -= OnAttackWithHand;

            DragonUsurperAnimController dragonUsurperAnimController = (DragonUsurperAnimController)base.animController;

            dragonUsurperAnimController.OnWakeUpEndedDelegate -= OnWakeUpEnded;
            dragonUsurperAnimController.OnAttackCheck -= DoDamage;
            dragonUsurperAnimController.OnBeginAttack -= CheckBeginAttack;
        }

        public override float TakeDamage(float DamageAmount, AUnit DamageCauser = null)
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

            if (hpUITimerHandler == null)
            {
                hpUITimerHandler = GameManager.SetTimer(3, 1, 0, () => {

                    GameManager.InGameUI.SetMonsterInfoUIVisible(false);

                }, true);

                hpUITimerHandler.IsStayingActive = true;
            }
            else
            {
                hpUITimerHandler.RestartTimer();
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

            DragonUsurperAnimController monsterAnimController = (DragonUsurperAnimController)animController;

            if (statComponent.CurrentHealthPoint <= 0)
            {
                monsterAnimController.PlayDeathAnim();
                //OnKilled.Invoke(this, DamageCauser);
            }
            else
            {
                //monsterAnimController.PlayHitAnim();
            }

            return actualDamageAmount;
        }

        private void OnWakeUpStart()
        {
            DragonUsurperAnimController dragonUsurperAnimController = (DragonUsurperAnimController)base.animController;

            dragonUsurperAnimController.PlayWakeUpAnim();

        }

        private void OnWakeUpEnded()
        {
            DragonUsurperAIController dragonUsurperAIController = (DragonUsurperAIController)base.aiController;
            dragonUsurperAIController.CurrentPhase = BossAIController.EAIPhase.Phase1;
            dragonUsurperAIController.CurrentAIState = DragonUsurperAIController.EAIState.Walk;
        }

        private void OnAttakWithMouth()
        {
            DragonUsurperAnimController dragonUsurperAnimController = (DragonUsurperAnimController)base.animController;

            dragonUsurperAnimController.PlayAttackMouth();
        }
        private void OnAttackWithHand()
        {
            DragonUsurperAnimController dragonUsurperAnimController = (DragonUsurperAnimController)base.animController;

            dragonUsurperAnimController.PlayAttackHand();
        }

        private void DoDamage(bool isStartingAttackCheck)
        {
            Debug.Log($"damage : {isStartingAttackCheck}");
        }

        private void CheckBeginAttack(bool BeganAttack)
        {
            DragonUsurperAIController dragonUsurperAIController = (DragonUsurperAIController)base.aiController;
            dragonUsurperAIController.IsAttacking = BeganAttack;

            Debug.Log($"beginATtack : {BeganAttack}");
        }
    }
}
