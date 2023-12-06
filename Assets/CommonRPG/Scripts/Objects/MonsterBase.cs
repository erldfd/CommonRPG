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
        public virtual float TakeDamage(float DamageAmount, AUnit DamageCauser = null)
        {
            statComponent.CurrentHealthPoint -= DamageAmount;
            Debug.Log($"Damage is Taked : {DamageAmount}, CurrentHp : {statComponent.CurrentHealthPoint}");

            float currentHpRatio = Mathf.Clamp01(statComponent.CurrentHealthPoint / statComponent.TotalHealth);
            GameManager.SetMonsterHealthBarFillRatio(currentHpRatio);
            GameManager.SetMonsterInfoUIVisible(true);
            if (currentHpRatio <= 0)
            {
                BeKilled();
            }

            return DamageAmount;
        }

        // Start is called before the first frame update
        void Start()
        {
            statComponent.TotalHealth = 5;
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
