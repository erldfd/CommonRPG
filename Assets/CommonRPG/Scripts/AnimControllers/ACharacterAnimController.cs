using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public class ACharacterAnimController : AAnimController
    {
        [SerializeField]
        protected string drinkAnimName;

        protected bool isDrinking = false;
        public bool IsDrinking { get { return isDrinking; } }

        public virtual void PlayDrinkAnim()
        {
            animator.Play(drinkAnimName, 1);
            isDrinking = true;
        }

        public override void OnHitAnimEnd()
        {
            isHit = false;
            isDrinking = false;
        }

        public virtual void OnDrinkingEnd()
        {
            isDrinking = false;
            isHit = false;
        }
    }
}

