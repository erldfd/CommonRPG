using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

namespace CommonRPG
{
    public class QuestNameEntry : AListViewEntry, IPointerDownHandler 
    {
        /// <summary>
        /// args : string questNameText, string questDescription
        /// </summary>
        public event Action<string, string> OnEntryClickedDelegate = null;

        [SerializeField]
        private TextMeshProUGUI questNameText;

        [SerializeField]
        private Image questNameEntryImage;

        private string questDescription;

        private bool isPending;

        private Color notPendingColor = new Color(255, 255, 255, 255);
        private Color pendingColor = new Color(255, 208, 0, 255);

        //private void OnEnable()
        //{
        //    GameManager.QuestManager.OnPendingQuestDelegate += OnPendingQuest;
        //}

        //private void OnDisable()
        //{
        //    GameManager.QuestManager.OnPendingQuestDelegate -= OnPendingQuest;
        //}

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
            isPending = questNameItem.IsPending;

            SetQuestNameEntryColor(isPending);

            GameManager.QuestManager.OnPendingQuestDelegate -= OnPendingQuest;
            GameManager.QuestManager.OnPendingQuestDelegate += OnPendingQuest;
        }

        public void SetQuestNameText(string newText)
        {
            questNameText.text = newText;
        }

        public bool IsOnEntryClickedDelegateBound()
        {
            return OnEntryClickedDelegate != null;
        }

        /// <summary>
        /// if quest state is pending or not , quest name entry color changes to pending color or not pending color
        /// </summary>
        /// <param name="isPending"></param>
        private void SetQuestNameEntryColor(bool isPending)
        {
            if(isPending)
            {
                questNameEntryImage.color = pendingColor;
            }
            else
            {
                questNameEntryImage.color = notPendingColor;
            }
        }

        private void OnPendingQuest(int questId)
        {
            int currentQuestId = GameManager.QuestManager.QuestNameIdTable[questNameText.text];

            if (currentQuestId == questId) 
            {
                SetQuestNameEntryColor(true);
            }
        }
    }
}
