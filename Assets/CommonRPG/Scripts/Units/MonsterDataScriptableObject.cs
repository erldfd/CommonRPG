using System;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public enum EMonsterName
    {
        None = -1,
        RedSlime = 0,
        BlueTurtle = 1,
    }

    [CreateAssetMenu(fileName = "MonsterData", menuName = "ScriptableObjects/MonsterDataScriptableObject", order = 2)]
    public class MonsterDataScriptableObject : ScriptableObject
    {
        [SerializeField]
        private List<MonsterData> monsterDataList;
        public List<MonsterData> MonsterDataList { get { return monsterDataList; } }
    }

    [Serializable]
    public class MonsterData
    {
        [SerializeField]
        private string name = null;

        [SerializeField]
        private SMonsterData data;
        public SMonsterData Data { get { return data; } }

        [SerializeField]
        private MonsterBase monsterPrefab = null;
        public MonsterBase MonsterPrefab { get { return monsterPrefab; } }
    }

    [Serializable]
    public struct SMonsterData
    {
        public EMonsterName MonsterName;
        public float Damage;
        public float Defense;
        public float Hp;
        public float Mp;
        public float Exp;
        public float ExpTolerance;
        public float HoldingMoney;
        public float MoneyTolerance;
        [HideInInspector]
        public List<EItemName> HoldingItems;
    }
}

