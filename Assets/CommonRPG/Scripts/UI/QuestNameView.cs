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
            LinkedList<ListViewEntry> entryDataList = unlockedQuestNameView.Entries;

            BindEntryClickedDelegate(entryDataList);

            entryDataList = ongoingQuestNameView.Entries;

            BindEntryClickedDelegate(entryDataList);

            entryDataList = completedQuestNameView.Entries;

            BindEntryClickedDelegate(entryDataList);

            void BindEntryClickedDelegate(LinkedList<ListViewEntry> newEntryDataList)
            {
                foreach (ListViewEntry entryList in newEntryDataList)
                {
                    QuestNameEntry questNameEntry = entryList as QuestNameEntry;
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
            LinkedList<ListViewEntry> entryDataList = unlockedQuestNameView.Entries;

            RemoveBindingEntryClickedDelegate(entryDataList);

            entryDataList = ongoingQuestNameView.Entries;

            RemoveBindingEntryClickedDelegate(entryDataList);

            entryDataList = completedQuestNameView.Entries;

            RemoveBindingEntryClickedDelegate(entryDataList);

            void RemoveBindingEntryClickedDelegate(LinkedList<ListViewEntry> newEntryDataList)
            {
                foreach (ListViewEntry entryList in newEntryDataList)
                {
                    QuestNameEntry questNameEntry = entryList as QuestNameEntry;
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

        public void AddQuestName(QuestNameItem quest, EQuestState questState)
        {
            switch(questState)
            {
                case EQuestState.None:
                {
                    Debug.LogAssertion("Weird Quest State");
                    break;
                }
                case EQuestState.Unlocked:
                {
                    unlockedQuestNameView.AddItem(quest);
                    break;
                }
                case EQuestState.Locked:
                {
                    Debug.LogAssertion("Locked Quest can not be added");
                    break;
                }
                case EQuestState.Ongoing:
                {
                    ongoingQuestNameView.AddItem(quest);
                    break;
                }
                case EQuestState.Pending:
                {
                    ongoingQuestNameView.AddItem(quest);
                    break;
                }
                case EQuestState.Completed:
                {
                    completedQuestNameView.AddItem(quest);
                    break;
                }
                default:
                {
                    Debug.LogAssertion("Weird Quest State");
                    break;
                }
            }
        }

        public void RemoveQuestName()
        {

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
