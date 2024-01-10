using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CommonRPG
{
    public class QuestNameEntry : ListViewEntry, IPointerDownHandler
    {
        /// <summary>
        /// args : string questNameText, string questDescription
        /// </summary>
        public event Action<string, string> OnEntryClickedDelegate = null;

        [SerializeField]
        private TextMeshProUGUI questNameText;

        private string questDescription;

        private int currentItemIndex;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (OnEntryClickedDelegate == null)
            {
                Debug.Log("OnEntryClickedDelegate is not bound");
                return;
            }

            OnEntryClickedDelegate.Invoke(questNameText.text, questDescription);
        }

        public override void OnUpdateEntry(ListViewItem updatedData)
        {
            QuestNameItem questNameItem = updatedData as QuestNameItem;
            if (questNameItem == null) 
            {
                return;
            }

            SetQuestNameText(questNameItem.QuestName);
            questDescription = questNameItem.QuestDescription;
            currentItemIndex = questNameItem.Index;
        }

        public void SetQuestNameText(string newText)
        {
            questNameText.text = newText;
        }

        public bool IsOnEntryClickedDelegateBound()
        {
            return OnEntryClickedDelegate != null;
        }
    }
}
