using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

namespace CommonRPG
{
    public class QuestManager : MonoBehaviour
    {
        [SerializeField]
        private QuestDataScriptableObject questData;

        private Dictionary<string, QuestInfo> allQuestTable = new Dictionary<string, QuestInfo>();

        /// <summary>
        /// use this like -> List[(int)EQuestState][(int)EQuestType]
        /// </summary>
        private List<List<Dictionary<string, QuestInfo>>> classifiedQuestTableList = new List<List<Dictionary<string, QuestInfo>>>();

        private List<string> tableKeyRemoverList = new();
        //private Dictionary<string, QuestInfo> unlockedHuntQuestTable = new Dictionary<string, QuestInfo>();
        //private Dictionary<string, QuestInfo> unlockedDestinationQuestTable = new Dictionary<string, QuestInfo>();
        //private Dictionary<string, QuestInfo> unlockedInteractionQuestTable = new Dictionary<string, QuestInfo>();
        //private Dictionary<string, QuestInfo> unlockedItemCollectionQuestTable = new Dictionary<string, QuestInfo>();

        //private Dictionary<string, QuestInfo> lockedHuntQuestTable = new Dictionary<string, QuestInfo>();
        //private Dictionary<string, QuestInfo> lockedDestinationQuestTable = new Dictionary<string, QuestInfo>();
        //private Dictionary<string, QuestInfo> lockedInteractionQuestTable = new Dictionary<string, QuestInfo>();
        //private Dictionary<string, QuestInfo> lockedItemCollectionQuestTable = new Dictionary<string, QuestInfo>();

        //private Dictionary<string, QuestInfo> ongoingHuntQuestTable = new Dictionary<string, QuestInfo>();
        //private Dictionary<string, QuestInfo> ongoingDestinationQuestTable = new Dictionary<string, QuestInfo>();
        //private Dictionary<string, QuestInfo> ongoingInteractionQuestTable = new Dictionary<string, QuestInfo>();
        //private Dictionary<string, QuestInfo> ongoingItemCollectionQuestTable = new Dictionary<string, QuestInfo>();

        //private Dictionary<string, QuestInfo> pendingHuntQuestTable = new Dictionary<string, QuestInfo>();
        //private Dictionary<string, QuestInfo> pendingDestinationQuestTable = new Dictionary<string, QuestInfo>();
        //private Dictionary<string, QuestInfo> pendingInteractionQuestTable = new Dictionary<string, QuestInfo>();
        //private Dictionary<string, QuestInfo> pendingItemCollectionQuestTable = new Dictionary<string, QuestInfo>();

        //private Dictionary<string, QuestInfo> completedHuntQuestTable = new Dictionary<string, QuestInfo>();
        //private Dictionary<string, QuestInfo> completedDestinationQuestTable = new Dictionary<string, QuestInfo>();
        //private Dictionary<string, QuestInfo> completedInteractionQuestTable = new Dictionary<string, QuestInfo>();
        //private Dictionary<string, QuestInfo> completedItemCollectionQuestTable = new Dictionary<string, QuestInfo>();

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

        public void UnlockQuest(string questName)
        {
            QuestInfo questInfo;
            bool isSucceeded = allQuestTable.TryGetValue(questName, out questInfo);

            if (isSucceeded == false)
            {
                Debug.LogAssertion("Not Available Quest Key");
                return;
            }

            if (questInfo.QuestState == EQuestState.None)
            {
                Debug.LogAssertion("Weird Quest State");
                return;
            }

            if (questInfo.QuestState != EQuestState.Locked)
            {
                Debug.LogAssertion("This is not Locked Quest");
                return;
            }

            questInfo.QuestState = EQuestState.Unlocked;

            if (classifiedQuestTableList[(int)EQuestState.Unlocked][(int)questInfo.QuestType].TryAdd(questName, questInfo) == false)
            {
                Debug.LogAssertion("Quest Receive Failed..");
                return;
            }

            classifiedQuestTableList[(int)EQuestState.Locked][(int)questInfo.QuestType].Remove(questName);

            //TODO: UI Change.......
        }

