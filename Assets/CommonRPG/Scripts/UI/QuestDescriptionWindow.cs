using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CommonRPG
{
    public class QuestDescriptionWindow : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI questNameText;

        [SerializeField]
        private TextMeshProUGUI questDescriptionText;

        [SerializeField]
        private Button abandonQuestButton;

        private void Awake()
        {
            Debug.Assert(questNameText);
            Debug.Assert(questDescriptionText);
            Debug.Assert(abandonQuestButton);
        }

        public void SetQuestName(string newString)
        {
            questNameText.text = newString;
        }

        public void SetQuestDescription(string newString)
        {
            questDescriptionText.text = newString;
        }
    }
}
