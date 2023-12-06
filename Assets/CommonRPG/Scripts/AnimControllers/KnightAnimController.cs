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
        /// params : int bIsStartingAttackCheck
        /// </summary>
        public event Action<int> OnAttackCheck = null;

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
            animator.Play("Attack01_SwordAndShiled", 0);
            Debug.Log($"PlayNormalAttack Index : {playIndex}");
        }

        public void PlayDeathAnim()
        {
            animator.Play("Die01_SwordAndShield", 0);
        }

        public void StartAttackCheck(int bIsStartingAttackCheck)
        {
            OnAttackCheck.Invoke(bIsStartingAttackCheck);
        }
    }

}
