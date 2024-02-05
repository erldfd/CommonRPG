using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CommonRPG.ConversationDataScriptableObject;

namespace CommonRPG
{
    [CreateAssetMenu(fileName = "ConversationData", menuName = "ScriptableObjects/ConversationDataScriptableObject", order = 6)]
    public class ConversationDataScriptableObject : ScriptableObject
    {

        [SerializeField]
        private string converstionDataName;
        public string ConversationDataName { get { return converstionDataName; } set { converstionDataName = value; } }

        [SerializeField]
        private List<DrawingNodeInfo> drawInfoNodes = new();
        public List<DrawingNodeInfo> DrawInfoNodes { get { return drawInfoNodes; } }

        [SerializeField]
        private Dictionary<int, ConversationNode> conversationTable = new();
        public Dictionary<int, ConversationNode> ConversationTable
        {
            get { return conversationTable; }
        }

        public void ReadyToStart()
        {
            if (ConversationTable.Count > 0) 
            {
                return;
            }

            foreach (DrawingNodeInfo drawingNodeInfo in drawInfoNodes) 
            {
                ConversationNode conversationNode = new(drawingNodeInfo.SpeakerName, drawingNodeInfo.ParentId, drawingNodeInfo.ChildrenIds, drawingNodeInfo.NodeId, drawingNodeInfo.Conversations, drawingNodeInfo.NodeType == ENodeType.Choice);
                ConversationTable.Add(conversationNode.MyID, conversationNode);
            }
        }

        [Serializable]
        public class ConversationNode
        {
            [SerializeField]
            private string speakerName;
            public string SpeakerName { get { return speakerName; } set { speakerName = value; } }
            private int parentID = 0;
            public int ParentID { get { return parentID; } set { parentID = value; } }
            private List<int> childrenIDs = new();
            public List<int> ChildrenIDs { get { return childrenIDs; } set { childrenIDs = value; } }
            private int myID = 0;
            public int MyID { get { return myID; } set { myID = value; } }
            public List<string> Conversations = new List<string>();
            public bool IsChoiceConversation = false;
            public ConversationNode(string speakerName, int parentID, List<int> childrenIDs, int myID, List<string> conversations, bool isChoiceConversation)
            {
                this.speakerName = speakerName;
                this.parentID = parentID;
                this.childrenIDs = childrenIDs;
                this.myID = myID;

                Conversations = conversations;
                IsChoiceConversation = isChoiceConversation;
            }
        }

        [Serializable]
        public class DrawingNodeInfo
        {
            public string SpeakerName;

            public int NodeId = 0;
            public int ParentId = 0;
            public List<int> ChildrenIds = new();

            public string NodeName;
            public ENodeType NodeType;
            public Rect NodeRect;

            public List<string> Conversations = new();
        }
    }
}
