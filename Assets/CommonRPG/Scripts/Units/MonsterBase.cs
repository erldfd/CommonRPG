using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public class MonsterBase : AUnit, IDamageable
    {
        [SerializeField]
        protected float attackDamage = 1;

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
        public virtual float TakeDamage(float DamageAmount, AUnit DamageCauser = null)
        {
            statComponent.CurrentHealthPoint -= DamageAmount;
            Debug.Log($"Damage is Taked : {DamageAmount}, CurrentHp : {statComponent.CurrentHealthPoint}");

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

            if (currentHpRatio <= 0)
            {
                BeKilled();
            }

            return DamageAmount;
        }

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
                    Destroy(gameObject);
                }
            }

            animController.CurrentMoveSpeed = aiController.CurrentSpeed;
        }

        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
        }

        protected void BeKilled()
        {
            base.isDead = true;
            deathTime = 0;
        }

        
    }

}
