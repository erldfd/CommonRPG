using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Knight : ACharacter
{
    public override float TakeDamage(float DamageAmount, IDamageable DamageCauser = null)
    {
        Debug.Log($"Damage is Taked : {DamageAmount}");
        return DamageAmount;
    }

    protected override void OnMove(InputAction.CallbackContext value)
    {
        base.OnMove(value);
        Debug.Log("KnightMove");
    }

}
