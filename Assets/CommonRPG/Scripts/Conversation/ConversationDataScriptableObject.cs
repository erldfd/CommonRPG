using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    [CreateAssetMenu(fileName = "ConversationData", menuName = "ScriptableObjects/ConversationDataScriptableObject", order = 6)]
    public class ConversationDataScriptableObject : ScriptableObject
    {
        [SerializeField]
        private List<ConversationNode> conversations;

        private Dictionary<int, ConversationNode> conversationTable = new();
    }

    [Serializable]
    public class ConversationNode
    {
        [SerializeField]
        private string speakerName;
        public string SpeakerName { get { return speakerName; } set { speakerName = value; } } 

        private int parentID;
        public int ParentID { get { return parentID; } set { parentID = value; } }

        private List<int> childrenIDs = new();
        public List<int> ChildrenIDs { get { return childrenIDs; } set { childrenIDs = value; } }

        private int myID;
        public int MyID { get { return myID; } set { myID = value; } }

        public List<string> Conversations = new List<string>();

        public bool IsSelectConversation = false;
    }
}
