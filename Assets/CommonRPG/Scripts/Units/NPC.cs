using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public class NPC : AUnit
    {
        [SerializeField]
        private List<ConversationDataScriptableObject> conversations = new List<ConversationDataScriptableObject>();
        public List<ConversationDataScriptableObject> Conversations { get { return conversations; } }

        [SerializeField]
        private ConversationDataScriptableObject currentConversationData;
        public ConversationDataScriptableObject CurrentConversationData { get { return currentConversationData; } }

        protected override void Awake()
        {
            base.Awake();

            currentConversationData = conversations[0];
        }
    }
}
