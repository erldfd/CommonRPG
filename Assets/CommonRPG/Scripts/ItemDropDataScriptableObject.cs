using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    /// <summary>
    /// call ReadyToUse method before using
    /// </summary>
    [CreateAssetMenu(fileName = "ItemDropData", menuName = "ScriptableObjects/ItemDropDataScriptableObject", order = 4)]
    public class ItemDropDataScriptableObject : ScriptableObject
    {
        [SerializeField]
        private List<SItemDropData> itemDropDataList;
        public List<SItemDropData> ItemDropDataList { get { return itemDropDataList; } }

        /// <summary>
        /// call this method before using
        /// </summary>
        public void ReadyToUse()
        {
            itemDropDataList.Sort();
        }

        /// <summary>
        /// proablility is 0 ~ 1
        /// </summary>
        public EItemName GetDropItemData(EMonsterName monsterName, float probability)
        {
            SItemDropData itemDropData = itemDropDataList[(int)monsterName];
            List<SItemDropProbabilityData> probabilityDataList = itemDropData.ItemProbabilityDataList;
            int itemProbabilityDataListCount = probabilityDataList.Count;

            float totalProbability = 0;

            for (int i = 0; i < itemProbabilityDataListCount; ++i) 
            {
                totalProbability += probabilityDataList[i].dropProbability;
            }

            float chosenProbalility = totalProbability * probability;
            float calculatedProbability = 0;
            EItemName chosenItemName = EItemName.None;

            for (int i = 0; i < itemProbabilityDataListCount; ++i) 
            {
                if (calculatedProbability > chosenProbalility) 
                {
                    chosenItemName = probabilityDataList[i].ItemName;
                }
                else
                {
                    calculatedProbability += probabilityDataList[i].dropProbability;
                }
            }

            return chosenItemName;
        }
    }

    [Serializable]
    public struct SItemDropData : IComparable<SItemDropData>
    {
        [SerializeField]
        private string name;

        public EMonsterName MonsterName;
        public List<SItemDropProbabilityData> ItemProbabilityDataList;

        public int CompareTo(SItemDropData other)
        {
            if (MonsterName > other.MonsterName)
            {
                return 1;
            }
            else if (MonsterName == other.MonsterName)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
    }

    [Serializable]
    public struct SItemDropProbabilityData
    {
        public EItemName ItemName;
        public float dropProbability;
    }
}
