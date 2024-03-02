using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

namespace CommonRPG
{
    [DefaultExecutionOrder(-9)]
    public class QuestManager : MonoBehaviour
    {
        /// <summary>
        /// arg : int Quest id
        /// </summary>
        public event Action<int> OnCompleteQuestDelegate;

        /// <summary>
        /// arg : int Quest id
        /// </summary>
        public event Action<int> OnPendingQuestDelegate;

        [SerializeField]
        private QuestDataScriptableObject questData;

        private List<Quest> questList = new List<Quest>();

        [SerializeField]
        private QuestWindow questWindow;

        /// <summary>
        /// int -> quest id
        /// </summary>
        private Dictionary<int, Quest> allQuestTable = new Dictionary<int, Quest>();

        /// <summary>
        /// use this like -> List[(int)EQuestState][(int)EQuestType], and dictionary key is QuestName
        /// </summary>
        private List<List<Dictionary<int, Quest>>> classifiedQuestTableList = new List<List<Dictionary<int, Quest>>>();
        
        private List<int> tableKeyRemoverList = new List<int>();

        /// <summary>
        /// string : quest name, int : quest id
        /// </summary>
        private Dictionary<string, int> questNameIdTable = new Dictionary<string, int>();
        public Dictionary<string, int> QuestNameIdTable { get { return questNameIdTable; } }

        private void Awake()
        {
            Debug.Assert(questData);

            int eQuestStateLength = System.Enum.GetValues(typeof(EQuestState)).Length;
            int eQuestTypeLength = System.Enum.GetValues(typeof(EQuestType)).Length;

            for (int i = 0; i < eQuestStateLength; ++i) 
            {
                classifiedQuestTableList.Add(new());

                for (int j = 0; j < eQuestTypeLength; ++j) 
                {
                    classifiedQuestTableList[i].Add(new());
                }
            }

            ArrangeQuestTable();
        }

        private void OnEnable()
        {
            MonsterBase.OnKilled += OnKilled;
        }

        private void OnDisable()
        {
            MonsterBase.OnKilled -= OnKilled;
        }

        public void OpenAndCloseQuestWindow()
        {
            bool shouldOpen = questWindow.gameObject.activeSelf == false;
            questWindow.gameObject.SetActive(shouldOpen);
            GameManager.TryUseOrNotUIInteractionState();
        }

        public bool IsQuestWindowOpened()
        {
            return (questWindow.gameObject.activeSelf);
        }

        public void UnlockQuest(int questId)
        {
            Quest quest;
            bool isSucceeded = allQuestTable.TryGetValue(questId, out quest);

            if (isSucceeded == false)
            {
                Debug.LogAssertion("Not Available Quest Key");
                return;
            }

            if (quest.QuestState == EQuestState.None)
            {
                Debug.LogAssertion("Weird Quest State");
                return;
            }

            if (quest.QuestState != EQuestState.Locked)
            {
                Debug.LogWarning("This is not Locked Quest");
                return;
            }

            quest.QuestState = EQuestState.Unlocked;

            if (classifiedQuestTableList[(int)EQuestState.Unlocked][(int)quest.QuestInfo.QuestType].TryAdd(questId, quest) == false)
            {
                Debug.LogAssertion("Quest Receive Failed..");
                return;
            }

            classifiedQuestTableList[(int)EQuestState.Locked][(int)quest.QuestInfo.QuestType].Remove(questId);

            questWindow.QuestNameView.AddQuest(quest.QuestInfo.QuestName, quest.QuestInfo.QuestDescription, EQuestState.Unlocked);
        }

        public bool TryReceiveQuest(int questId)
        {
            Quest quest;
            bool isSucceeded = allQuestTable.TryGetValue(questId, out quest);

            if (isSucceeded == false)
            {
                Debug.LogAssertion("Not Available Quest Key");
                return false;
            }

            if (quest.QuestState == EQuestState.None)
            {
                Debug.LogAssertion("Weird Quest State");
                return false;
            }

            if (quest.QuestState != EQuestState.Unlocked)
            {
                Debug.LogAssertion("This is not Unlocked Quest");
                return false;
            }

            quest.QuestState = EQuestState.Ongoing;

            if (classifiedQuestTableList[(int)EQuestState.Ongoing][(int)quest.QuestInfo.QuestType].TryAdd(questId, quest) == false)
            {
                Debug.LogAssertion("Quest Receive Failed..");
                return false;
            }

            classifiedQuestTableList[(int)EQuestState.Unlocked][(int)quest.QuestInfo.QuestType].Remove(questId);

            questWindow.QuestNameView.RemoveQuest(quest.QuestInfo.QuestName, EQuestState.Unlocked);
            questWindow.QuestNameView.AddQuest(quest.QuestInfo.QuestName, quest.QuestInfo.QuestDescription, EQuestState.Ongoing);

            return true;
        }

