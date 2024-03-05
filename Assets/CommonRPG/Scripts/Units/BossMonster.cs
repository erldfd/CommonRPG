using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public class BossMonster : MonsterBase
    {
        public enum EState
        {
            Sleep,
            Phase1,
            Phase2,
            Dead
        }

        protected override void Update()
        {
            base.Update();


        }
    }
}
