using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

namespace CommonRPG
{
    [DefaultExecutionOrder(-9)]
    public class QuestManager : MonoBehaviour
    {
        [SerializeField]
        private QuestDataScriptableObject questData;

        [SerializeField]
        private QuestWindow questWindow;

        private Dictionary<string, QuestInfo> allQuestTable = new Dictionary<string, QuestInfo>();

        /// <summary>
        /// use this like -> List[(int)EQuestState][(int)EQuestType], and dictionary key is QuestName
        /// </summary>
        private List<List<Dictionary<string, QuestInfo>>> classifiedQuestTableList = new List<List<Dictionary<string, QuestInfo>>>();

        private List<string> tableKeyRemoverList = new();

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
            GameManager.TimerManager.PauseGameWorld(shouldOpen);
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
                Debug.LogWarning("This is not Locked Quest");
                return;
            }

            questInfo.QuestState = EQuestState.Unlocked;

            if (classifiedQuestTableList[(int)EQuestState.Unlocked][(int)questInfo.QuestType].TryAdd(questName, questInfo) == false)
            {
                Debug.LogAssertion("Quest Receive Failed..");
                return;
            }

            classifiedQuestTableList[(int)EQuestState.Locked][(int)questInfo.QuestType].Remove(questName);

            questWindow.QuestNameView.AddQuest(questName, questInfo.QuestDescription, EQuestState.Unlocked);
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

            questWindow.QuestNameView.RemoveQuest(questName, EQuestState.Unlocked);
            questWindow.QuestNameView.AddQuest(questName, questInfo.QuestDescription, EQuestState.Ongoing);

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

            questWindow.QuestNameView.RemoveQuest(questName, EQuestState.Pending);
            questWindow.QuestNameView.AddQuest(questName, questInfo.QuestDescription, EQuestState.Completed);
            
            //TODO: Receive rewards

            return true;
        }

        public void PrintQuests()
        {
            foreach (QuestInfo questInfo in allQuestTable.Values)
            {
                
                if (questInfo.QuestType == EQuestType.Hunt && questInfo.QuestState == EQuestState.Ongoing || questInfo.QuestState == EQuestState.Pending || questInfo.QuestState == EQuestState.Completed) 
                {
                    Debug.Log($"Quest Name : {questInfo.QuestName}, Quest Type : {questInfo.QuestType}, Quest State : {questInfo.QuestState}");
                    foreach (var KeyAndValue in questInfo.OngoingHuntTable) 
                    {
                        Debug.Log($"Current Hunt {KeyAndValue.Key} : {KeyAndValue.Value}");
                    }
                }
            }
        }

        public List<Dictionary<string, QuestInfo>> GetQuestsByState(EQuestState questState)
        {
            if (classifiedQuestTableList.Count <= (int)questState) 
            {
                return null;
            }

            return classifiedQuestTableList[(int)questState];
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
