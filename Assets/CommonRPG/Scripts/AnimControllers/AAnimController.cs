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

        protected float currentMoveSpeed = 0;
        public float CurrentMoveSpeed
        {
            get { return currentMoveSpeed; }
            set
            {
                currentMoveSpeed = value;
                animator.SetFloat("CurrentMoveSpeed", Mathf.Floor(value));
            }
        }

        protected bool isHit = false;
        public bool IsHit { get { return isHit; } }

        private void Awake()
        {
            Debug.Assert(animator);
        }

        public abstract void PlayHitAnim();
    }
}
