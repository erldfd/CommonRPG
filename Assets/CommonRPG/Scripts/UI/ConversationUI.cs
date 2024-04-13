using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CommonRPG.ConversationDataScriptableObject;

namespace CommonRPG
{
    public class ConversationUI : MonoBehaviour
    {

        private const int MAX_CHOICE_BUTTON_COUNT = 4;

        private enum EAudioClipList
        {
            NextConversation
        }

        [SerializeField]
        private AudioContainer conversationSoundContainer;

        /// <summary>
        /// string : ConversationName, int : Covnersation Node Id
        /// </summary>
        public event Action<string, int> OnNornalConversationDelegate;

        /// <summary>
        /// string : ConversationName, int : Covnersation Node Id
        /// </summary>
        public event Action<string, int> OnChoiceConversationDelegate;

        /// <summary>
        /// string : ConversationName, int : Covnersation Node Id, int : ClickedButtonIndex
        /// </summary>
        public event Action<string, int, int> OnChoiceConversationButtonClickedDelegate;

        /// <summary>
        /// string : ConversationName, int : Covnersation Node Id
        /// </summary>
        public event Action<string, int> OnConversationFinishedDelegate;

        [SerializeField]
        private GameObject background;

        [SerializeField]
        private TextMeshProUGUI nameText;

        [SerializeField]
        private TextMeshProUGUI normalConversationText;

        [SerializeField]
        private GameObject choiceConversationTextButtonsGameObject;

        [SerializeField]
        private List<Button> choiceButtonList;

        private List<TextMeshProUGUI> choiceButtonTextList = new List<TextMeshProUGUI>();

        [SerializeField]
        private Button nextConversationButton;

        [SerializeField]
        private ConversationDataScriptableObject conversationData;
        public ConversationDataScriptableObject ConversationData { get { return conversationData; } set { conversationData = value; } }

        private bool isConversationStarted = false;
        public bool IsConversationStarted { get { return isConversationStarted; } }

        private int currentConversationId = 0;

        private void Awake()
        {
            Debug.Assert(conversationSoundContainer);

            choiceButtonTextList.Clear();

            foreach (Button button in choiceButtonList) 
            {
                choiceButtonTextList.Add(button.transform.GetChild(0).GetComponent<TextMeshProUGUI>());
            }

            background.SetActive(false);
        }

        public void StartConversation()
        {
            currentConversationId = 0;
            conversationData.ReadyToStart();

            isConversationStarted = true;

            StartNextConversation();

            background.SetActive(true);
        }

        public void OnNextConversationButtonClicked()
        {
            Dictionary<int, ConversationNode> conversationTable = conversationData.ConversationTable;
            ConversationNode conversationNode = conversationTable[currentConversationId];

            if (GameManager.InGameUI.IsSettingText) 
            {
                GameManager.InGameUI.SetTextAtOnce(normalConversationText, conversationNode.Conversations[0].ToCharArray());
                return;
            }

            if (conversationNode.ChildrenIDs.Count == 1)
            {
                currentConversationId = conversationNode.ChildrenIDs[0];
                StartNextConversation();
            }
        }

        public void OnChoiceConversationButtonClicked(int index)
        {
            Dictionary<int, ConversationNode> conversationTable = conversationData.ConversationTable;
            ConversationNode conversationNode = conversationTable[currentConversationId];

            if(OnChoiceConversationButtonClickedDelegate !=null)
            {
                OnChoiceConversationButtonClickedDelegate.Invoke(conversationData.ConversationDataName, currentConversationId, index);
            }

            currentConversationId = conversationNode.ChildrenIDs[index];
            StartNextConversation();
        }

        private void StartNextConversation()
        {
            Dictionary<int, ConversationNode> conversationTable = conversationData.ConversationTable;
            ConversationNode conversationNode = conversationTable[currentConversationId];

            GameManager.AudioManager.PlayAudio2D(conversationSoundContainer.AudioClipList[(int)EAudioClipList.NextConversation], 1);

            if (conversationNode.MyID == 0)
            {
                currentConversationId = conversationNode.ChildrenIDs[0];
                StartNextConversation();
                Open();

                return;
            }
            else if (conversationNode.ChildrenIDs.Count == 0)
            {
                FinishConversation();
                
                // finish
                return;
            }

            nameText.text = conversationNode.SpeakerName;

            int childrenCount = conversationNode.ChildrenIDs.Count;

            if (childrenCount == 1)
            {
                //normalConversationText.text = conversationNode.Conversations[0];
                GameManager.InGameUI.SetTextSequentially(normalConversationText, conversationNode.Conversations[0].ToCharArray());

                normalConversationText.gameObject.SetActive(true);
                choiceConversationTextButtonsGameObject.SetActive(false);

                for (int i = 0; i < MAX_CHOICE_BUTTON_COUNT; ++i)
                {
                    choiceButtonList[i].gameObject.SetActive(false);
                }

                if (OnNornalConversationDelegate != null) 
                {
                    OnNornalConversationDelegate.Invoke(conversationData.ConversationDataName, conversationNode.MyID);
                }
                
            }
            else if (childrenCount > 1 && childrenCount <= MAX_CHOICE_BUTTON_COUNT) 
            {
                
                for (int i = 0; i < childrenCount; ++i) 
                {
                    choiceButtonList[i].gameObject.SetActive(true);
                    choiceButtonTextList[i].text = conversationNode.Conversations[i];
                }

                normalConversationText.gameObject.SetActive(false);
                choiceConversationTextButtonsGameObject.SetActive(true);

                if (OnChoiceConversationDelegate != null) 
                {
                    OnChoiceConversationDelegate.Invoke(conversationData.ConversationDataName, conversationNode.MyID);
                }
            }
            else
            {
                Debug.LogAssertion("Wierd Children Count..");
                return;
            }
        }

        private void Open()
        {
            background.SetActive(true);
        }

        private void Close()
        {
            background.SetActive(false);
        }

        private void FinishConversation()
        {
            Close();
            isConversationStarted = false;
            //GameManager.TimerManager.TryPuaseGameWorld();
            //Cursor.visible = false;
            //Cursor.lockState = CursorLockMode.Locked;
            GameManager.TryUseOrNotUIInteractionState();

            if (OnConversationFinishedDelegate != null) 
            {
                Dictionary<int, ConversationNode> conversationTable = conversationData.ConversationTable;
                ConversationNode conversationNode = conversationTable[currentConversationId];

                OnConversationFinishedDelegate.Invoke(conversationData.ConversationDataName, conversationNode.MyID);
            }
        }

    }
}


