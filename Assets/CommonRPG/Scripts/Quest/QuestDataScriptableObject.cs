using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public enum EQuestType
    {
        None,
        Hunt,
        Destination,
        InteractionWithObject,
        ItemCollection
    }

    public enum EQuestState
    {
        None,
        Unlocked,
        Locked,
        Ongoing,
        Pending, // it means quest is completed but didnt comfirm
        Completed
    }

    [CreateAssetMenu(fileName = "QuestData", menuName = "ScriptableObjects/QuestDataScriptableObject", order = 6)]
    public class QuestDataScriptableObject : ScriptableObject
    {
        [SerializeField]
        private List<QuestInfo> questInfoList;
        public List<QuestInfo> QuestInfoList { get { return questInfoList; } }

        private Dictionary<string, QuestInfo> questTable = new Dictionary<string, QuestInfo>();
        public Dictionary<string, QuestInfo> QuestTable { get { return questTable; } }

        public List<QuestInfo> GetData()
        {
            List<QuestInfo> dataList = new List<QuestInfo>();

            foreach (QuestInfo quest in questInfoList) 
            {
                quest.Arrange();
                dataList.Add(new QuestInfo(quest));
            }

            return dataList;
        }
    }

    [Serializable]
    public class QuestInfo
    {
        [SerializeField]
        private string questName;
        public string QuestName { get { return questName; } }

        [SerializeField]
        private string questDescription;
        public string QuestDescriotion { get { return questDescription; } }

        [SerializeField]
        private EQuestType questType = EQuestType.None;
        public EQuestType QuestType { get { return questType; } }

        [SerializeField]
        private EQuestState questState = EQuestState.Unlocked;
        public EQuestState QuestState { get { return questState; } set { questState = value; } }

        [Header("Hunt Qeust")]
        [SerializeField]
        private List<HuntData> huntQuestCompleteCondition = new List<HuntData>();
        private Dictionary<EMonsterName, int> huntQuestConditionTable = new Dictionary<EMonsterName, int>();
        public Dictionary<EMonsterName, int> HuntQuestConditionTable { get { return huntQuestConditionTable; } }

        private Dictionary<EMonsterName, int> ongoingHuntTable = new Dictionary<EMonsterName, int>();
        public Dictionary<EMonsterName, int> OngoingHuntTable { get { return ongoingHuntTable; } set { ongoingHuntTable = value; } }
        
        [Header("Destination Quest")]
        // 0 index is first destination, 1 index is second destination..... ect..
        [SerializeField]
        private List<Vector3> destinationList;

        [SerializeField]
        private float destinationTolerance = 1;

        //[Header("InteractionWithObject Quest")]

        [Header("ItemCollection Quest")]
        [SerializeField]
        private List<ItemCollectionData> itemCollectionQuestCompleteCondition;

        public QuestInfo()
        {
            foreach (HuntData huntData in huntQuestCompleteCondition) 
            {
                huntQuestConditionTable.TryAdd(huntData.MonsterName, huntData.HuntCount);
                ongoingHuntTable.TryAdd(huntData.MonsterName, 0);
            }
        }

        public QuestInfo(QuestInfo other)
        {
            questName = other.questName;
            questDescription = other.questDescription;
            questType = other.questType;
            questState = other.questState;

            huntQuestCompleteCondition.Clear();

            foreach (HuntData huntConditionData in other.huntQuestCompleteCondition) 
            {
                huntQuestCompleteCondition.Add(new(huntConditionData));
            }

            huntQuestConditionTable.Clear();

            foreach (var keyAndValue in other.huntQuestConditionTable) 
            {
                huntQuestConditionTable.Add(keyAndValue.Key, keyAndValue.Value);
            }

            ongoingHuntTable.Clear();

            foreach (var keyAndValue in other.ongoingHuntTable) 
            {
                ongoingHuntTable.Add(keyAndValue.Key, keyAndValue.Value);
            }

            //TODO: other quest type data need to copy
        }

        public void Arrange()
        {
            foreach (HuntData huntData in huntQuestCompleteCondition)
            {
                huntQuestConditionTable.TryAdd(huntData.MonsterName, huntData.HuntCount);
                ongoingHuntTable.TryAdd(huntData.MonsterName, 0);
            }
        }

    }

    [Serializable]
    public class HuntData
    {
        [SerializeField]
        private EMonsterName monsterName;
        public EMonsterName MonsterName { get { return monsterName; } set { monsterName = value; } }

        [SerializeField]
        private int huntCount;
        public int HuntCount { get { return huntCount; } set { huntCount = value; } }

        public HuntData(HuntData other)
        {
            monsterName = other.monsterName;
            huntCount = other.huntCount;
        }
    }

    [Serializable]
    public class ItemCollectionData
    {
        [SerializeField]
        private EItemName collectingItemName;
        public EItemName CollectingItemName { get { return collectingItemName; } set { collectingItemName = value; } }

        [SerializeField]
        private int collectingItemCount;
        public int CollectingItemCount { get { return collectingItemCount; } set { collectingItemCount = value; } }
    }

}
