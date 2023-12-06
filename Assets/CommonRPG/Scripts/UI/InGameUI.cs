using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CommonRPG
{
    public class InGameUI : MonoBehaviour
    {
        [Header("MonsterUI")]
        [SerializeField]
        private GameObject monsterInfoUI = null;

        [SerializeField]
        private TextMeshProUGUI monsterNameText = null;

        [SerializeField]
        private ProgressBar monsterHealthBar = null;

        [Header("PlayerUI")]
        [SerializeField]
        private GameObject playerInfoUI = null;

        [SerializeField]
        private TextMeshProUGUI playerNameText = null;

        [SerializeField]
        private ProgressBar playerHealthBar = null;

        [SerializeField]
        private ProgressBar playerManaBar = null;

        [SerializeField]
        private TextMeshProUGUI playerLevelText = null;

        [Header("Etc.")]
        [SerializeField]
        private GameObject pauseUI = null;

        private void Awake()
        {
            Debug.Assert(monsterInfoUI);
            Debug.Assert(monsterNameText);
            Debug.Assert(monsterHealthBar);

            Debug.Assert(playerInfoUI);
            Debug.Assert(playerHealthBar);
            Debug.Assert(playerManaBar);
            Debug.Assert(playerLevelText);

            Debug.Assert(pauseUI);
        }

        public void SetMonsterInfoUIVisible(bool shouldVisible)
        {
            monsterInfoUI.SetActive(shouldVisible);
        }

        public void SetMonsterNameText(string NewName)
        {
            monsterNameText.SetText(NewName);
        }

        public void SetMonsterHealthBarFillRatio(float ratio)
        {
            ratio = Mathf.Clamp01(ratio);
            monsterHealthBar.FillAmount = ratio;
        }

        public void SetPlayerInfoUIVisible(bool shouldVisible)
        {
            playerInfoUI.SetActive(shouldVisible);
        }

        public void SetPlayerNameText(string NewName)
        {
            playerNameText.SetText(NewName);
        }

        public void SetPlayerHealthBarFillRatio(float ratio)
        {
            ratio = Mathf.Clamp01(ratio);
            playerHealthBar.FillAmount = ratio;
        }

        public void SetPlayerManaBarFillRatio(float ratio)
        {
            ratio = Mathf.Clamp01(ratio);
            playerManaBar.FillAmount = ratio;
        }

        public void SetPlayerLevelText(int NewLevel)
        {
            playerLevelText.SetText(NewLevel.ToString());
        }

        public void SetPauseUIVisible(bool shouldVisible)
        {
            pauseUI.SetActive(shouldVisible);
        }

        public void OnReturnToMenuButtonClicked()
        {
            Debug.Log("Return to Menu Button Clicled.");
            SceneManager.LoadSceneAsync(0);
        }
    }

}
