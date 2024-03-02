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

        [SerializeField]
        private Image completeSignImage;

        private void Awake()
        {
            Debug.Assert(questNameText);
            Debug.Assert(questDescriptionText);
            Debug.Assert(abandonQuestButton);
            Debug.Assert(completeSignImage);
        }

        private void OnEnable()
        {
            GameManager.QuestManager.OnPendingQuestDelegate += OnPendingQuest;
        }

        private void OnDisable()
        {
            GameManager.QuestManager.OnPendingQuestDelegate -= OnPendingQuest;
        }

        public void SetQuestName(string newString)
        {
            questNameText.text = newString;
        }

        public void SetQuestDescription(string newString)
        {
            questDescriptionText.text = newString;
        }

        public void SetActiveAbandonQuestButton(bool shouldActivate)
        {
            abandonQuestButton.gameObject.SetActive(shouldActivate);
        }

        public void SetActiveCompleteSignImage(bool shouldActivate)
        {
            completeSignImage.gameObject.SetActive(shouldActivate);
        }

        private void OnPendingQuest(int questId)
        {
            SetActiveCompleteSignImage(true);
        }
    }
}
