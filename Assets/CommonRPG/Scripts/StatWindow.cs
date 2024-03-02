using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CommonRPG
{
    public class StatWindow : MonoBehaviour
    {
        [Header("Simple Infos")]
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

        [Header("Detail Infos")]
        [SerializeField]
        private TextMeshProUGUI detailHpInfo = null;

        [SerializeField]
        private TextMeshProUGUI detailMpInfo = null;

        [SerializeField]
        private TextMeshProUGUI detailDamageInfo = null;

        [SerializeField]
        private TextMeshProUGUI detailArmorInfo = null;

        [Header("Buttons")]
        [SerializeField]
        private Button investToHpButton = null;

        [SerializeField]
        private Button investToMpButton = null;

        [SerializeField]
        private Button investToDamageButton = null;

        [SerializeField]
        private Button investToArmorButton = null;

        [Header("etc.")]
        [SerializeField]
        private StatComponent statComponent = null;

        [SerializeField]
        private Sprite activatedInvestButtonSprite;

        [SerializeField]
        private Sprite deactivatedInvestButtonSprite;

        private void OnEnable()
        {
            if (statComponent == null) 
            {
                statComponent = GameManager.GetPlayerCharacter().StatComponent;
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

            simpleArmorInfo.text = $"{statComponent.TotalDefense}";
            detailArmorInfo.text = $"{statComponent.BaseDefense} + {statComponent.StatDefensePoint} * {StatComponent.STAT_DEFENSE_POINT_COEFFICIENT} + {statComponent.WeaponDefenseBonus} + 0";

            statPointsInfo.text = statComponent.UninvestedStatPoint.ToString();
            expInfo.text = $"{statComponent.CurrentExp:F1} / {statComponent.MaxExpOfCurrentLevel:F1}";

            float currentExpRatio = statComponent.CurrentExp / statComponent.MaxExpOfCurrentLevel;
            GameManager.InGameUI.SetPlayerExpBarFillRatio(currentExpRatio);

            SetActiveInvestButtons(statComponent.UninvestedStatPoint > 0);
        }

        private void SetActiveInvestButtons(bool shouldActivate)
        {
            if (shouldActivate) 
            {
                investToArmorButton.image.sprite = activatedInvestButtonSprite;
                investToDamageButton.image.sprite = activatedInvestButtonSprite;
                investToHpButton.image.sprite = activatedInvestButtonSprite;
                investToMpButton.image.sprite = activatedInvestButtonSprite;
            }
            else
            {
                investToArmorButton.image.sprite = deactivatedInvestButtonSprite;
                investToDamageButton.image.sprite = deactivatedInvestButtonSprite;
                investToHpButton.image.sprite = deactivatedInvestButtonSprite;
                investToMpButton.image.sprite = deactivatedInvestButtonSprite;
            }
        }

        private void OnInvestToHp()
        {
            statComponent.InvestStatPointToHp(1);
            simpleHpInfo.text = $"{statComponent.CurrentHealthPoint} / {statComponent.TotalHealth}";
            detailHpInfo.text = $"{statComponent.BaseHealthPoint} + {statComponent.StatHpPoint} * {StatComponent.STAT_HP_POINT_COEFFICIENT} + {statComponent.WeaponHealthBonus} + 0";
            statPointsInfo.text = statComponent.UninvestedStatPoint.ToString();

            SetActiveInvestButtons(statComponent.UninvestedStatPoint > 0);

            GameManager.InGameUI.SetPlayerHealthBarFillRatio(statComponent.CurrentHealthPoint / statComponent.TotalHealth);
        }

        private void OnInvestToMp()
        {
            statComponent.InvestStatPointToMp(1);
            simpleMpInfo.text = $"{statComponent.CurrentManaPoint} / {statComponent.TotalMana}";
            detailMpInfo.text = $"{statComponent.BaseManaPoint} + {statComponent.StatMpPoint} * {StatComponent.STAT_MP_POINT_COEFFICIENT} + {statComponent.WeaponManaBonus} + 0";
            statPointsInfo.text = statComponent.UninvestedStatPoint.ToString();

            SetActiveInvestButtons(statComponent.UninvestedStatPoint > 0);

            GameManager.InGameUI.SetPlayerManaBarFillRatio(statComponent.CurrentManaPoint / statComponent.TotalMana);
        }

        private void OnInvestToDamage()
        {
            statComponent.InvestStatPointToAttackPower(1);
            simpleDamageInfo.text = $"{statComponent.TotalAttackPower}";
            detailDamageInfo.text = $"{statComponent.BaseAttackPower} + {statComponent.StatAttackPowerPoint} * {StatComponent.STAT_ATTACK_POWER_POINT_COEFFICIENT} + {statComponent.WeaponAttackPowerBonus} + 0";
            statPointsInfo.text = statComponent.UninvestedStatPoint.ToString();

            SetActiveInvestButtons(statComponent.UninvestedStatPoint > 0);
        }

        private void OnInvestToArmor()
        {
            statComponent.InvestStatPointToDefense(1);
            simpleArmorInfo.text = $"{statComponent.TotalDefense}";
            detailArmorInfo.text = $"{statComponent.BaseDefense} + {statComponent.StatDefensePoint} * {StatComponent.STAT_DEFENSE_POINT_COEFFICIENT} + {statComponent.WeaponDefenseBonus} + 0";
            statPointsInfo.text = statComponent.UninvestedStatPoint.ToString();

            SetActiveInvestButtons(statComponent.UninvestedStatPoint > 0);
        }

        /// <summary>
        /// arg : int amount - Amount of level increase at once
        /// </summary>
        private void OnLevelUp(int amount)
        {
            statPointsInfo.text = statComponent.UninvestedStatPoint.ToString();
            expInfo.text = $"{statComponent.CurrentExp:F1} / {statComponent.MaxExpOfCurrentLevel:F1}";

            float currentExpRatio = statComponent.CurrentExp / statComponent.MaxExpOfCurrentLevel;
            GameManager.InGameUI.SetPlayerExpBarFillRatio(currentExpRatio);

            SetActiveInvestButtons(statComponent.UninvestedStatPoint > 0);
            Debug.Log("Level Up!!!!!!!!!!!!!!!!!!");
        }



    }
}


