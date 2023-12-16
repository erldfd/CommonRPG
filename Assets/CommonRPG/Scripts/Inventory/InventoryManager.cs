using System.Collections;
using System.Collections.Generic;
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

        private InventorySlotItemData tempSlotItemData = new InventorySlotItemData();

        private bool isInventoryOpened = false;
        public bool IsInventoryOpened { get { return isInventoryOpened; } }

        private void Awake()
        {
            Debug.Assert(inventoryList.Count > 0);
            inventoryList.Sort();

            foreach (AInventory inventory in inventoryList)
            {
                inventory.InitInventory();
                inventory.gameObject.SetActive(true);
                isInventoryOpened = true;
            }
        }

        private void Start()
        {
            foreach (AInventory inventory in inventoryList)
            {
                inventory.InitInventory();
                inventory.gameObject.SetActive(false);
                isInventoryOpened = false;
            }
        }

        private void OnEnable()
        {
            foreach (AInventory inventory in inventoryList)
            {
                List<InventorySlotUI> inventorySlotUiList = inventory.SlotUiList;

                int slotUiListCount = inventorySlotUiList.Count;
                for (int i = 0; i < slotUiListCount; ++i)
                {
                    inventorySlotUiList[i].OnEndDragDelegate += ExchangeOrMoveOrMergeItem;
                }
            }
        }

        private void OnDisable()
        {
            foreach (AInventory inventory in inventoryList)
            {
                List<InventorySlotUI> inventorySlotUiList = inventory.SlotUiList;

                int slotUiListCount = inventorySlotUiList.Count;
                for (int i = 0; i < slotUiListCount; ++i)
                {
                    inventorySlotUiList[i].OnEndDragDelegate -= ExchangeOrMoveOrMergeItem;
                }
            }
        }

        public void ExchangeOrMoveOrMergeItem(int firstSlotIndex, int secondSlotIndex, EInventoryType firstInventoryType, EInventoryType secondInventoryType)
        {
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

        public void OpenAndCloseMainInventory()
        {
            isInventoryOpened = (isInventoryOpened == false);

            foreach (AInventory inventory in inventoryList)
            {
                inventory.gameObject.SetActive(isInventoryOpened);
            }

            Time.timeScale = (isInventoryOpened) ? 0 : 1;
            Cursor.visible = isInventoryOpened;
            Cursor.lockState = (IsInventoryOpened) ? CursorLockMode.Confined : CursorLockMode.Locked;
        }
    }

}
