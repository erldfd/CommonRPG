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
        private Sprite selectedButtonSprite;

        [SerializeField]
        private Sprite unselectedButtonSprite;

        [SerializeField]
        private Color selectedColor;

        [SerializeField]
        private Color unselectedColor;

        private Image unlockedQuestButtonImage;
        private Image ongoingQuestButtonImage;
        private Image completedQuestButtonImage;

        private TextMeshProUGUI unlockedQuestButtonText;
        private TextMeshProUGUI ongoingQuestButtonText;
        private TextMeshProUGUI completedQuestButtonText;

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

            unlockedQuestButtonImage = unlockedQuestButton.GetComponent<Image>();
            Debug.Assert(unlockedQuestButtonImage);

            ongoingQuestButtonImage = ongoingQuestButton.GetComponent<Image>();
            Debug.Assert(ongoingQuestButtonImage);

            completedQuestButtonImage = completedQuestButton.GetComponent<Image>();
            Debug.Assert(completedQuestButtonImage);

            unlockedQuestButtonText = unlockedQuestButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            Debug.Assert(unlockedQuestButtonText);

            ongoingQuestButtonText = ongoingQuestButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            Debug.Assert(ongoingQuestButtonText);

            completedQuestButtonText = completedQuestButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            Debug.Assert(completedQuestButtonText);

            questDescriptionWindow.SetQuestName("");
            questDescriptionWindow.SetQuestDescription("");
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

            unlockedQuestButtonImage.sprite = selectedButtonSprite;
            ongoingQuestButtonImage.sprite = unselectedButtonSprite;
            completedQuestButtonImage.sprite = unselectedButtonSprite;

            unlockedQuestButtonText.color = selectedColor;
            ongoingQuestButtonText.color = unselectedColor;
            completedQuestButtonText.color = unselectedColor;
        }

        private void OnOngoingQuestButtonClicked()
        {
            questNameView.ShowOngoingQuestWindow();

            unlockedQuestButtonImage.sprite = unselectedButtonSprite;
            ongoingQuestButtonImage.sprite = selectedButtonSprite;
            completedQuestButtonImage.sprite = unselectedButtonSprite;

            unlockedQuestButtonText.color = unselectedColor;
            ongoingQuestButtonText.color = selectedColor;
            completedQuestButtonText.color = unselectedColor;
        }

        private void OnCompletedQuestButtonClicked()
        {
            questNameView.ShowCompletedQuestWindow();

            unlockedQuestButtonImage.sprite = unselectedButtonSprite;
            ongoingQuestButtonImage.sprite = unselectedButtonSprite;
            completedQuestButtonImage.sprite = selectedButtonSprite;

            unlockedQuestButtonText.color = unselectedColor;
            ongoingQuestButtonText.color = unselectedColor;
            completedQuestButtonText.color = selectedColor;
        }

        private void OnQuestNameEntryClicked(string questName, string questDescription)
        {
            questDescriptionWindow.SetQuestName(questName);
            questDescriptionWindow.SetQuestDescription(questDescription);

            EQuestState questState = GameManager.QuestManager.GetQuestStateFromQuestName(questName);
            bool CanAbandonQuest = (questState == EQuestState.Ongoing || questState == EQuestState.Pending);

            questDescriptionWindow.SetActiveAbandonQuestButton(CanAbandonQuest);
            questDescriptionWindow.SetActiveCompleteSignImage(questState == EQuestState.Pending);
        }
    }
}
