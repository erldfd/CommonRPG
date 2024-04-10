using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public class DragonUserperDamageEventInfo : ADamageEventInfo
    {
        public enum EAttackType
        {
            None,
            MouthAttack,
            HandAttack,
            FlameAttack
        }

        public EAttackType damageType = EAttackType.None;
    }
}
