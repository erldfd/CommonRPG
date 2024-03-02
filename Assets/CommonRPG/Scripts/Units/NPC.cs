using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public class NPC : AUnit
    {
        [SerializeField]
        protected List<ConversationDataScriptableObject> conversations = new List<ConversationDataScriptableObject>();
        public List<ConversationDataScriptableObject> Conversations { get { return conversations; } }

        [SerializeField]
        protected ConversationDataScriptableObject currentConversationData;
        public ConversationDataScriptableObject CurrentConversationData { get { return currentConversationData; } }

        [SerializeField]
        protected List<string> questNameList;

        protected List<int> questIdList = new List<int>();

        protected int currentQuestId;

        protected override void Awake()
        {
            base.Awake();

            if (conversations.Count > 0) 
            {
                currentConversationData = conversations[0];
            }

            questIdList.Clear();

            foreach (string questName in questNameList)
            {
                questIdList.Add(GameManager.QuestManager.QuestNameIdTable[questName]);
            }

            int questIdListCount = questIdList.Count;
            for (int i = 0; i < questIdListCount; ++i) 
            {
                GameManager.QuestManager.UnlockQuest(questIdList[i]);
            }

            if (questIdListCount > 0) 
            {
                currentQuestId = questIdList[0];
            }
        }

        public virtual void InteractWithPlayer()
        {
           
        }
    }
}
