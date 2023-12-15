using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

namespace CommonRPG
{
    [DefaultExecutionOrder(-1)]
    public class GameManager : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField]
        private TimerManager timerManager = null;

        [SerializeField]
        private InventoryManager inventoryManager = null;

        [SerializeField]
        private UnitManager unitManager = null;

        [SerializeField]
        private InGameUI inGameUI = null;
        //private InGameUI inGameUIInstance = null;
        private static GameManager instance = null;

        [SerializeField]
        private ItemDataScriptableObject itemData = null;

        [SerializeField]
        private MonsterDataScriptableObject monsterData = null;

        [SerializeField]
        private AInventory playerInventory = null;

        private void Awake()
        {
            instance = this;
            
            Debug.Assert(instance);
            Debug.Assert(inGameUI);
            Debug.Assert(timerManager);

           // playerInventory = GetComponentInChildren<Inventory>();
            Debug.Assert(playerInventory);

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            Debug.Log("GameManager Start");

            SetTimer(1, 0, 0, () => { playerInventory.ObtainItem(1, 1, itemData.ItemDataList[(int)EItemName.TheSecondSword].Data); }, true);
            SetTimer(2, 0, 0, () => 
            { 
                
                int count = playerInventory.ObtainItem(5, itemData.ItemDataList[(int)EItemName.TheFirstSword].Data);
                Debug.Log(count);

            }, true);

            SetTimer(3, 0, 0, () => { playerInventory.DeleteItem(2, 1); }, true);
            SetTimer(4, 0, 0, () => { playerInventory.DeleteItem(3, 1); }, true);
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

        public static ItemData GetItemData(EItemName itemName)
        {
            return instance.itemData.ItemDataList[(int)itemName];
        }

        public static AItem SpawnItem(EItemName itemName, Transform transform, bool isFieldItem)
        {
            ItemData itemData = instance.itemData.ItemDataList[(int)itemName];

            AItem item = Instantiate(itemData.ItemPrefab, transform);
            item.IsFieldItem = isFieldItem;
            item.Data = itemData.Data;

            return item;
        }

        public static AItem SpawnItem(EItemName itemName, UnityEngine.Vector3 spawnPosition, UnityEngine.Quaternion rotation, bool isFieldItem)
        {
            AItem item = Instantiate(instance.itemData.ItemDataList[(int)itemName].ItemPrefab, spawnPosition, rotation);
            item.IsFieldItem = isFieldItem;

            return item;
        }

        public static ACharacter GetPlayer()
        {
            return instance.unitManager.Player;
        }

        public static void GetDragSlotImage(out Image dragSlotImage)
        {
            dragSlotImage = instance.inventoryManager.DragSltoImage;
        }

        public static MonsterData GetMonsterData(EMonsterName monsterName)
        {
            return instance.monsterData.MonsterDataList[(int)monsterName];
        }

        public static void SpawnMonster(EMonsterName monsterName, UnityEngine.Vector3 spawnPosition, UnityEngine.Quaternion rotation)
        {
            instance.unitManager.SpawnMonster(instance.monsterData.MonsterDataList[(int)monsterName], spawnPosition, rotation);
        }

        public static void DeactiveMonster(MonsterBase monster)
        {
            instance.unitManager.DeactiveMonster(monster);
        }
    }
}
