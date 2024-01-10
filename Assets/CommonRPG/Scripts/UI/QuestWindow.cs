using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CommonRPG
{
    public class QuestWindow : MonoBehaviour
    {
        [SerializeField]
        private Button unlockedQuestButton;

        [SerializeField]
        private Button ongoingQuestButton;

        [SerializeField]
        private Button completedQuestButton;

        [SerializeField]
        private QuestNameView questNameView;
        public QuestNameView QuestNameView { get { return questNameView; } }

        [SerializeField]
        private QuestDescriptionWindow questDescriptionWindow;

        private void Awake()
        {
            Debug.Assert(unlockedQuestButton);
            Debug.Assert(ongoingQuestButton);
            Debug.Assert(completedQuestButton);

            Debug.Assert(questNameView);
            Debug.Assert(questDescriptionWindow);
        }

        private void OnEnable()
        {
            questNameView.OnQuestNameEntryClickedDelegate += OnQuestNameEntryClicked;

            unlockedQuestButton.onClick.AddListener(OnUnlockedQuestButtonClicked);
            ongoingQuestButton.onClick.AddListener(OnOngoingQuestButtonClicked);
            completedQuestButton.onClick.AddListener(OnCompletedQuestButtonClicked);

        }

        private void OnDisable()
        {
            questNameView.OnQuestNameEntryClickedDelegate -= OnQuestNameEntryClicked;

            unlockedQuestButton.onClick.RemoveListener(OnUnlockedQuestButtonClicked);
            ongoingQuestButton.onClick.RemoveListener(OnOngoingQuestButtonClicked);
            completedQuestButton.onClick.RemoveListener(OnCompletedQuestButtonClicked);
        }

        private void OnUnlockedQuestButtonClicked()
        {
            questNameView.ShowUnlockedQuestWindow();
        }

        private void OnOngoingQuestButtonClicked()
        {
            questNameView.ShowOngoingQuestWindow();
        }

        private void OnCompletedQuestButtonClicked()
        {
            questNameView.ShowCompletedQuestWindow();
        }

        private void OnQuestNameEntryClicked(string questName, string questDescription)
        {
            questDescriptionWindow.SetQuestName(questName);
            questDescriptionWindow.SetQuestDescription(questDescription);
        }
    }
}
