using System;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public class KnightAnimController : ACharacterAnimController
    {
        [SerializeField]
        protected List<string> comboAttackAnimList;

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
        /// bool isCheckingComboInitiate
        /// </summary>
        public event Action<bool> OnComboCheckDelegate = null;

        /// <summary>
        /// int : combo index
        /// </summary>
        public event Action<int> OnStartPlayingComboAttackDelegate = null;

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

        public void PlayComboAttackAnim(int playIndex)
        {
            base.animator.Play(comboAttackAnimList[playIndex], 0);
            OnStartPlayingComboAttackDelegate.Invoke(playIndex);
        }

        public override void StartAttackCheck(int bIsCheckingAttack)
        {
            base.StartAttackCheck(bIsCheckingAttack);

            OnComboCheckDelegate.Invoke(false);
        }

        public void StartComboCheck(int bIsCheckingComboInitiate)
        {
            OnComboCheckDelegate.Invoke(bIsCheckingComboInitiate != 0);
        }

        public void CheckAttackAnimBegin(int bIsBeginningAttackAnim)
        {
            bool isBeginningAttackAnim = (bIsBeginningAttackAnim != 0);
            this.isBeginningAttackAnim = isBeginningAttackAnim;

            if (isBeginningAttackAnim)
            {
                shouldPlayNextComboAttack = false;
            }
            else if (shouldPlayNextComboAttack) 
            {
                PlayComboAttackAnim(comboCount++);
            }
        }
        /// <summary>
        /// this is for initialize animation properties.
        /// </summary>
        public override void OnAnimStart()
        {
            base.OnAnimStart();

            OnComboCheckDelegate.Invoke(false);
            isBeginningAttackAnim = false;
            shouldPlayNextComboAttack = false;
        }
    }

}