        public bool TryReceiveQuest(string questName)
        {
            QuestInfo questInfo;
            bool isSucceeded = allQuestTable.TryGetValue(questName, out questInfo);

            if (isSucceeded == false)
            {
                Debug.LogAssertion("Not Available Quest Key");
                return false;
            }

            if (questInfo.QuestState == EQuestState.None)
            {
                Debug.LogAssertion("Weird Quest State");
                return false;
            }

            if (questInfo.QuestState != EQuestState.Unlocked)
            {
                Debug.LogAssertion("This is not Unlocked Quest");
                return false;
            }

            questInfo.QuestState = EQuestState.Ongoing;

            if (classifiedQuestTableList[(int)EQuestState.Ongoing][(int)questInfo.QuestType].TryAdd(questName, questInfo) == false)
            {
                Debug.LogAssertion("Quest Receive Failed..");
                return false;
            }

            classifiedQuestTableList[(int)EQuestState.Unlocked][(int)questInfo.QuestType].Remove(questName);

            //TODO: UI Change.......

            return true;
        }

        public bool TryCompleteQuest(string questName)
        {
            QuestInfo questInfo;
            bool isSucceeded = allQuestTable.TryGetValue(questName, out questInfo);

            if (isSucceeded == false)
            {
                Debug.LogAssertion("Not Available Quest Key");
                return false;
            }

            if (questInfo.QuestState == EQuestState.None)
            {
                Debug.LogAssertion("Weird Quest State");
                return false;
            }

            if (questInfo.QuestState != EQuestState.Pending)
            {
                Debug.LogAssertion("This is not Pending Quest");
                return false;
            }

            questInfo.QuestState = EQuestState.Completed;

            if (classifiedQuestTableList[(int)EQuestState.Completed][(int)questInfo.QuestType].TryAdd(questName, questInfo) == false)
            {
                Debug.LogAssertion("Quest Receive Failed..");
                return false;
            }

            classifiedQuestTableList[(int)EQuestState.Pending][(int)questInfo.QuestType].Remove(questName);

            //TODO: UI Change & Receive rewards

            return true;
        }

        public void PrintQuests()
        {
            foreach (QuestInfo questInfo in allQuestTable.Values)
            {
                Debug.Log($"Quest Name : {questInfo.QuestName}, Quest Type : {questInfo.QuestType}, Quest State : {questInfo.QuestState}");
                if (questInfo.QuestType == EQuestType.Hunt) 
                {

                    foreach (var KeyAndValue in questInfo.OngoingHuntTable) 
                    {
                        Debug.Log($"Current Hunt {KeyAndValue.Key} : {KeyAndValue.Value}");
                    }
                }
            }
        }

        private void ArrangeQuestTable()
        {
            List<QuestInfo> questInfos = questData.GetData();

            foreach (QuestInfo questInfo in questInfos)
            {
                if (questInfo.QuestState == EQuestState.None) 
                {
                    Debug.LogAssertion("Weird Quest State");
                    return;
                }

                if (allQuestTable.TryAdd(questInfo.QuestName, questInfo) == false) 
                {
                    Debug.LogAssertion($"this Quest Name is already exist : {questInfo.QuestName}");
                    return;
                }

                if (classifiedQuestTableList[(int)questInfo.QuestState][(int)questInfo.QuestType].TryAdd(questInfo.QuestName, questInfo) == false)
                {
                    Debug.LogAssertion($"this Quest Name is already exist : {questInfo.QuestName}");
                    return;
                }
            }
        }

        private void OnKilled(MonsterBase monster, AUnit Killer)
        {
            Debug.Log($"{monster} is killed by {Killer}");

            Dictionary<string, QuestInfo> ongoingHuntQuestTable = classifiedQuestTableList[(int)EQuestState.Ongoing][(int)EQuestType.Hunt];
            tableKeyRemoverList.Clear();

            foreach (KeyValuePair<string, QuestInfo> questKeyValuePair in ongoingHuntQuestTable) 
            {
                QuestInfo questInfo = questKeyValuePair.Value;

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

                    Debug.Log($"Hunt Quest Clear : {questKeyValuePair.Value.QuestName}");

                    if (classifiedQuestTableList[(int)EQuestState.Pending][(int)EQuestType.Hunt].TryAdd(questKeyValuePair.Key, questInfo) == false) 
                    {
                        Debug.LogAssertion("Add Failed to PendingTable");
                        return;
                    }
                }
            }

            foreach (string key in tableKeyRemoverList) 
            {
                ongoingHuntQuestTable.Remove(key);
            }
        }
    }
}
