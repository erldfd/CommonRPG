using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Knight : ACharacter
{
    public override float TakeDamage(float DamageAmount, IDamageable DamageCauser = null)
    {
        statComponenet.CurrentHealthPoint -= DamageAmount;
        float currentHpRatio = Mathf.Clamp01(statComponenet.CurrentHealthPoint / statComponenet.TotalHealth);
        GameManager.SetPlayerHealthBarFillRatio(currentHpRatio);

        Debug.Log($"Damage is Taked : {DamageAmount}, CurrentHp : {statComponenet.CurrentHealthPoint}");

        return DamageAmount;
    }

    protected override void OnMove(InputAction.CallbackContext value)
    {
        base.OnMove(value);
        Debug.Log("KnightMove");
    }

}
