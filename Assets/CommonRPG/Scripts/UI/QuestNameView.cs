using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.EventSystems;

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

        private static Dictionary<string, QuestNameItem> includedQuest = new();

        private void Awake()
        {
            Debug.Assert(unlockedQuestNameView);
            Debug.Assert(ongoingQuestNameView);
            Debug.Assert(completedQuestNameView);

            //List<Dictionary<string, QuestInfo>> questTableList = GameManager.QuestManager.GetQuestsByState(EQuestState.Unlocked);

            //if (questTableList != null)
            //{
            //    foreach (Dictionary<string, QuestInfo> questTable in questTableList)
            //    {
            //        foreach (var quest in questTable)
            //        {
            //            AddQuest(quest.Key, quest.Value.QuestDescription, EQuestState.Unlocked);
            //        }
            //    }
            //}

            //questTableList = GameManager.QuestManager.GetQuestsByState(EQuestState.Ongoing);

            //if (questTableList != null)
            //{
            //    foreach (Dictionary<string, QuestInfo> questTable in questTableList)
            //    {
            //        foreach (var quest in questTable)
            //        {
            //            ongoingQuestNameView.AddItem(new QuestNameItem(quest.Key, quest.Value.QuestDescription));
            //            AddQuest(quest.Key, quest.Value.QuestDescription, EQuestState.Ongoing);
            //        }
            //    }
            //}

            //questTableList = GameManager.QuestManager.GetQuestsByState(EQuestState.Completed);

            //if (questTableList != null)
            //{
            //    foreach (Dictionary<string, QuestInfo> questTable in questTableList)
            //    {
            //        foreach (var quest in questTable)
            //        {
            //            AddQuest(quest.Key, quest.Value.QuestDescription, EQuestState.Ongoing);
            //        }
            //    }
            //}

            ShowUnlockedQuestWindow();
        }

        private void OnEnable()
        {
            LinkedList<AListViewEntry> entryDataList = unlockedQuestNameView.Entries;

            BindEntryClickedDelegate(entryDataList);

            entryDataList = ongoingQuestNameView.Entries;

            BindEntryClickedDelegate(entryDataList);

            entryDataList = completedQuestNameView.Entries;

            BindEntryClickedDelegate(entryDataList);

            void BindEntryClickedDelegate(LinkedList<AListViewEntry> newEntryDataList)
            {
                foreach (AListViewEntry entryList in newEntryDataList)
                {
                    QuestNameEntry questNameEntry = entryList as QuestNameEntry;
                    if (questNameEntry == null)
                    {
                        continue;
                    }

                    if (questNameEntry.IsOnEntryClickedDelegateBound())
                    {
                        continue;
                    }

                    questNameEntry.OnEntryClickedDelegate += OnQuestNameEntryClicked;
                }
            }
        }

        private void OnDisable()
        {
            LinkedList<AListViewEntry> entryDataList = unlockedQuestNameView.Entries;

            RemoveBindingEntryClickedDelegate(entryDataList);

            entryDataList = ongoingQuestNameView.Entries;

            RemoveBindingEntryClickedDelegate(entryDataList);

            entryDataList = completedQuestNameView.Entries;

            RemoveBindingEntryClickedDelegate(entryDataList);

            void RemoveBindingEntryClickedDelegate(LinkedList<AListViewEntry> newEntryDataList)
            {
                foreach (AListViewEntry entryList in newEntryDataList)
                {
                    QuestNameEntry questNameEntry = entryList as QuestNameEntry;
                    if (questNameEntry == null)
                    {
                        continue;
                    }

                    if (questNameEntry.IsOnEntryClickedDelegateBound() == false)
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

        public void AddQuest(string questName, string questDescription, EQuestState questState)
        {
            if (includedQuest.ContainsKey(questName))
            {
                AddQuest(includedQuest[questName], questState);
            }
            else if (questState == EQuestState.Unlocked) 
            {
                QuestNameItem newQuest = new QuestNameItem(questName, questDescription);
                AddQuest(newQuest, questState);
                includedQuest.Add(questName, newQuest);
            }
            else
            {
                Debug.Log("Quest Add Failed.");
            }
        }

        private void AddQuest(QuestNameItem quest, EQuestState questState)
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

                    QuestNameEntry questNameEntry = unlockedQuestNameView.Entries.Last.Value as QuestNameEntry;
                    if (questNameEntry == null)
                    {
                        Debug.LogAssertion("questNameEntry is null");
                        return;
                    }

                    if (questNameEntry.IsOnEntryClickedDelegateBound())
                    {
                        break;
                    }

                    questNameEntry.OnEntryClickedDelegate += OnQuestNameEntryClicked;

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
                    
                    QuestNameEntry questNameEntry = ongoingQuestNameView.Entries.Last.Value as QuestNameEntry;
                    if (questNameEntry == null)
                    {
                        Debug.LogAssertion("questNameEntry is null");
                        return;
                    }

                    if (questNameEntry.IsOnEntryClickedDelegateBound()) 
                    {
                        break;
                    }

                    questNameEntry.OnEntryClickedDelegate += OnQuestNameEntryClicked;

                    break;
                }
                case EQuestState.Pending:
                {
                    ongoingQuestNameView.AddItem(quest);

                    QuestNameEntry questNameEntry = ongoingQuestNameView.Entries.Last.Value as QuestNameEntry;
                    if (questNameEntry == null)
                    {
                        Debug.LogAssertion("questNameEntry is null");
                        return;
                    }

                    if (questNameEntry.IsOnEntryClickedDelegateBound())
                    {
                        break;
                    }

                    questNameEntry.OnEntryClickedDelegate += OnQuestNameEntryClicked;

                    break;
                }
                case EQuestState.Completed:
                {
                    completedQuestNameView.AddItem(quest);

                    QuestNameEntry questNameEntry = completedQuestNameView.Entries.Last.Value as QuestNameEntry;
                    if (questNameEntry == null)
                    {
                        Debug.LogAssertion("questNameEntry is null");
                        return;
                    }

                    if (questNameEntry.IsOnEntryClickedDelegateBound())
                    {
                        break;
                    }

                    questNameEntry.OnEntryClickedDelegate += OnQuestNameEntryClicked;

                    break;
                }
                default:
                {
                    Debug.LogAssertion("Weird Quest State");
                    break;
                }
            }
        }

        public void RemoveQuest(string questName, EQuestState questState)
        {
            if (includedQuest.ContainsKey(questName))
            {
                RemoveQuest(includedQuest[questName], questState);
            }
            else
            {
                Debug.LogAssertion("Weird quest remove detected");
            }
        }

        public void RemoveQuest(QuestNameItem quest, EQuestState questState)
        {
            switch (questState)
            {
                case EQuestState.None:
                {
                    Debug.LogAssertion("Weird Quest State");
                    break;
                }
                case EQuestState.Unlocked:
                {
                    unlockedQuestNameView.RemoveItem(quest);
                    break;
                }
                case EQuestState.Locked:
                {
                    Debug.LogAssertion("Locked Quest can not be removed");
                    break;
                }
                case EQuestState.Ongoing:
                {
                    ongoingQuestNameView.RemoveItem(quest);
                    break;
                }
                case EQuestState.Pending:
                {
                    ongoingQuestNameView.RemoveItem(quest);
                    break;
                }
                case EQuestState.Completed:
                {
                    completedQuestNameView.RemoveItem(quest);
                    break;
                }
                default:
                {
                    Debug.LogAssertion("Weird Quest State");
                    break;
                }
            }
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

        public bool IsPending { get; set; }

        public QuestNameItem(string questName, string questDescription)
        {
            QuestName = questName;
            QuestDescription = questDescription;
            IsPending = false;
        }
    }
}
