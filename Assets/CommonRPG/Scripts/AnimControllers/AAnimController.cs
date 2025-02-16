using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public abstract class AAnimController : MonoBehaviour
    {
        [SerializeField]
        protected Animator animator;

        [SerializeField]
        protected string hitAnimName;

        [SerializeField]
        protected string deathAnimName;

        protected float currentMoveSpeed = 0;
        public virtual float CurrentMoveSpeed
        {
            get { return currentMoveSpeed; }
            set
            {
                currentMoveSpeed = value;
                animator.SetFloat("CurrentMoveSpeed", value);
            }
        }

        protected bool isHit = false;
        public bool IsHit { get { return isHit; } }

        /// <summary>
        /// params : bool bIsStartingAttackCheck
        /// </summary>
        public event Action<bool> OnAttackCheckDelegate = null;

        protected virtual void Awake()
        {
            Debug.Assert(animator);
        }

        public virtual void PlayHitAnim()
        {
            animator.Play(hitAnimName, 0);
            isHit = true;
        }

        public virtual void OnHitAnimEnd()
        {
            isHit = false;
        }

        public virtual void PlayDeathAnim()
        {
            animator.Play(deathAnimName, 0);
        }

        public virtual void StartAttackCheck(int bIsCheckingAttack)
        {
            OnAttackCheckDelegate.Invoke(bIsCheckingAttack != 0);
        }

        public virtual void OnAnimStart()
        {
            OnAttackCheckDelegate.Invoke(false);
        }
    }
}
