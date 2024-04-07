using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public interface IDamageable
    {
        /// <summary>
        /// Take Damage from DamageCauser.
        /// </summary>
        /// <returns> Taked Damage Amount. </returns>
        public float TakeDamage(float DamageAmount, AUnit DamageCauser, Object extraData = null);
    }
}


