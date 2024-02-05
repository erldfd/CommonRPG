using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CommonRPG
{
    public class InGameUI : MonoBehaviour
    {
        public bool IsConversationStarted { get { return conversationUI.IsConversationStarted; } }

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

        [SerializeField]
        private GameObject interactionUI = null;

        [Header("Conversation")]
        [SerializeField]
        private ConversationUI conversationUI = null;

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

            Debug.Assert(interactionUI);

            Debug.Assert(conversationUI);

            Debug.Assert(pauseUI);

            //DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            //conversationUI.StartConversation();
            //GameManager.TimerManager.PauseGameWorld(true);
            //Cursor.visible = true;
            //Cursor.lockState = (true) ? CursorLockMode.Confined : CursorLockMode.Locked;
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
            Debug.Log("Return to Menu Button Clicked.");
            SceneManager.LoadSceneAsync(0);
        }

        public void SetActiveInteractioUI(bool ShouldActivate)
        {
            interactionUI.SetActive(ShouldActivate);
        }

        public void ReadyToConversate(ConversationDataScriptableObject conversationData)
        {
            conversationUI.ConversationData = conversationData;

            conversationUI.StartConversation();
            GameManager.TimerManager.PauseGameWorld(true);
            Cursor.visible = true;
            Cursor.lockState = (true) ? CursorLockMode.Confined : CursorLockMode.Locked;
        }

        /// <summary>
        /// args -> string : ConversationName, int : Covnersation Node Id, int : ClickedButtonIndex
        /// </summary>
        public void BindEventToOnChoiceConversationButtonClickedDelegate(in Action<string, int, int> actionToBind)
        {
            conversationUI.OnChoiceConversationButtonClickedDelegate += actionToBind;
        }

        /// <summary>
        /// args -> string : ConversationName, int : Covnersation Node Id, int : ClickedButtonIndex
        /// </summary>
        public void RemoveEventToOnChoiceConversationButtonClickedDelegate(in Action<string, int, int> actionToRemove)
        {
            conversationUI.OnChoiceConversationButtonClickedDelegate -= actionToRemove;
        }

        /// <summary>
        /// string : ConversationName, int : Covnersation Node Id
        /// </summary>
        public void BindEventToOnConversationFinishedDelegate(in Action<string, int> actionToBind)
        {
            conversationUI.OnConversationFinishedDelegate += actionToBind;
        }

        /// <summary>
        /// string : ConversationName, int : Covnersation Node Id
        /// </summary>
        public void RemoveEventToOnConversationFinishedDelegate(in Action<string, int> actionToRemove)
        {
            conversationUI.OnConversationFinishedDelegate -= actionToRemove;
        }
    }

}
