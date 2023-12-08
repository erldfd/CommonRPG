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
        private InGameUI inGameUIInstance = null;
        private static GameManager instance = null;

        private void Awake()
        {
            instance = this;

            Debug.Assert(instance);
            Debug.Assert(inGameUI);
            Debug.Assert(timerManager);

            DontDestroyOnLoad(gameObject);

            inGameUIInstance = Instantiate(inGameUI);
            Debug.Assert(inGameUIInstance);

            DontDestroyOnLoad(inGameUIInstance);
        }

        private void Start()
        {
            Debug.Log("GameManager Start");

            //TimerHandler handler1 = SetTimer(5, 1, 0, () => 
            //{   
            //    Debug.Log("Handler 1 (3, 1, 3)");
            //    TimerHandler handlerInternal = SetTimer(5, 1, 1, () => 
            //    { 
            //        Debug.Log("HandlerInternal");
            //        TimerHandler handlerInternalInternal = SetTimer(5, 2, 5, () => { Debug.Log("HandlerInternal 2"); }, true);
                
            //    }, true);

            //}, true);
        }

        private void OnEnable()
        {
            Debug.Log("GameManager OnEnable");
        }

        public static void SetInGameUIVisible(bool shouldVisible)
        {
            instance.inGameUIInstance.gameObject.SetActive(shouldVisible);
        }

        public static void SetMonsterInfoUIVisible(bool shouldVisible)
        {
            instance.inGameUIInstance.SetMonsterInfoUIVisible(shouldVisible);
        }

        public static void SetMonsterNameText(string NewName)
        {
            instance.inGameUIInstance.SetMonsterNameText(NewName);
        }

        public static void SetMonsterHealthBarFillRatio(float ratio)
        {
            instance.inGameUIInstance.SetMonsterHealthBarFillRatio(ratio);
        }

        public static void SetPlayerInfoUIVisible(bool shouldVisible)
        {
            instance.inGameUIInstance.SetPlayerInfoUIVisible(shouldVisible);
        }

        public static void SetPlayerNameText(string NewName)
        {
            instance.inGameUIInstance.SetPlayerNameText(NewName);
        }

        public static void SetPlayerHealthBarFillRatio(float ratio)
        {
            instance.inGameUIInstance.SetPlayerHealthBarFillRatio(ratio);
        }

        public static void SetPlayerManaBarFillRatio(float ratio)
        {
            instance.inGameUIInstance.SetPlayerManaBarFillRatio(ratio);
        }

        public static void SetPlayerLevelText(int NewLevel)
        {
            instance.inGameUIInstance.SetPlayerLevelText(NewLevel);
        }

        public static void SetPauseUIVisible(bool shouldVisible)
        {
            instance.inGameUIInstance.SetPauseUIVisible(shouldVisible);
        }

        public static TimerHandler SetTimer(float startTime, float interval, int repeatNumber, Action function, bool isActive)
        {
            return instance.timerManager.SetTimer(startTime, interval, repeatNumber, function, isActive);
        }
    }
}
