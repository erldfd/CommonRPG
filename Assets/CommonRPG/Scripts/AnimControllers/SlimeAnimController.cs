using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace CommonRPG
{
    public class SlimeAnimController : MonsterAnimController
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
        /// params : int bIsStartingAttackCheck
        /// </summary>
        public event Action<int> OnAttackCheck = null;

        public override void PlayHitAnim()
        {
            animator.Play("GetHit", 0);
            isHit = true;
        }

        public void OnHitAnimEnd()
        {
            isHit = false;
        }

        public void PlayAttackAnim()
        {
            animator.Play("Attack01", 0);
        }

        public void PlayDeathAnim()
        {
            animator.Play("Die", 0);
        }

        public void StartAttackCheck(int bIsStartingAttackCheck)
        {
            OnAttackCheck.Invoke(bIsStartingAttackCheck);
        }
    }
}

