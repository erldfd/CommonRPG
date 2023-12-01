using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    /// <summary>
    /// Take Damage from DamageCauser.
    /// </summary>
    /// <returns> Taked Damage Amount. </returns>
    float TakeDamage(float DamageAmount, IDamageable DamageCauser = null);
}
