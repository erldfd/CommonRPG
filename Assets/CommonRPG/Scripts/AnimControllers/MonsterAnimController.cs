using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public class MonsterAnimController : AAnimController
    {
        [SerializeField]
        protected string attackAnimName;

        public virtual void PlayAttackAnim()
        {
            base.animator.Play(attackAnimName, 0);
        }
    }
}

