using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

namespace CommonRPG
{
    [CreateAssetMenu(fileName = "LevelExpData", menuName = "ScriptableObjects/LevelExpDataScriptableObject", order = 3)]
    public class LevelExpDataScriptableObject : ScriptableObject
    {
        [SerializeField]
        private List<float> levelMaxExpList;

        public float GetLevelMaxExp(int level)
        {
            return levelMaxExpList[level];
        }
    }
}

