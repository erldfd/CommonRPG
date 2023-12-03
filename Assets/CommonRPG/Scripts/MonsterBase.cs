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

        statComponenet.CurrentHealthPoint -= DamageAmount;
        float currentHpRatio = Mathf.Clamp01(statComponenet.CurrentHealthPoint / statComponenet.TotalHealth);
        GameManager.SetMonsterHealthBarFillRatio(currentHpRatio);
        GameManager.SetMonsterInfoUIVisible(true);
        if (currentHpRatio <= 0) 
        {
            BeKilled();
        }

        return DamageAmount;
    }

    // Start is called before the first frame update
    void Start()
    {
        statComponenet.TotalHealth = 5;
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
