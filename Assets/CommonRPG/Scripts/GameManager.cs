using System;
using UnityEngine;

namespace CommonRPG
{
    public class GameManager : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField]
        private TimerManager timerManager = null;

        [SerializeField]
        private InGameUI inGameUI = null;
        //private InGameUI inGameUIInstance = null;
        private static GameManager instance = null;

        private void Awake()
        {
            instance = this;

            Debug.Assert(instance);
            Debug.Assert(inGameUI);
            Debug.Assert(timerManager);

            DontDestroyOnLoad(gameObject);

            //inGameUIInstance = Instantiate(inGameUI);
            //Debug.Assert(inGameUIInstance);

            //DontDestroyOnLoad(inGameUIInstance);
        }

        private void Start()
        {
            Debug.Log("GameManager Start");
        }

        private void OnEnable()
        {
            Debug.Log("GameManager OnEnable");
        }

        public static void SetInGameUIVisible(bool shouldVisible)
        {
            instance.inGameUI.gameObject.SetActive(shouldVisible);
        }

        public static void SetMonsterInfoUIVisible(bool shouldVisible)
        {
            instance.inGameUI.SetMonsterInfoUIVisible(shouldVisible);
        }

        public static void SetMonsterNameText(string NewName)
        {
            instance.inGameUI.SetMonsterNameText(NewName);
        }

        public static void SetMonsterHealthBarFillRatio(float ratio)
        {
            instance.inGameUI.SetMonsterHealthBarFillRatio(ratio);
        }

        public static void SetPlayerInfoUIVisible(bool shouldVisible)
        {
            instance.inGameUI.SetPlayerInfoUIVisible(shouldVisible);
        }

        public static void SetPlayerNameText(string NewName)
        {
            instance.inGameUI.SetPlayerNameText(NewName);
        }

        public static void SetPlayerHealthBarFillRatio(float ratio)
        {
            Debug.Assert(instance);
            Debug.Assert(instance.inGameUI);
            instance.inGameUI.SetPlayerHealthBarFillRatio(ratio);
        }

        public static void SetPlayerManaBarFillRatio(float ratio)
        {
            instance.inGameUI.SetPlayerManaBarFillRatio(ratio);
        }

        public static void SetPlayerLevelText(int NewLevel)
        {
            instance.inGameUI.SetPlayerLevelText(NewLevel);
        }

        public static void SetPauseUIVisible(bool shouldVisible)
        {
            instance.inGameUI.SetPauseUIVisible(shouldVisible);
        }

        public static TimerHandler SetTimer(float startTime, float interval, int repeatNumber, Action function, bool isActive)
        {
            return instance.timerManager.SetTimer(startTime, interval, repeatNumber, function, isActive);
        }
    }
}
