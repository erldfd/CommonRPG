using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public class CraftingStation : NPC
    {
        private const string CRAFT_STATION_CONVERSATION_DATA_NAME = "CraftingStationConversation";
        private const int CRAFT_ALLOW_CONVERSATION_NODE_ID = -1801746829;
        private const int CRAFT_ALLOW_CONVERSATION_CHOICE_BUTTON_INDEX = 0;

        private const int CRAFT_STATION_CONVERSATION_END_NODE_ID = 1101054152;

        private bool shouldOpenCraftingWindow = false;

        protected override void OnEnable()
        {
            base.OnEnable();

            GameManager.InGameUI.BindEventToOnChoiceConversationButtonClickedDelegate(OnChoiceConversationButtonClicked);
            GameManager.InGameUI.BindEventToOnConversationFinishedDelegate(OnConversationFinished);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            GameManager.InGameUI.RemoveEventToOnChoiceConversationButtonClickedDelegate(OnChoiceConversationButtonClicked);
            GameManager.InGameUI.RemoveEventToOnConversationFinishedDelegate(OnConversationFinished);
        }

        public override void InteractWithPlayer()
        {
            base.InteractWithPlayer();

            if (CurrentConversationData)
            {
                GameManager.InGameUI.ReadyToConversate(CurrentConversationData);
                GameManager.SetActiveInteractioUI(false);
                return;
            }

            GameManager.InventoryManager.OpenAndCloseCraftInventory(true);
            GameManager.SetActiveInteractioUI(false);
        }

        private void OnChoiceConversationButtonClicked(string conversationDataName, int nodeId, int clickedButtonIndex)
        {
            if (conversationDataName == CRAFT_STATION_CONVERSATION_DATA_NAME && nodeId == CRAFT_ALLOW_CONVERSATION_NODE_ID && clickedButtonIndex == CRAFT_ALLOW_CONVERSATION_CHOICE_BUTTON_INDEX) 
            {
                shouldOpenCraftingWindow = true;
            }
        }

        private void OnConversationFinished(string conversationName, int nodeId)
        {
            if (conversationName == CRAFT_STATION_CONVERSATION_DATA_NAME && nodeId == CRAFT_STATION_CONVERSATION_END_NODE_ID && shouldOpenCraftingWindow) 
            {
                GameManager.InventoryManager.OpenAndCloseCraftInventory(true);
                shouldOpenCraftingWindow = false;
            }
        }
    }
}
