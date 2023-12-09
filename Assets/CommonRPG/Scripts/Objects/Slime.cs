using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CommonRPG
{
    public class Slime : MonsterBase
    {
        protected override void Awake()
        {
            base.Awake();
            SlimeAnimController slimeAnimController = (SlimeAnimController)animController;
            Debug.Assert(slimeAnimController);

            //TimerHandler timerHandler = new TimerHandler();
            //timerHandler.StartTime = 1;
            //timerHandler.Interval = 1;
            //timerHandler.RepeatNumber = 100;
            //timerHandler.Function = 

            GameManager.SetTimer(1, 1, 1, () =>
            {
                if (isDead || slimeAnimController.IsHit)
                {
                    return;
                }

                slimeAnimController.PlayAttackAnim();
            }, true);
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            SlimeAnimController slimeAnimController = (SlimeAnimController)animController;
            Debug.Assert(slimeAnimController);

            slimeAnimController.OnAttackCheck += DoDamage;
        }

        protected override void OnDisable()
        {
            SlimeAnimController slimeAnimController = (SlimeAnimController)animController;
            Debug.Assert(slimeAnimController);

            slimeAnimController.OnAttackCheck -= DoDamage;
        }

        public override float TakeDamage(float DamageAmount, AUnit DamageCauser = null)
        {
            base.TakeDamage(DamageAmount, DamageCauser);

            SlimeAnimController slimeAnimController = (SlimeAnimController)animController;
            Debug.Assert(slimeAnimController);

            if (statComponent.CurrentHealthPoint <= 0) 
            {
                slimeAnimController.PlayDeathAnim();
            }
            else
            {
                slimeAnimController.PlayHitAnim();
            }
                return DamageAmount;
        }

        private void DoDamage(int bIsStartingAttackCheck)
        {
            LayerMask layerMask = LayerMask.GetMask("Character");
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, attackRange, layerMask)) 
            {
                IDamageable damageableTarget = hit.transform.GetComponent<IDamageable>();
                if(damageableTarget == null)
                {
                    return;
                }

                damageableTarget.TakeDamage(attackDamage, this);
            }
        }
    }
}
