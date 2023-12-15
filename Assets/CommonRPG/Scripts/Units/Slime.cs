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

            base.aiController.OnAttack += Attack;
        }

        protected override void OnDisable()
        {
            SlimeAnimController slimeAnimController = (SlimeAnimController)animController;
            Debug.Assert(slimeAnimController);

            slimeAnimController.OnAttackCheck -= DoDamage;

            base.aiController.OnAttack -= Attack;
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

        private void DoDamage(bool isStartingAttackCheck)
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

        private void Attack(Transform targetTransform)
        {
            SlimeAnimController slimeAnimController = (SlimeAnimController)animController;
            Debug.Assert(slimeAnimController);

            if (isDead)
            {
                aiController.IsAIActivated = false;
                return;
            }

            if (slimeAnimController.IsHit)
            {
                return;
            }

            Vector3 LookTargetVector = targetTransform.position - transform.position;
            transform.forward = LookTargetVector;

            slimeAnimController.PlayAttackAnim();
        }
    }
}
