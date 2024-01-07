using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CommonRPG
{
    [DefaultExecutionOrder(-10)]
    public class GameManager : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField]
        private TimerManager timerManager = null;

        [SerializeField]
        private InventoryManager inventoryManager = null;
        public static InventoryManager InventoryManager
        {
            get
            {
                return instance.inventoryManager;
            }
        }

        [SerializeField]
        private QuestManager questManager = null;
        public static QuestManager QuestManager { get { return instance.questManager; } }

        [SerializeField]
        private UnitManager unitManager = null;

        [Header("UIs")]
        [SerializeField]
        private InGameUI inGameUI = null;

        [SerializeField]
        private StatWindow statWindow = null;

        [SerializeField]
        private ItemInfoWindow itemInfoWindow = null;

        private static GameManager instance = null;

        [Header("Data")]
        [SerializeField]
        private ItemDataScriptableObject itemData = null;

        [SerializeField]
        private MonsterDataScriptableObject monsterData = null;

        [SerializeField]
        private LevelExpDataScriptableObject levelMaxExpData = null;

        /// <summary>
        /// call ReadyToUse method before using
        /// </summary>
        [SerializeField]
        private ItemDropDataScriptableObject itemDropData = null;

        [SerializeField]
        private CraftingRecipeDataScriptableObject craftingRecipeData = null;
        private Dictionary<string, SItemRecipeResultInfo> itemRecipeTable = null;

        //[Header("etc.")]
        //[SerializeField]
        //private AInventory playerInventory = null;

        private void Awake()
        {
            instance = this;
            Debug.Assert(instance);

            Debug.Assert(timerManager);
            Debug.Assert(inventoryManager);
            Debug.Assert(questManager);

            Debug.Assert(inGameUI);
            Debug.Assert(statWindow);
            Debug.Assert(itemInfoWindow);

            DontDestroyOnLoad(gameObject);

            itemDropData.ReadyToUse();

            craftingRecipeData.CreateRecipeTable();
            itemRecipeTable = craftingRecipeData.RecipeTable;
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            SetTimer(1, 0, 0, () => { InventoryManager.ObtainItem(EInventoryType.Equipment ,1, 1, itemData.ItemDataList[(int)EItemName.TheSecondSword].Data); }, true);
            SetTimer(2, 0, 0, () => 
            { 
                
                int count = InventoryManager.ObtainItem(EInventoryType.Equipment, 5, itemData.ItemDataList[(int)EItemName.TheFirstSword].Data);

            }, true);

            SetTimer(3, 0, 0, () => { InventoryManager.DeleteItem(EInventoryType.Equipment, 2, 1); }, true);
            SetTimer(4, 0, 0, () => { InventoryManager.DeleteItem(EInventoryType.Equipment, 3, 1); }, true);
            SetTimer(4, 0, 0, () => { SpawnItem(EItemName.TheFirstSword, Vector3.zero, Quaternion.identity, true); }, true);
        }

        private void OnEnable()
        {
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

        /// <summary>
        /// this is attach to transform
        /// if itemName is None, return null
        /// </summary>
        public static AItem SpawnItem(EItemName itemName, Transform transform, bool isFieldItem)
        {
            if (EItemName.None == itemName) 
            {
                return null;
            }

            ItemData itemData = instance.itemData.ItemDataList[(int)itemName];

            AItem item = Instantiate(itemData.ItemPrefab, transform);

            item.IsFieldItem = isFieldItem;
            item.Data = itemData.Data;

            if (isFieldItem) 
            {
                item.gameObject.layer = LayerMask.NameToLayer("FieldItem");
                item.EnableCollider(true);
            }

            return item;
        }

        /// <summary>
        /// if itemName is None, return null
        /// </summary>
        public static AItem SpawnItem(EItemName itemName, UnityEngine.Vector3 spawnPosition, UnityEngine.Quaternion rotation, bool isFieldItem)
        {
            if (EItemName.None == itemName)
            {
                return null;
            }

            ItemData itemData = instance.itemData.ItemDataList[(int)itemName];

            AItem item = Instantiate(instance.itemData.ItemDataList[(int)itemName].ItemPrefab, spawnPosition, rotation);

            item.IsFieldItem = isFieldItem;
            item.Data = itemData.Data;

            if (isFieldItem)
            {
                item.gameObject.layer = LayerMask.NameToLayer("FieldItem");
                item.EnableCollider(true);
            }

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

        public static MonsterBase SpawnMonster(EMonsterName monsterName, UnityEngine.Vector3 spawnPosition, UnityEngine.Quaternion rotation)
        {
            return instance.unitManager.SpawnMonster(instance.monsterData.MonsterDataList[(int)monsterName], spawnPosition, rotation);
        }

        public static void DeactiveMonster(MonsterBase monster)
        {
            instance.unitManager.DeactiveMonster(monster);
        }

        public static void OpenAndCloseInventory()
        {
            instance.inventoryManager.OpenAndCloseMainInventory();
            instance.statWindow.OpenAndCloseStatWindow();

            instance.itemInfoWindow.ShowOrHide(false);
        }

        public static bool IsInventoryOpened()
        {
            return instance.inventoryManager.IsInventoryOpened;
        }

        public static void UpdateStatWindow()
        {
            instance.statWindow.UpdateStatWindow();
        }

        public static float GetLevelMaxExpData(int level)
        {
            return instance.levelMaxExpData.GetLevelMaxExp(level);
        }

        public static int GetCurrentCoins()
        {
            return instance.inventoryManager.Coins;
        }

        public static void SetCoins(int amount)
        {
            instance.inventoryManager.Coins = amount;
        }

        public static void ShowItemInfoWindow(Vector2 slotPos, Vector2 slotWithAndHeight, in SItemData data)
        {
            instance.itemInfoWindow.SetToProperPosition(slotPos, slotWithAndHeight);
            instance.itemInfoWindow.SetItemInfoData(data);
            instance.itemInfoWindow.ShowOrHide(true);
        }

        public static void HideItemInfoWindow()
        {
            instance.itemInfoWindow.ShowOrHide(false);
        }

        public static AItem DropItemFromMonster(EMonsterName monsterName, Vector3 dropPos, Quaternion rotation)
        {
            EItemName chosenItem = instance.itemDropData.GetDropItem(monsterName);

            return SpawnItem(chosenItem, dropPos, rotation, true);
        }

        public static void SetActiveInteractioUI(bool ShouldActivate)
        {
            instance.inGameUI.SetActiveInteractioUI(ShouldActivate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>is it succeeded to get recipe?</returns>
        public static bool TryGetRecipe(in List<CraftingMaterialInfo> craftingRecipes, out SItemRecipeResultInfo resultInfo)
        {
            return instance.itemRecipeTable.TryGetValue(instance.craftingRecipeData.GenerateCraftingHashCode(craftingRecipes), out resultInfo);
        }

        public static void PrintAllQuests()
        {
            instance.questManager.PrintQuests();
        }
    }
}