        public bool TryCompleteQuest(int questId)
        {
            Quest quest;
            bool isSucceeded = allQuestTable.TryGetValue(questId, out quest);

            if (isSucceeded == false)
            {
                Debug.LogAssertion("Not Available Quest Key");
                return false;
            }

            if (quest.QuestState == EQuestState.None)
            {
                Debug.LogAssertion("Weird Quest State");
                return false;
            }

            if (quest.QuestState != EQuestState.Pending)
            {
                Debug.LogAssertion("This is not Pending Quest");
                return false;
            }

            quest.QuestState = EQuestState.Completed;

            if (classifiedQuestTableList[(int)EQuestState.Completed][(int)quest.QuestInfo.QuestType].TryAdd(questId, quest) == false)
            {
                Debug.LogAssertion("Quest Receive Failed..");
                return false;
            }

            classifiedQuestTableList[(int)EQuestState.Pending][(int)quest.QuestInfo.QuestType].Remove(questId);

            questWindow.QuestNameView.RemoveQuest(quest.QuestInfo.QuestName, EQuestState.Pending);
            questWindow.QuestNameView.AddQuest(quest.QuestInfo.QuestName, quest.QuestInfo.QuestDescription, EQuestState.Completed);

            if (OnCompleteQuestDelegate != null)
            {
                OnCompleteQuestDelegate.Invoke(questId);
            }

            return true;
        }

        public void PrintQuests()
        {
            foreach (Quest quest in allQuestTable.Values)
            {
                QuestInfo questInfo = quest.QuestInfo;

                if (questInfo.QuestType == EQuestType.Hunt && quest.QuestState == EQuestState.Ongoing || quest.QuestState == EQuestState.Pending || quest.QuestState == EQuestState.Completed)
                {
                    Debug.Log($"Quest Name : {questInfo.QuestName}, Quest Type : {questInfo.QuestType}, Quest State : {quest.QuestState}");
                    foreach (var KeyAndValue in questInfo.OngoingHuntTable)
                    {
                        Debug.Log($"Current Hunt {KeyAndValue.Key} : {KeyAndValue.Value}");
                    }
                }
                else
                {
                    Debug.Log($"Quest Name : {questInfo.QuestName}, Quest Type : {questInfo.QuestType}, Quest State : {quest.QuestState}");
                }
            }
        }

        public List<Dictionary<int, Quest>> GetQuestsByState(EQuestState questState)
        {
            if (classifiedQuestTableList.Count <= (int)questState)
            {
                return null;
            }

            return classifiedQuestTableList[(int)questState];
        }

        public EQuestState GetQuestStateFromQuestName(string questName)
        {
            return GetQuestStateFromQuestId(QuestNameIdTable[questName]);
        }

        public EQuestState GetQuestStateFromQuestId(int questId)
        {
            if (allQuestTable.TryGetValue(questId, out Quest quest) == false)
            {
                return EQuestState.None;
            }

            return quest.QuestState;
        }

        private void ArrangeQuestTable()
        {
            List<QuestInfo> questInfos = questData.GetData();
            
            questList.Clear();

            foreach (QuestInfo questInfo in questInfos)
            {
                questList.Add(new Quest(questInfo));
            }

            foreach (Quest quest in questList)
            {
                QuestInfo questInfo = quest.QuestInfo;

                if (quest.QuestState == EQuestState.None)
                {
                    Debug.LogAssertion("Weird Quest State");
                    return;
                }

                if (allQuestTable.TryAdd(questInfo.QuestId, quest) == false)
                {
                    Debug.LogAssertion($"this Quest Name is already exist : {questInfo.QuestName}");
                    return;
                }

                if (classifiedQuestTableList[(int)quest.QuestState][(int)questInfo.QuestType].TryAdd(quest.QuestInfo.QuestId, quest) == false)
                {
                    Debug.LogAssertion($"this Quest Name is already exist : {questInfo.QuestName}");
                    return;
                }

                if (questNameIdTable.TryAdd(questInfo.QuestName, questInfo.QuestId) == false) 
                {
                    Debug.LogAssertion("quest Add Failed");
                    return;
                }
            }
        }

        private void OnKilled(MonsterBase monster, AUnit Killer)
        {
            Debug.Log($"{monster} is killed by {Killer}");

            Dictionary<int, Quest> ongoingHuntQuestTable = classifiedQuestTableList[(int)EQuestState.Ongoing][(int)EQuestType.Hunt];
            tableKeyRemoverList.Clear();

            foreach (KeyValuePair<int, Quest> questKeyValuePair in ongoingHuntQuestTable)
            {
                QuestInfo questInfo = questKeyValuePair.Value.QuestInfo;

                if (questInfo.OngoingHuntTable.ContainsKey(monster.MonsterName) == false)
                {
                    continue;
                }

                questInfo.OngoingHuntTable[monster.MonsterName]++;

                if (questInfo.OngoingHuntTable[monster.MonsterName] >= questInfo.HuntQuestConditionTable[monster.MonsterName])
                {
                    // pend hunt quest
                    //ongoingHuntQuestTable.Remove(questKeyValuePair.Key);
                    tableKeyRemoverList.Add(questKeyValuePair.Key);
                    questKeyValuePair.Value.QuestState = EQuestState.Pending;

                    Debug.Log($"Hunt Quest Clear : {questKeyValuePair.Value.QuestInfo.QuestName}");

                    if (classifiedQuestTableList[(int)EQuestState.Pending][(int)EQuestType.Hunt].TryAdd(questKeyValuePair.Key, questKeyValuePair.Value))
                    {
                        if (OnPendingQuestDelegate != null) 
                        {
                            OnPendingQuestDelegate.Invoke(questInfo.QuestId);
                        }
                        
                    }
                    else
                    {
                        Debug.LogAssertion("Add Failed to PendingTable");
                        return;
                    }
                }
            }

            foreach (int key in tableKeyRemoverList)
            {
                ongoingHuntQuestTable.Remove(key);
            }
        }
    }
}
