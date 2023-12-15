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
    }
}

