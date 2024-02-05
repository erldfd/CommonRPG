using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public class NPC : AUnit
    {
        [SerializeField]
        private ConversationDataScriptableObject conversationData;
        public ConversationDataScriptableObject ConversationData { get { return conversationData; } }

        protected override void Awake()
        {
            base.Awake();

        }
    }
}
