using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace CommonRPG
{
    [DefaultExecutionOrder(-8)]
    public class QuestNameView : MonoBehaviour
    {
        /// <summary>
        /// args : string questName, string questDescription
        /// </summary>
        public event Action<string, string> OnQuestNameEntryClickedDelegate = null;
        [SerializeField]
        private ListView unlockedQuestNameView;

        [SerializeField]
        private ListView ongoingQuestNameView;

        [SerializeField]
        private ListView completedQuestNameView;

        private void Awake()
        {
            Debug.Assert(unlockedQuestNameView);
            Debug.Assert(ongoingQuestNameView);
            Debug.Assert(completedQuestNameView);

            List<Dictionary<string, QuestInfo>> questTableList = GameManager.QuestManager.GetQuestsByState(EQuestState.Unlocked);

            if (questTableList != null) 
            {
                foreach (Dictionary<string, QuestInfo> questTable in questTableList)
                {
                    foreach (var quest in questTable)
                    {
                        unlockedQuestNameView.AddItem(new QuestNameItem(quest.Key, quest.Value.QuestDescriotion));
                    }
                }
            }

            //unlockedQuestNameView.Init();

            questTableList = GameManager.QuestManager.GetQuestsByState(EQuestState.Ongoing);

            if (questTableList != null) 
            {
                foreach (Dictionary<string, QuestInfo> questTable in questTableList)
                {
                    foreach (var quest in questTable)
                    {
                        ongoingQuestNameView.AddItem(new QuestNameItem(quest.Key, quest.Value.QuestDescriotion));
                    }
                }
            }

            //ongoingQuestNameView.Init();

            questTableList = GameManager.QuestManager.GetQuestsByState(EQuestState.Completed);

            if (questTableList != null)
            {
                foreach (Dictionary<string, QuestInfo> questTable in questTableList)
                {
                    foreach (var quest in questTable)
                    {
                        completedQuestNameView.AddItem(new QuestNameItem(quest.Key, quest.Value.QuestDescriotion));
                    }
                }
            }

           // completedQuestNameView.Init();

            ShowUnlockedQuestWindow();
        }

        private void OnEnable()
        {

            LinkedList<ListViewEntryData> entryDataList = unlockedQuestNameView.EntryData;

            BindEntryClickedDelegate(entryDataList);

            entryDataList = ongoingQuestNameView.EntryData;

            BindEntryClickedDelegate(entryDataList);

            entryDataList = completedQuestNameView.EntryData;

            BindEntryClickedDelegate(entryDataList);

            void BindEntryClickedDelegate(LinkedList<ListViewEntryData> newEntryDataList)
            {
                foreach (ListViewEntryData entryList in newEntryDataList)
                {
                    QuestNameEntry questNameEntry = entryList.Entry as QuestNameEntry;
                    if (questNameEntry == null)
                    {
                        continue;
                    }

                    questNameEntry.OnEntryClickedDelegate += OnQuestNameEntryClicked;
                }
            }
        }

        private void OnDisable()
        {
            LinkedList<ListViewEntryData> entryDataList = unlockedQuestNameView.EntryData;

            RemoveBindingEntryClickedDelegate(entryDataList);

            entryDataList = ongoingQuestNameView.EntryData;

            RemoveBindingEntryClickedDelegate(entryDataList);

            entryDataList = completedQuestNameView.EntryData;

            RemoveBindingEntryClickedDelegate(entryDataList);

            void RemoveBindingEntryClickedDelegate(LinkedList<ListViewEntryData> newEntryDataList)
            {
                foreach (ListViewEntryData entryList in newEntryDataList)
                {
                    QuestNameEntry questNameEntry = entryList.Entry as QuestNameEntry;
                    if (questNameEntry == null)
                    {
                        continue;
                    }

                    questNameEntry.OnEntryClickedDelegate -= OnQuestNameEntryClicked;
                }
            }
        }

        public void ShowUnlockedQuestWindow()
        {
            unlockedQuestNameView.gameObject.SetActive(true);
            ongoingQuestNameView.gameObject.SetActive(false);
            completedQuestNameView.gameObject.SetActive(false);
        }

        public void ShowOngoingQuestWindow()
        {
            unlockedQuestNameView.gameObject.SetActive(false);
            ongoingQuestNameView.gameObject.SetActive(true);
            completedQuestNameView.gameObject.SetActive(false);
        }

        public void ShowCompletedQuestWindow()
        {
            unlockedQuestNameView.gameObject.SetActive(false);
            ongoingQuestNameView.gameObject.SetActive(false);
            completedQuestNameView.gameObject.SetActive(true);
        }


        private void OnQuestNameEntryClicked(string questName, string questDescription)
        {
            OnQuestNameEntryClickedDelegate.Invoke(questName, questDescription);
        }
    }

    public class QuestNameItem : ListViewItem
    {
        public string QuestName { get; set; }
        public string QuestDescription { get; set; }

        public int ItemIndex { get; set; }

        public QuestNameItem(string questName, string questDescription)
        {
            QuestName = questName;
            QuestDescription = questDescription;
        }
    }
}
