using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CommonRPG
{
    public class InGameUI : MonoBehaviour
    {
        public bool IsConversationStarted { get { return conversationUI.IsConversationStarted; } }

        [Header("Monster UI")]
        [SerializeField]
        private GameObject monsterInfoUI = null;

        [SerializeField]
        private TextMeshProUGUI monsterNameText = null;

        [SerializeField]
        private ProgressBar monsterHealthBar = null;

        [SerializeField]
        private float monsterHpBarAfterImageChangeSpeed = 1;

        [Header("Player UI")]
        [SerializeField]
        private GameObject playerInfoUI = null;

        [SerializeField]
        private TextMeshProUGUI playerNameText = null;

        [SerializeField]
        private ProgressBar playerHealthBar = null;

        [SerializeField]
        private ProgressBar playerManaBar = null;

        [SerializeField]
        private ProgressBar playerExpBar = null;

        [SerializeField]
        private TextMeshProUGUI playerLevelText = null;

        [SerializeField]
        private GameObject interactionUI = null;

        [Header("Conversation UI")]
        [SerializeField]
        private ConversationUI conversationUI = null;

        [Header("Billboard UI")]
        [SerializeField]
        private Canvas billboardCanvas;

        [SerializeField]
        private DamageNumber damageNumberPrefab;

        [Header("Etc.")]
        [SerializeField]
        private GameObject pauseUI = null;

        private TimerHandler monsterHealthBarAfterimageStayTimerHandler = null;

        private bool isStartedAfterimageHpChanging = false;
        private float afterimageHpRatio = 0;
        private float currentHpRatio = 0;

        private MonsterBase lastDisplayedMonster = null;

        private Queue<DamageNumber> activatedDamageNumberQueue = new Queue<DamageNumber>();
        private Queue<DamageNumber> deactivatedDamageNumberQueue = new Queue<DamageNumber>();

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

            Debug.Assert(billboardCanvas);
            Debug.Assert(damageNumberPrefab);

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

        private void Update()
        {
            if (isStartedAfterimageHpChanging && currentHpRatio < afterimageHpRatio) 
            {
                afterimageHpRatio -= monsterHpBarAfterImageChangeSpeed * Time.deltaTime * 0.1f;

                // index 1 : afterImage hp bar fill ratio
                SetMonsterHealthBarFillRatio(1, afterimageHpRatio);

                if (currentHpRatio >= afterimageHpRatio) 
                {
                    isStartedAfterimageHpChanging = false;
                    lastDisplayedMonster = null;
                }
            }
        }

        public void SetMonsterInfoUIVisible(bool shouldVisible)
        {
            monsterInfoUI.SetActive(shouldVisible);
        }

        public void SetMonsterNameText(string NewName)
        {
            monsterNameText.SetText(NewName);
        }

        /// <summary>
        ///  <para> index : index of progressBarImageList. </para>
        ///  you can multi progressBar if you want.
        ///  first progressBar index is 0, second is 1, ect...
        /// </summary>
        public void SetMonsterHealthBarFillRatio(int index, float ratio)
        {
            ratio = Mathf.Clamp01(ratio);
            monsterHealthBar.SetProgressBarFillAmount(index, ratio);
        }

        /// <summary>
        /// <para> Decrease Monster Hp bar with afterimage </para>
        /// <para> currentRatio : Current Hp ratio </para>
        /// <para> beforeRatio : hp ration before damage </para>
        /// <para> afterimageChangeStartTime : after this time, afterimage start to change </para>
        /// </summary>
        public void DisplayDecrasingMonsterHealthBar(float currentRatio, float beforeRatio, float afterimageChangeStartTime, MonsterBase monster)
        {
            // index 0 : current hp bar fill ratio
            SetMonsterHealthBarFillRatio(0, currentRatio);
            currentHpRatio = currentRatio;

            if (lastDisplayedMonster == null || lastDisplayedMonster != monster)
            {
                lastDisplayedMonster = monster;

                // index 1 : afterImage hp bar fill ratio
                SetMonsterHealthBarFillRatio(1, beforeRatio);
                afterimageHpRatio = beforeRatio;
                isStartedAfterimageHpChanging = false;
            }
            
            
            //GameManager.TimerManager.SetTimer(afterimageChangeStartTime, 0, 0, () =>
            //{
            //    isStartedAfterimageHpChanging = true;

            //}, true);

            //if (monsterHealthBarAfterimageStayTimerHandler == null)
            //{
            //    monsterUITimerHandler = GameManager.SetTimer(3, 1, 0, () => { GameManager.InGameUI.SetMonsterInfoUIVisible(false); }, true);
            //    monsterUITimerHandler.IsStayingActive = true;
            //}
            //else
            //{
            //    monsterUITimerHandler.RestartTimer();
            //}

            if (monsterHealthBarAfterimageStayTimerHandler == null)
            {
                monsterHealthBarAfterimageStayTimerHandler = GameManager.TimerManager.SetTimer(afterimageChangeStartTime, 0, 0, () =>
                {
                    isStartedAfterimageHpChanging = true;

                }, true);
            }
            else
            {
                monsterHealthBarAfterimageStayTimerHandler.ResetTimer(afterimageChangeStartTime, 0, 0, () =>
                {
                    isStartedAfterimageHpChanging = true;

                }, true);

                monsterHealthBarAfterimageStayTimerHandler.RestartTimer();
            }

            monsterHealthBarAfterimageStayTimerHandler.IsStayingActive = true;
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
            playerHealthBar.SetProgressBarFillAmount(0, ratio);
        }

        public void SetPlayerManaBarFillRatio(float ratio)
        {
            ratio = Mathf.Clamp01(ratio);
            playerManaBar.SetProgressBarFillAmount(0, ratio);
        }

        public void SetPlayerExpBarFillRatio(float ratio)
        {
            ratio = Mathf.Clamp01(ratio);
            playerExpBar.SetProgressBarFillAmount(0, ratio);
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
            if (conversationData == null) 
            {
                return;
            }

            conversationUI.ConversationData = conversationData;

            conversationUI.StartConversation();
            //GameManager.TimerManager.TryPuaseGameWorld();
            //Cursor.visible = true;
            //Cursor.lockState = (true) ? CursorLockMode.Confined : CursorLockMode.Locked;
            GameManager.TryUseOrNotUIInteractionState();
        }

        public void DisplayDamageNumber(float damageAmount, Vector3 position)
        {
            SpawnDamageNumber(damageAmount, position);
        }

        private void SpawnDamageNumber(float damageAmount, Vector3 position)
        {
            DamageNumber damageNumber = null;

            if (deactivatedDamageNumberQueue.Count > 0) 
            {
                damageNumber = deactivatedDamageNumberQueue.Dequeue();
                damageNumber.gameObject.SetActive(true);
            }
            else
            {
                damageNumber = Instantiate(damageNumberPrefab);
            }

            damageNumber.transform.SetParent(billboardCanvas.transform);

            damageNumber.SetDamageText(damageAmount);
            damageNumber.transform.position = position;

            Quaternion billboardRotation = Camera.main.transform.rotation;
            billboardRotation = new Quaternion(0, billboardRotation.y, 0, billboardRotation.w);
            damageNumber.transform.rotation = billboardRotation;

            //activatedDamageNumberQueue.Enqueue(damageNumber);

            GameManager.TimerManager.SetTimer(damageNumber.LifeTime, 0, 0, () =>
            {
                DespawnDamageNumber(damageNumber);

            }, true);
        }

        private void DespawnDamageNumber(DamageNumber damageNumber)
        {
            damageNumber.gameObject.SetActive(false);
            deactivatedDamageNumberQueue.Enqueue(damageNumber);
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
