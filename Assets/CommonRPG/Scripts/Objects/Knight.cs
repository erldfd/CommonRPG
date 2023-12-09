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

        private bool canCombo = false;

        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(knightWeapon);

            float currentHpRatio = Mathf.Clamp01(statComponent.CurrentHealthPoint / statComponent.TotalHealth);
            GameManager.SetPlayerHealthBarFillRatio(currentHpRatio);
            GameManager.SetPlayerLevelText(statComponent.Level);
            GameManager.SetPlayerManaBarFillRatio(statComponent.CurrentManaPoint / statComponent.TotalMana);
            GameManager.SetPlayerNameText(base.unitName);
        }

        protected override void Update()
        {
            base.Update();
            if (IsMovable() == false)
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
            knightAnimController.OnComboCheck += CheckCombo;

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

        protected override void OnMove(InputAction.CallbackContext context)
        {
            if (IsMovable() == false)
            {
                SetMovementDirection(Vector2.zero);
                return;
            }

            base.OnMove(context);
        }

        protected override void OnPauseAndResume(InputAction.CallbackContext context)
        {
            KnightAnimController knightAnimController = (KnightAnimController)animController;
            Debug.Assert(knightAnimController);

            knightAnimController.PlayHitAnim();
        }

        protected override void OnNormalAttack(InputAction.CallbackContext context)
        {
            if (IsAttackPossible() == false) 
            {
                return;
            }

            KnightAnimController knightAnimController = (KnightAnimController)animController;
            Debug.Assert(knightAnimController);

            if (knightAnimController.IsBeginningAttackAnim == false)
            {
                knightAnimController.ComboCount = 0;
                knightAnimController.PlayNormalAttackAnim(knightAnimController.ComboCount++);
            }
            else if (canCombo) 
            {
                knightAnimController.ShouldPlayNextComboAttackAnim = true;
            }
        }

        private void EnableCollider(bool shouldEnable)
        {
            knightWeapon.EnableCollider(shouldEnable);
        }

        /// <summary>
        /// check if combo is possible now
        /// </summary>
        private void CheckCombo(bool canCombo)
        {
            this.canCombo = canCombo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true means movable state</returns>
        private bool IsMovable()
        {
            KnightAnimController knightAnimController = (KnightAnimController)animController;
            Debug.Assert(knightAnimController);

            bool isMovable = (isDead == false && knightAnimController.IsHit == false && knightAnimController.IsBeginningAttackAnim == false);
            //Debug.Log($"{isDead}, {knightAnimController.IsHit}, {knightAnimController.IsBeginningAttackAnim}, {isMoveable}");
            return isMovable;
        }

        private bool IsAttackPossible()
        {
            KnightAnimController knightAnimController = (KnightAnimController)animController;
            Debug.Assert(knightAnimController);

            bool isAttackPossible = (isDead == false && knightAnimController.IsHit == false);
            return isAttackPossible;
        }
    }

}
