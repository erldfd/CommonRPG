using System;
using UnityEngine;

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
        TimerHandler timerHandler = new TimerHandler();
        timerHandler.RepeatNumber = 0;
        timerHandler.StartTime = 5;
        timerHandler.Function = () => { Debug.Log("StartTime 5 funxtion"); };
        timerManager.SetTimer(timerHandler);

        TimerHandler timerHandler2 = new TimerHandler();
        timerHandler2.RepeatNumber = 3;
        timerHandler2.StartTime = 10;
        timerHandler2.Interval = 3;
        timerHandler2.Function = TestFunction;
        timerManager.SetTimer(timerHandler2);
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

    private void TestFunction()
    {
        Debug.Log("Timer Text Function");
    }
}