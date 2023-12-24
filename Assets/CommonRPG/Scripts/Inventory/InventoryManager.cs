using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CommonRPG
{
    public class InventoryManager : MonoBehaviour
    {
        [SerializeField]
        private Image dragSlotImage;
        public Image DragSltoImage
        {
            get { return dragSlotImage; }
            set { dragSlotImage = value; }
        }

        [SerializeField]
        private List<AInventory> inventoryList = null;

        [SerializeField]
        private GameObject mainInventoryObject = null;

        private InventorySlotItemData tempSlotItemData = new InventorySlotItemData();

        private bool isInventoryOpened = false;
        public bool IsInventoryOpened { get { return isInventoryOpened; } }

        private int coins = 0;
        public int Coins
        {
            get { return coins; }
            set
            {
                if (value < 0) 
                {
                    coins = 0;
                }
                else
                {
                    coins = value;
                }

                currentCoinText.text = coins.ToString();
            }
        }

        [SerializeField]
        private TextMeshProUGUI currentCoinText = null;

        [SerializeField]
        private TradeAmountDecisionWindow tradeAmountDecisionWindow = null;

        private void Awake()
        {
            Debug.Assert(inventoryList.Count > 0);
            inventoryList.Sort();

            foreach (AInventory inventory in inventoryList)
            {
                inventory.InitInventory();
                //inventory.gameObject.SetActive(true);
                //isInventoryOpened = true;
            }

            Debug.Assert(tradeAmountDecisionWindow);
        }

        private void Start()
        {
            //foreach (AInventory inventory in inventoryList)
            //{
            //    inventory.InitInventory();
            //    //inventory.gameObject.SetActive(false);
            //    //isInventoryOpened = false;
            //}
        }

        private void OnEnable()
        {
            foreach (AInventory inventory in inventoryList)
            {
                List<InventorySlotUI> inventorySlotUiList = inventory.SlotUiList;

                int slotUiListCount = inventorySlotUiList.Count;
                for (int i = 0; i < slotUiListCount; ++i)
                {
                    inventorySlotUiList[i].OnPointerEnterDelegate += OnPointerEnterToSlot;
                    inventorySlotUiList[i].OnPointerExitDelegate += OnPointerExitFromSlot;

                    inventorySlotUiList[i].OnRightMouseDownDelegate += OnRgihtMouseButtonDown;
                    inventorySlotUiList[i].OnLeftMouseDoubleClickDelegate += OnLeftMouseDoubleClick;

                    inventorySlotUiList[i].OnEndDragDelegate += ExchangeOrMoveOrMergeItem;
                }
            }

            tradeAmountDecisionWindow.OnTradeAcceptButtonClickedDelegate += OnTradeSucceeded;
        }

        private void OnDisable()
        {
            foreach (AInventory inventory in inventoryList)
            {
                List<InventorySlotUI> inventorySlotUiList = inventory.SlotUiList;

                int slotUiListCount = inventorySlotUiList.Count;
                for (int i = 0; i < slotUiListCount; ++i)
                {
                    inventorySlotUiList[i].OnPointerEnterDelegate -= OnPointerEnterToSlot;
                    inventorySlotUiList[i].OnPointerExitDelegate -= OnPointerExitFromSlot;

                    inventorySlotUiList[i].OnRightMouseDownDelegate -= OnRgihtMouseButtonDown;
                    inventorySlotUiList[i].OnLeftMouseDoubleClickDelegate -= OnLeftMouseDoubleClick;

                    inventorySlotUiList[i].OnEndDragDelegate -= ExchangeOrMoveOrMergeItem;
                }
            }

            tradeAmountDecisionWindow.OnTradeAcceptButtonClickedDelegate += OnTradeSucceeded;
        }

        public void ExchangeOrMoveOrMergeItem(int firstSlotIndex, int secondSlotIndex, EInventoryType firstInventoryType, EInventoryType secondInventoryType)
        {
            if (firstInventoryType == EInventoryType.MerchantInventory || secondInventoryType == EInventoryType.MerchantInventory) 
            {
                TradeWithMerchant(firstSlotIndex, firstInventoryType, secondInventoryType);
                return;
            }

            AInventory firstInventory = inventoryList[(int)firstInventoryType];
            AInventory secondInventory = inventoryList[(int)secondInventoryType];

            InventorySlotItemData firstSlotItemData = firstInventory.InventoryItemDataList[firstSlotIndex];
            InventorySlotItemData secondSlotItemData = secondInventory.InventoryItemDataList[secondSlotIndex];

            if (secondSlotItemData.CurrentItemCount > 0 && firstInventory.CheckAllowedItemInSlot(firstSlotIndex, secondSlotItemData.ItemData.ItemType) == false)
            {
                return;
            }

            if (firstSlotItemData.CurrentItemCount > 0 && secondInventory.CheckAllowedItemInSlot(secondSlotIndex, firstSlotItemData.ItemData.ItemType) == false)
            {
                return;
            }

            // merge
            if (firstSlotItemData.CurrentItemCount > 0 && secondSlotItemData.CurrentItemCount > 0 && firstSlotItemData.ItemData.ItemName == secondSlotItemData.ItemData.ItemName)
            {
                int totalItemCount = firstSlotItemData.CurrentItemCount + secondSlotItemData.CurrentItemCount;

                if (totalItemCount > secondSlotItemData.ItemData.MaxItemCount)
                {
                    firstInventory.SetSlotItemCount(firstSlotIndex, totalItemCount - secondSlotItemData.ItemData.MaxItemCount);
                    secondInventory.SetSlotItemCount(secondSlotIndex, secondSlotItemData.ItemData.MaxItemCount);
                }
                else
                {
                    firstInventory.SetSlotItemCount(firstSlotIndex, 0);
                    secondInventory.SetSlotItemCount(secondSlotIndex, totalItemCount);
                }

                return;
            }

            //exchange or move
            tempSlotItemData.CopyFrom(firstSlotItemData);
            firstInventory.SetItemInSlot(firstSlotIndex, secondSlotItemData);
            secondInventory.SetItemInSlot(secondSlotIndex, tempSlotItemData);
        }

        public void TradeWithMerchant(int slotIndexFrom, EInventoryType inventoryTypeFrom, EInventoryType inventoryTypeTo)
        {
            if (inventoryTypeFrom == inventoryTypeTo) 
            {
                return;
            }

            AInventory inventoryFrom = inventoryList[(int)inventoryTypeFrom];
            InventorySlotItemData slotItemDataFrom = inventoryFrom.InventoryItemDataList[slotIndexFrom];

            AInventory inventoryTo = inventoryList[(int)inventoryTypeTo];

            if ((slotItemDataFrom.ItemData.ItemType & inventoryTo.AllowedItemType) != slotItemDataFrom.ItemData.ItemType) 
            {
                Debug.Log("ItemType is Not Matched");
                return;
            }

            // buy
            if (inventoryTypeFrom == EInventoryType.MerchantInventory)
            {
                int buyPrice = slotItemDataFrom.ItemData.BuyPrice;

                if (buyPrice > Coins || inventoryTo.GetEmptySlotIndex() == -1) 
                {
                    Debug.Log("Fail to buy");
                    return;
                    // fail to buy item
                }

                tradeAmountDecisionWindow.ReadyToShowWindow(slotIndexFrom, slotItemDataFrom, inventoryTypeFrom, inventoryTypeTo, Coins, buyPrice);
                tradeAmountDecisionWindow.gameObject.SetActive(true);
                //// need to confirm if you will buy this item or not and amount...
                //Coins -= buyPrice;

                //int buyAmount = 1;
                //inventoryTo.ObtainItem(inventoryTo.GetEmptySlotIndex(), buyAmount, slotItemDataFrom.ItemData);
                
            }
            // sell
            else if (inventoryTypeTo == EInventoryType.MerchantInventory) 
            {
                int sellPrice = slotItemDataFrom.ItemData.SellPrice;

                tradeAmountDecisionWindow.ReadyToShowWindow(slotIndexFrom, slotItemDataFrom, inventoryTypeFrom, inventoryTypeTo, Coins, sellPrice);
                tradeAmountDecisionWindow.gameObject.SetActive(true);
                //// need to confirm if you will sell this item or not and amount
                //Coins += sellPrice;

                //int sellAmount = 1;
                //inventoryFrom.DeleteItem(slotIndexFrom, sellAmount);
            }
        }

        public void DisplayMerchantGoods(List<InventorySlotItemData> goodsList)
        {
            AInventory merchantInventory = inventoryList[(int)EInventoryType.MerchantInventory];
            int inventorySlotCount = merchantInventory.InventoryItemDataList.Count;
            int goodsCount = goodsList.Count;

            for (int i = 0; i < inventorySlotCount; ++i) 
            {
                MerchantInventorySlotUI merchantSlotUI = (MerchantInventorySlotUI)merchantInventory.SlotUiList[i];

                if (merchantSlotUI == null)
                {
                    Debug.LogAssertion("This is not merchantSlotUI");
                    return;
                }

                if (i < goodsCount)
                {
                    goodsList[i].CurrentItemCount = 1;
                    merchantInventory.SetItemInSlot(i, goodsList[i]);

                    merchantSlotUI.SetSellItemNameInfoText(goodsList[i].ItemData.ItemName.ToString());
                    merchantSlotUI.SetSellItemPriceInfoText(goodsList[i].ItemData.BuyPrice.ToString());

                    continue;
                }

                merchantInventory.SetSlotItemCount(i, 0);

                merchantSlotUI.SetSellItemNameInfoText("");
                merchantSlotUI.SetSellItemPriceInfoText("");
            }
        }

        public void ObtainItem(EInventoryType inventoryType, int slotIndex, int itemAddCount, in SItemData itemData)
        {
            inventoryList[(int)inventoryType].ObtainItem(slotIndex, itemAddCount, itemData);
        }

        /// <summary>
        ///  add item to inventory automatically as much as itemAddcount until inventory has no space.
        /// </summary>
        /// <returns> remaining item count after adding items until inventory is filled.</returns>
        public int ObtainItem(EInventoryType inventoryType, int itemAddCount, in SItemData itemData)
        {
            return inventoryList[(int)inventoryType].ObtainItem(itemAddCount, itemData);
        }

        public void DeleteItem(EInventoryType inventoryType, int slotIndex, int deleteCount)
        {
            inventoryList[(int)inventoryType].DeleteItem(slotIndex, deleteCount);
        }

        public void UseSlotItem(int slotIndex, EInventoryType inventoryType)
        {
            inventoryList[(int)inventoryType].UseSlotItem(slotIndex);
        }

        public void UseQuickSlot(EInputKey inputKey)
        {
            QuickSlot quickSlot = (QuickSlot)inventoryList[(int)EInventoryType.QuickSlot];
            if (quickSlot == null) 
            {
                return;
            }

            quickSlot.UseQuickSlot(inputKey);
        }

        public void OpenAndCloseMainInventory()
        {
            isInventoryOpened = (isInventoryOpened == false);

            mainInventoryObject.SetActive(isInventoryOpened);
            inventoryList[(int)EInventoryType.EquipmentScreen].gameObject.SetActive(isInventoryOpened);

            Time.timeScale = (isInventoryOpened) ? 0 : 1;
            Cursor.visible = isInventoryOpened;
            Cursor.lockState = (IsInventoryOpened) ? CursorLockMode.Confined : CursorLockMode.Locked;

            currentCoinText.text = coins.ToString();
        }

        public void OpenEquipmentInventory()
        {
            inventoryList[(int)EInventoryType.Equipment].gameObject.SetActive(true);
            inventoryList[(int)EInventoryType.MiscItemInventory].gameObject.SetActive(false);
        }

        public void OpenMiscInevntory()
        {
            inventoryList[(int)EInventoryType.Equipment].gameObject.SetActive(false);
            inventoryList[(int)EInventoryType.MiscItemInventory].gameObject.SetActive(true);
        }

        public void OpenAndCloseMerchantInventory(bool ShouldOpen)
        {
            inventoryList[(int)EInventoryType.MerchantInventory].gameObject.SetActive(ShouldOpen);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns> if inventory is full, return -1</returns>
        public int GetEmptySlotInex(EInventoryType inventoryType)
        {
            return inventoryList[(int)inventoryType].GetEmptySlotIndex();
        }

        private void OnPointerEnterToSlot(int slotIndex, EInventoryType inventoryType, Vector2 slotPos, Vector2 slotWidthAndHeight)
        {
            AInventory inventory = inventoryList[(int)inventoryType];
            if (inventory.SlotUiList[slotIndex].IsEmpty) 
            {
                return;
            }

            GameManager.ShowItemInfoWindow(slotPos, slotWidthAndHeight, inventory.InventoryItemDataList[slotIndex].ItemData);
        }

        private void OnPointerExitFromSlot(int slotIndex)
        {
            GameManager.HideItemInfoWindow();
        }

        private void OnRgihtMouseButtonDown(int slotIndex, EInventoryType inventoryType)
        {
            inventoryList[(int)inventoryType].AbandonItem(slotIndex);
        }

        private void OnLeftMouseDoubleClick(int slotIndex, EInventoryType inventoryType)
        {
            UseSlotItem(slotIndex, inventoryType);
        }

        private void OnTradeSucceeded(int tradeSlotIndexFrom, InventorySlotItemData tradeSlotItemData, EInventoryType tradeInventoryTypeFrom, EInventoryType tradeInventoryTypeTo, int totalPrice, int tradeAmount)
        {
            AInventory inventoryFrom = inventoryList[(int)tradeInventoryTypeFrom];
            AInventory inventoryTo = inventoryList[(int)tradeInventoryTypeTo];

            // buy
            if (tradeInventoryTypeFrom == EInventoryType.MerchantInventory)
            {
                Coins -= totalPrice;
                inventoryTo.ObtainItem(inventoryTo.GetEmptySlotIndex(), tradeAmount, tradeSlotItemData.ItemData);

            }
            // sell
            else if (tradeInventoryTypeTo == EInventoryType.MerchantInventory)
            {
                Coins += totalPrice;
                inventoryFrom.DeleteItem(tradeSlotIndexFrom, tradeAmount);
            }
        }
    }
}
