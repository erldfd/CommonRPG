using System;
using UnityEngine;

namespace CommonRPG
{
    public class KnightAnimController : ACharacterAnimController
    {
        protected float normalStateRate = 1;
        public float NormalStateRate
        {
            get { return normalStateRate; }
            set
            {
                normalStateRate = value;
                base.animator.SetFloat("NormalStateRate", value);

                BattleStateRate = 1 - value;
            }
        }

        protected float battleStateRate = 0;
        public float BattleStateRate
        {
            get { return battleStateRate; }
            set
            {
                battleStateRate = value;
                base.animator.SetFloat("BattleStateRate", value);

                NormalStateRate = 1 - value;
            }
        }
        /// <summary>
        /// params : bool isCheckingAttack
        /// </summary>
        public event Action<bool> OnAttackCheck = null;
        /// <summary>
        /// bool isCheckingComboInitiate
        /// </summary>
        public event Action<bool> OnComboCheck = null;

        private bool isBeginningAttackAnim = false;
        public bool IsBeginningAttackAnim
        {
            get { return isBeginningAttackAnim; }
            private set { isBeginningAttackAnim = value; }
        }

        private bool shouldPlayNextComboAttack = false;
        public bool ShouldPlayNextComboAttackAnim
        {
            get { return shouldPlayNextComboAttack; }
            set { shouldPlayNextComboAttack = value; }
        }

        private int comboCount = 0;
        public int ComboCount
        {
            get { return comboCount; }
            set { comboCount = value; }
        }

        public override void PlayHitAnim()
        {
            animator.Play("GetHit01_SwordAndShield", 0);
            isHit = true;
        }

        public void OnHitAnimEnd()
        {
            isHit = false;
        }

        public void PlayNormalAttackAnim(int playIndex)
        {
            switch(playIndex)
            {
                case 0:
                    animator.Play("Attack01_SwordAndShiled", 0);
                    break;
                case 1:
                    animator.Play("Attack02_SwordAndShiled", 0);
                    break;
                case 2:
                    animator.Play("Attack03_SwordAndShiled", 0);
                    break;
                case 3:
                    animator.Play("Attack04_SwordAndShiled", 0);
                    break;
                default:
                    Debug.LogWarning($"Wrong Index : {playIndex}");
                    break;
            }
        }

        public void PlayDeathAnim()
        {
            animator.Play("Die01_SwordAndShield", 0);
        }

        public void StartAttackCheck(int bIsCheckingAttack)
        {
            OnAttackCheck.Invoke(bIsCheckingAttack != 0);
            OnComboCheck.Invoke(false);
        }

        public void StartComboCheck(int bIsCheckingComboInitiate)
        {
            OnComboCheck.Invoke(bIsCheckingComboInitiate != 0);
        }

        public void CheckAttackAnimBegin(int bIsBeginningAttackAnim)
        {
            bool isBeginningAttackAnim = (bIsBeginningAttackAnim != 0);
            this.isBeginningAttackAnim = isBeginningAttackAnim;
            //Debug.Log($"isBeginningAttackAnim : {isBeginningAttackAnim}");
            if (isBeginningAttackAnim)
            {
                shouldPlayNextComboAttack = false;
            }
            else if (shouldPlayNextComboAttack) 
            {
                PlayNormalAttackAnim(comboCount++);
            }
        }
        /// <summary>
        /// this is for initialize animation properties.
        /// </summary>
        public void OnAnimStart()
        {
            OnAttackCheck.Invoke(false);
            OnComboCheck.Invoke(false);
            isBeginningAttackAnim = false;
            shouldPlayNextComboAttack = false;
        }
    }

}
