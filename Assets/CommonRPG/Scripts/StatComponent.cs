using UnityEngine;
using UnityEngine.Events;

namespace CommonRPG
{
    public enum EStatType
    {
        Hp,
        Mp,
        AttackPower,
        Defense
    }

    public class StatComponent : MonoBehaviour
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
                int previousLevel = level;
                level = value;

                UninvestedStatPoint += UNINVESTED_STAT_POINT_PER_LEVEL_UP * (level - previousLevel);
                MaxExpOfCurrentLevel = GameManager.GetLevelMaxExpData(level);

                CurrentHealthPoint = totalHealth;
                CurrentManaPoint = totalMana;

                GameManager.InGameUI.SetPlayerHealthBarFillRatio(CurrentHealthPoint / totalHealth);
                GameManager.InGameUI.SetPlayerManaBarFillRatio(CurrentManaPoint / totalMana);
                GameManager.InGameUI.SetPlayerLevelText(level);

                OnLevelUpdate.Invoke(level - previousLevel);
            }
        }

        private const int UNINVESTED_STAT_POINT_PER_LEVEL_UP = 4;

        /// <summary>
        /// arg : int Added Level
        /// </summary>
        [Tooltip("arg : int Added Level")]
        public UnityEvent<int> OnLevelUpdate = null;

        [SerializeField]
        private float currentExp = 0;
        public float CurrentExp
        {
            get
            {
                return currentExp;
            }
            set
            {
                currentExp = value;

                if (currentExp >= MaxExpOfCurrentLevel)
                {
                    currentExp -= MaxExpOfCurrentLevel;
                    Level += 1;
                }
            }
        }

        [SerializeField]
        private float maxExpOfCurrentLevel = 10;
        public float MaxExpOfCurrentLevel
        {
            get
            {
                return maxExpOfCurrentLevel;
            }
            set
            {
                maxExpOfCurrentLevel = value;
            }
        }

        [SerializeField]
        private int uninvestedStatPoint = 0;
        public int UninvestedStatPoint
        {
            get
            {
                return uninvestedStatPoint;
            }
            set
            {
                uninvestedStatPoint = value;
            }
        }

        [SerializeField]
        private int statHpPoint = 0;
        public int StatHpPoint
        {
            get
            {
                return statHpPoint;
            }
            set
            {
                statHpPoint = value;
                StatBonusHealthPoint = statHpPoint * STAT_HP_POINT_COEFFICIENT;
            }
        }

        public const float STAT_HP_POINT_COEFFICIENT = 10;

        [SerializeField]
        private int statMpPoint = 0;
        public int StatMpPoint
        {
            get
            {
                return statMpPoint;
            }
            set
            {
                statMpPoint = value;
                StatBonusManaPoint = statMpPoint * STAT_MP_POINT_COEFFICIENT;
            }
        }

        public const float STAT_MP_POINT_COEFFICIENT = 10;

        [SerializeField]
        private int statAttackPowerPoint = 0;
        public int StatAttackPowerPoint
        {
            get
            {
                return statAttackPowerPoint;
            }
            set
            {
                statAttackPowerPoint = value;
                StatBonusAttackPower = statAttackPowerPoint * STAT_ATTACK_POWER_POINT_COEFFICIENT;
            }
        }

        public const float STAT_ATTACK_POWER_POINT_COEFFICIENT = 2;

        [SerializeField]
        private int statDefensePoint = 0;
        public int StatDefensePoint
        {
            get
            {
                return statDefensePoint;
            }
            set
            {
                statDefensePoint = value;
                StatBonusDefense = statDefensePoint * STAT_DEFENSE_POINT_COEFFICIENT;
            }
        }

        public const float STAT_DEFENSE_POINT_COEFFICIENT = 1;

        [SerializeField]
        private float currentHealthPoint = 1;
        public float CurrentHealthPoint
        {
            get
            {
                return currentHealthPoint;
            }
            set
            {
                currentHealthPoint = value;
            }
        }

        [SerializeField]
        private float currentManaPoint = 1;
        public float CurrentManaPoint
        {
            get
            {
                return currentManaPoint;
            }
            set
            {
                currentManaPoint = value;
                GameManager.InGameUI.SetPlayerManaBarFillRatio(CurrentManaPoint / TotalMana);
            }
        }

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
                TotalHealth = baseHealthPoint + weaponHealthBonus + statBonusHealthPoint;
            }
        }

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
                TotalMana = baseManaPoint + weaponManaBonus + statBonusManaPoint;
            }
        }

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
                TotalAttackPower = baseAttackPower + weaponAttackPowerBonus + statBonusAttackPower;
            }
        }

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
                TotalDefense = baseDefense + weaponDefenseBonus + statBonusDefense;
            }
        }

        [SerializeField]
        private float statBonusHealthPoint = 0;
        public float StatBonusHealthPoint
        {
            get
            {
                return statBonusHealthPoint;
            }
            set
            {
                statBonusHealthPoint = value;
                TotalHealth = statBonusHealthPoint + baseHealthPoint + weaponHealthBonus;
            }
        }

        [SerializeField]
        private float statBonusManaPoint = 0;
        public float StatBonusManaPoint
        {
            get
            {
                return statBonusManaPoint;
            }
            set
            {
                statBonusManaPoint = value;
                TotalMana = statBonusManaPoint + baseManaPoint + weaponManaBonus;
            }
        }

        [SerializeField]
        private float statBonusAttackPower = 0;
        public float StatBonusAttackPower
        {
            get
            {
                return statBonusAttackPower;
            }
            set
            {
                statBonusAttackPower = value;
                TotalAttackPower = statBonusAttackPower + baseAttackPower + weaponAttackPowerBonus;
            }
        }

        [SerializeField]
        private float statBonusDefense = 0;
        public float StatBonusDefense
        {
            get
            {
                return statBonusDefense;
            }
            set
            {
                statBonusDefense = value;
                TotalDefense = statBonusDefense + baseDefense + weaponDefenseBonus;
            }
        }

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
                TotalHealth = baseHealthPoint + weaponHealthBonus + statBonusHealthPoint;
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
                TotalMana = baseManaPoint + weaponManaBonus + statBonusManaPoint;
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
                TotalAttackPower = baseAttackPower + weaponAttackPowerBonus + statBonusAttackPower;
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
                TotalDefense = baseDefense + weaponDefenseBonus + statBonusDefense;
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

        public void InvestStatPointToHp(int amount)
        {
            InvestStatPointTo(EStatType.Hp, amount);
        }
        public void InvestStatPointToMp(int amount)
        {
            InvestStatPointTo(EStatType.Mp, amount);
        }

        public void InvestStatPointToAttackPower(int amount)
        {
            InvestStatPointTo(EStatType.AttackPower, amount);
        }

        public void InvestStatPointToDefense(int amount)
        {
            InvestStatPointTo(EStatType.Defense, amount);
        }

        public void InvestStatPointTo(EStatType statType, int statPointsToInvest)
        {
            if (UninvestedStatPoint < statPointsToInvest)
            {
                return;
            }

            for (int i = 0; i < statPointsToInvest; i++) 
            {
                switch (statType)
                {
                    case EStatType.Hp:
                    {
                        StatHpPoint++;
                        break;
                    }
                    case EStatType.Mp:
                    {
                        StatMpPoint++;
                        break;
                    }
                    case EStatType.AttackPower:
                    {
                        StatAttackPowerPoint++;
                        break;
                    }
                    case EStatType.Defense:
                    {
                        StatDefensePoint++;
                        break;
                    }
                    default:
                    {
                        Debug.LogError("WeirdStatType Detected");
                        break;
                    }
                }

                UninvestedStatPoint--;
            }
            
        }
    }

}
