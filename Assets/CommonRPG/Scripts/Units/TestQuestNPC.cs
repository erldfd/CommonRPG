using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public class TestQuestNPC : NPC
    {
        private enum EQuestList
        {
            FisrtHuntQuest,
        }

        [SerializeField]
        private int QuestAcceptNodeId = -1;


        protected override void OnEnable()
        {
            base.OnEnable();

            GameManager.InGameUI.BindEventToOnChoiceConversationButtonClickedDelegate(OnChoiceConversationButtonClicked);
            GameManager.InGameUI.BindEventToOnConversationFinishedDelegate(OnConversationFinished);

            GameManager.QuestManager.OnCompleteQuestDelegate += OnCompleteQuest;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            GameManager.InGameUI.RemoveEventToOnChoiceConversationButtonClickedDelegate(OnChoiceConversationButtonClicked);
            GameManager.InGameUI.RemoveEventToOnConversationFinishedDelegate(OnConversationFinished);

            GameManager.QuestManager.OnCompleteQuestDelegate -= OnCompleteQuest;
        }

        public override void InteractWithPlayer()
        {
            base.InteractWithPlayer();

            EQuestState currentQuestState = GameManager.QuestManager.GetQuestStateFromQuestId(currentQuestId);

            int conversationIndex = -1;

            switch (currentQuestState)
            {
                case EQuestState.Unlocked:
                {
                    conversationIndex = 0;
                    break;
                }
                case EQuestState.Ongoing:
                {
                    conversationIndex = 1;
                    break;
                }
                case EQuestState.Pending:
                {
                    conversationIndex = 2;
                    break;
                }
                case EQuestState.Completed:
                {
                    conversationIndex = 3;
                    break;
                }
                default:
                { 
                    break; 
                }
            }

            if (currentConversationData != conversations[conversationIndex]) 
            {
                currentConversationData = conversations[conversationIndex];
            }

            if (CurrentConversationData)
            {
                GameManager.InGameUI.ReadyToConversate(CurrentConversationData);
                GameManager.SetActiveInteractioUI(false);
                return;
            }
        }

        private void OnChoiceConversationButtonClicked(string conversationDataName, int nodeId, int clickedButtonIndex)
        {
            if (conversationDataName == CurrentConversationData.ConversationDataName && nodeId == QuestAcceptNodeId)
            {
                // clicked button index : number of conversation order, first == 0, second == 1..
                if (clickedButtonIndex == 0)
                {
                    GameManager.QuestManager.TryReceiveQuest(questIdList[0]);
                }
            }
        }

        private void OnConversationFinished(string conversationName, int nodeId)
        {
            if (conversationName == CurrentConversationData.ConversationDataName)
            {
                EQuestState currentQuestState = GameManager.QuestManager.GetQuestStateFromQuestId(currentQuestId);

                switch (currentQuestState)
                {
                    case EQuestState.Pending:
                    {
                        GameManager.QuestManager.TryCompleteQuest(currentQuestId);
                        break;
                    }
                    default:
                    {
                        break;
                    }
                }
            }
        }

        private void OnCompleteQuest(int questId)
        {
            // first hunt quest
            if (questId == questIdList[0]) 
            {
                // 10 hp potion
                // 10 exp
                // 10 coins
                GameManager.InventoryManager.Coins += 10;
                GameManager.ObtainItem(EInventoryType.MiscItemInventory, EItemName.HpPotion, 10);
                GameManager.GetPlayerCharacter().ObtainExp(10);
            }
        }
    }
}

