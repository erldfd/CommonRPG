using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBase : AUnit, IDamageable
{
    [SerializeField]
    protected float despawnTime = 3;

    /// <summary>
    /// how much time elapsed after death
    /// </summary>
    protected float deathTime = 0;
    public float TakeDamage(float DamageAmount, IDamageable DamageCauser = null)
    {
        Debug.Log($"Damage : {DamageAmount}");
        BeKilled();
        return DamageAmount;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (base.isDead) 
        {
            if (deathTime < despawnTime) 
            {
                deathTime += Time.deltaTime;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    protected void BeKilled()
    {
        base.isDead = true;
        deathTime = 0;
    }
}
