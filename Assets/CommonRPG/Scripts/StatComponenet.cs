using UnityEngine;
using UnityEngine.Events;

public class StatComponenet : MonoBehaviour
{
    [SerializeField]
    private int level = 0;
    public int Level
    {
        get
        {
            return level;
        }
        set
        {
            level = value;
            OnLevelUpdate.Invoke(level);
        }
    }

    /// <summary>
    /// arg : int LevelUpdated
    /// </summary>
    [Tooltip("arg : int LevelUpdated")]
    public UnityEvent<int> OnLevelUpdate = null;

    [SerializeField]
    private float baseHealthPoint = 1;
    public float BaseHealthPoint
    {
        get 
        {
            return baseHealthPoint; 
        }
        set 
        { 
            baseHealthPoint = value;
            totalHealth = baseHealthPoint + weaponHealthBonus;

            OnBaseHealthUpdate.Invoke(baseHealthPoint);
        }
    }

    /// <summary>
    /// arg : float BaseHealthPointUpdated
    /// </summary>
    [Tooltip("arg : float BaseHealthPointUpdated")]
    public UnityEvent<float> OnBaseHealthUpdate = null;

    [SerializeField]
    private float baseManaPoint = 1;
    public float BaseManaPoint
    {
        get
        {
            return baseManaPoint;
        }
        set
        {
            baseManaPoint = value;
            totalMana = baseManaPoint + weaponManaBonus;

            OnBaseManaUpdate.Invoke(baseManaPoint);
        }
    }

    /// <summary>
    /// arg : float BaseManaPointUpdated
    /// </summary>
    [Tooltip("arg : float ManaPointUpdated")]
    public UnityEvent<float> OnBaseManaUpdate = null;

    [SerializeField]
    private float baseAttackPower = 1;
    public float BaseAttackPower
    {
        get
        {
            return baseAttackPower;
        }
        set
        {
            baseAttackPower = value;
            totalAttackPower = baseAttackPower + weaponAttackPowerBonus;

            OnBaseAttackPowerUpdate.Invoke(baseAttackPower);
        }
    }

    /// <summary>
    /// arg : float baseAttackPowerUpdated
    /// </summary>
    [Tooltip("arg : float baseAttackPower")]
    public UnityEvent<float> OnBaseAttackPowerUpdate = null;

    [SerializeField]
    private float baseDefense = 0;
    public float BaseDefense
    {
        get
        {
            return baseDefense;
        }
        set
        {
            baseDefense = value;
            totalDefense = baseDefense + weaponDefenseBonus;

            OnBaseDefenseUpdate.Invoke(baseDefense);
        }
    }

    /// <summary>
    /// arg : float baseDefenseUpdated
    /// </summary>
    [Tooltip("arg : float baseDefense")]
    public UnityEvent<float> OnBaseDefenseUpdate = null;

    [SerializeField]
    private float weaponHealthBonus = 0;
    public float WeaponHealthBonus
    {
        get
        {
            return weaponHealthBonus;
        }
        set
        {
            weaponHealthBonus = value;
            totalHealth = baseHealthPoint + weaponHealthBonus;
        }
    }

    [SerializeField]
    private float weaponManaBonus = 0;
    public float WeaponManaBonus
    {
        get
        {
            return weaponManaBonus;
        }
        set
        {
            weaponManaBonus = value;
            totalMana = baseManaPoint + weaponManaBonus;
        }
    }

    [SerializeField]
    private float weaponAttackPowerBonus = 0;
    public float WeaponAttackPowerBonus
    {
        get
        {
            return weaponAttackPowerBonus;
        }
        set
        {
            weaponAttackPowerBonus = value;
            totalAttackPower = baseAttackPower + weaponAttackPowerBonus;
        }
    }

    [SerializeField]
    private float weaponDefenseBonus = 0;
    public float WeaponDefenseBonus
    {
        get
        {
            return weaponDefenseBonus;
        }
        set
        {
            weaponDefenseBonus = value;
        }
    }

    [SerializeField]
    private float totalHealth = 1;
    public float TotalHealth
    {
        get
        {
            return totalHealth;
        }
        set
        {
            totalHealth = value;
        }
    }

    [SerializeField]
    private float totalMana = 1;
    public float TotalMana
    {
        get
        {
            return totalMana;
        }
        set
        {
            totalMana = value;
        }
    }

    [SerializeField]
    private float totalAttackPower = 1;
    public float TotalAttackPower
    {
        get
        {
            return totalAttackPower;
        }
        set
        {
            totalAttackPower = value;
        }
    }

    [SerializeField]
    private float totalDefense = 0;
    public float TotalDefense
    {
        get
        {
            return totalDefense;
        }
        set
        {
            totalDefense = value;
        }
    }
}
