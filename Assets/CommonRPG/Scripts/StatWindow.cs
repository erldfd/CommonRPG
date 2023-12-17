using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CommonRPG
{
    public class StatWindow : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI simpleHpInfo = null;

        [SerializeField]
        private TextMeshProUGUI simpleMpInfo = null;

        [SerializeField]
        private TextMeshProUGUI simpleDamageInfo = null;

        [SerializeField]
        private TextMeshProUGUI simpleArmorInfo = null;

        [SerializeField]
        private TextMeshProUGUI statPointsInfo = null;

        [SerializeField]
        private TextMeshProUGUI expInfo = null;

        [SerializeField]
        private TextMeshProUGUI detailHpInfo = null;

        [SerializeField]
        private TextMeshProUGUI detailMpInfo = null;

        [SerializeField]
        private TextMeshProUGUI detailDamageInfo = null;

        [SerializeField]
        private TextMeshProUGUI detailArmorInfo = null;

        [SerializeField]
        private Button investToHpButton = null;

        [SerializeField]
        private Button investToMpButton = null;

        [SerializeField]
        private Button investToDamageButton = null;

        [SerializeField]
        private Button investToArmorButton = null;

        [SerializeField]
        private StatComponent statComponent = null;

        private void OnEnable()
        {
            if (statComponent == null) 
            {
                statComponent = GameManager.GetPlayer().StatComponent;
            }

            statComponent.OnLevelUpdate.AddListener(OnLevelUp);
            investToHpButton.onClick.AddListener(OnInvestToHp);
            investToMpButton.onClick.AddListener(OnInvestToMp);
            investToDamageButton.onClick.AddListener(OnInvestToDamage);
            investToArmorButton.onClick.AddListener(OnInvestToArmor);
        }

        private void OnDisable()
        {
            statComponent.OnLevelUpdate.RemoveListener(OnLevelUp);
            investToHpButton.onClick.RemoveListener(OnInvestToHp);
            investToMpButton.onClick.RemoveListener(OnInvestToMp);
            investToDamageButton.onClick.RemoveListener(OnInvestToDamage);
            investToArmorButton.onClick.RemoveListener(OnInvestToArmor);
        }

        public void OpenAndCloseStatWindow()
        {
            gameObject.SetActive(GameManager.IsInventoryOpened());

            UpdateStatWindow();
        }

        public void UpdateStatWindow()
        {
            simpleHpInfo.text = $"{statComponent.CurrentHealthPoint} / {statComponent.TotalHealth}";
            detailHpInfo.text = $"{statComponent.BaseHealthPoint} + {statComponent.StatHpPoint} * {StatComponent.STAT_HP_POINT_COEFFICIENT} + {statComponent.WeaponHealthBonus} + 0";

            simpleMpInfo.text = $"{statComponent.CurrentManaPoint} / {statComponent.TotalMana}";
            detailMpInfo.text = $"{statComponent.BaseManaPoint} + {statComponent.StatMpPoint} * {StatComponent.STAT_MP_POINT_COEFFICIENT} + {statComponent.WeaponManaBonus} + 0";

            simpleDamageInfo.text = $"{statComponent.TotalAttackPower}";
            detailDamageInfo.text = $"{statComponent.BaseAttackPower} + {statComponent.StatAttackPowerPoint} * {StatComponent.STAT_ATTACK_POWER_POINT_COEFFICIENT} + {statComponent.WeaponAttackPowerBonus} + 0";

            simpleArmorInfo.text = $"{statComponent.TotalAttackPower}";
            detailArmorInfo.text = $"{statComponent.BaseDefense} + {statComponent.StatDefensePoint} * {StatComponent.STAT_DEFENSE_POINT_COEFFICIENT} + {statComponent.WeaponDefenseBonus} + 0";

            statPointsInfo.text = statComponent.UninvestedStatPoint.ToString();
            expInfo.text = $"{statComponent.CurrentExp:F1} / {statComponent.MaxExpOfCurrentLevel:F1}";
        }

        private void OnInvestToHp()
        {
            statComponent.InvestStatPointToHp(1);
            simpleHpInfo.text = $"{statComponent.CurrentHealthPoint} / {statComponent.TotalHealth}";
            detailHpInfo.text = $"{statComponent.BaseHealthPoint} + {statComponent.StatHpPoint} * {StatComponent.STAT_HP_POINT_COEFFICIENT} + {statComponent.WeaponHealthBonus} + 0";
            statPointsInfo.text = statComponent.UninvestedStatPoint.ToString();

            GameManager.SetPlayerHealthBarFillRatio(statComponent.CurrentHealthPoint / statComponent.TotalHealth);
        }

        private void OnInvestToMp()
        {
            statComponent.InvestStatPointToMp(1);
            simpleMpInfo.text = $"{statComponent.CurrentManaPoint} / {statComponent.TotalMana}";
            detailMpInfo.text = $"{statComponent.BaseManaPoint} + {statComponent.StatMpPoint} * {StatComponent.STAT_MP_POINT_COEFFICIENT} + {statComponent.WeaponManaBonus} + 0";
            statPointsInfo.text = statComponent.UninvestedStatPoint.ToString();

            GameManager.SetPlayerManaBarFillRatio(statComponent.CurrentManaPoint / statComponent.TotalMana);
        }

        private void OnInvestToDamage()
        {
            statComponent.InvestStatPointToAttackPower(1);
            simpleDamageInfo.text = $"{statComponent.TotalAttackPower}";
            detailDamageInfo.text = $"{statComponent.BaseAttackPower} + {statComponent.StatAttackPowerPoint} * {StatComponent.STAT_ATTACK_POWER_POINT_COEFFICIENT} + {statComponent.WeaponAttackPowerBonus} + 0";
            statPointsInfo.text = statComponent.UninvestedStatPoint.ToString();
        }

        private void OnInvestToArmor()
        {
            statComponent.InvestStatPointToDefense(1);
            simpleArmorInfo.text = $"{statComponent.TotalDefense}";
            detailArmorInfo.text = $"{statComponent.BaseDefense} + {statComponent.StatDefensePoint} * {StatComponent.STAT_DEFENSE_POINT_COEFFICIENT} + {statComponent.WeaponDefenseBonus} + 0";
            statPointsInfo.text = statComponent.UninvestedStatPoint.ToString();
        }

        /// <summary>
        /// arg : int amount - Amount of level increase at once
        /// </summary>
        private void OnLevelUp(int amount)
        {
            statPointsInfo.text = statComponent.UninvestedStatPoint.ToString();
            expInfo.text = $"{statComponent.CurrentExp:F1} / {statComponent.MaxExpOfCurrentLevel:F1}";
            Debug.Log("Level Up!!!!!!!!!!!!!!!!!!");
        }



    }
}


