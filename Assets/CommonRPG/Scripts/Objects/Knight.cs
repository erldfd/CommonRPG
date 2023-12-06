using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CommonRPG
{
    public class Knight : ACharacter
    {

        [SerializeField]
        private WeaponItem knightWeapon = null;

        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(knightWeapon);
        }

        protected override void Update()
        {
            base.Update();
            if (CheckMoveCondition() == false)
            {
                SetMovementDirection(Vector2.zero);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            KnightAnimController knightAnimController = (KnightAnimController)animController;
            Debug.Assert(knightAnimController);

            knightAnimController.OnAttackCheck += EnableCollider;
        }

        protected override void OnDisable()
        {
            KnightAnimController knightAnimController = (KnightAnimController)animController;
            Debug.Assert(knightAnimController);

            knightAnimController.OnAttackCheck -= EnableCollider;
        }

        public override float TakeDamage(float DamageAmount, AUnit DamageCauser = null)
        {
            statComponent.CurrentHealthPoint -= DamageAmount;
            float currentHpRatio = Mathf.Clamp01(statComponent.CurrentHealthPoint / statComponent.TotalHealth);
            GameManager.SetPlayerHealthBarFillRatio(currentHpRatio);

            Debug.Log($"Damage is Taked : {DamageAmount}, CurrentHp : {statComponent.CurrentHealthPoint}");

            KnightAnimController knightAnimController = (KnightAnimController)animController;
            Debug.Assert(knightAnimController);

            if (currentHpRatio > 0) 
            {
                knightAnimController.PlayHitAnim();
            }
            else
            {
                knightAnimController.PlayDeathAnim();
                isDead = true;
            }

            return DamageAmount;
        }

        protected override void OnMove(InputAction.CallbackContext value)
        {
            if (CheckMoveCondition() == false)
            {
                SetMovementDirection(Vector2.zero);
                return;
            }

            base.OnMove(value);
        }

        protected override void OnPauseAndResume(InputAction.CallbackContext context)
        {
            KnightAnimController knightAnimController = (KnightAnimController)animController;
            Debug.Assert(knightAnimController);

            knightAnimController.PlayNormalAttackAnim(9);
        }

        private void EnableCollider(int bShouldEnable)
        {
            knightWeapon.EnableCollider(Convert.ToBoolean(bShouldEnable));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true : movable state</returns>
        private bool CheckMoveCondition()
        {
            KnightAnimController knightAnimController = (KnightAnimController)animController;
            Debug.Assert(knightAnimController);
            
            return (isDead == false && knightAnimController.IsHit == false);
        }
    }

}
