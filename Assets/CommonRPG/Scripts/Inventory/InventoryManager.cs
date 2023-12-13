using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField]
    private List<Inventory> inventoryList = null;

    private InventorySlotItemData tempSlotItemData = new InventorySlotItemData();

    private void Awake()
    {
        Debug.Assert(inventoryList.Count > 0);
        inventoryList.Sort();
    }

    private void OnEnable()
    {
        foreach (Inventory inventory in inventoryList)
        {
            List<InventorySlotUI> inventorySlotUiList = inventory.SlotUiList;

            int slotUiListCount = inventorySlotUiList.Count;
            for (int i = 0; i < slotUiListCount; ++i)
            {
                inventorySlotUiList[i].OnEndDragDelegate += ExchangeOrMoveItem;
            }
        }
    }

    private void OnDisable()
    {
        foreach (Inventory inventory in inventoryList)
        {
            List<InventorySlotUI> inventorySlotUiList = inventory.SlotUiList;

            int slotUiListCount = inventorySlotUiList.Count;
            for (int i = 0; i < slotUiListCount; ++i)
            {
                inventorySlotUiList[i].OnEndDragDelegate -= ExchangeOrMoveItem;
            }
        }
    }

    public void ExchangeOrMoveItem(int firstSlotIndex, int secondSlotIndex, EInventoryType firstInventoryType, EInventoryType secondInventoryType)
    {
        Inventory firstInventory = inventoryList[(int)firstInventoryType];
        Inventory secondInventory = inventoryList[(int)secondInventoryType];

        InventorySlotItemData firstSlotItemData = firstInventory.InventoryItemDataList[firstSlotIndex];
        InventorySlotItemData secondSlotItemData = secondInventory.InventoryItemDataList[secondSlotIndex];

        if (secondSlotItemData.CurrentItemCount != 0 && firstInventory.CheckAllowedItem(secondSlotItemData.ItemData.ItemType) == false) 
        {
            return;
        }

        if (firstSlotItemData.CurrentItemCount != 0 && secondInventory.CheckAllowedItem(firstSlotItemData.ItemData.ItemType) == false)
        {
            return;
        }

        //tempSlotItemData = new InventorySlotItemData(firstSlotItemData);
        //firstSlotItemData = new InventorySlotItemData(secondSlotItemData);
        //secondSlotItemData = new InventorySlotItemData(tempSlotItemData);

        tempSlotItemData.CopyFrom(firstSlotItemData);
        firstSlotItemData.CopyFrom(secondSlotItemData);
        secondSlotItemData.CopyFrom(tempSlotItemData);

        int firstSlotItemCount = firstSlotItemData.CurrentItemCount;
        int secondSlotItemCount = secondSlotItemData.CurrentItemCount;

        firstInventory.SetItemCountText(firstSlotIndex, firstSlotItemCount);
        secondInventory.SetItemCountText(secondSlotIndex, secondSlotItemCount);

        if (firstSlotItemCount == 0) 
        {
            firstInventory.PaintSlotImage(firstSlotIndex, null);
            
        }
        else
        {
            firstInventory.PaintSlotImage(firstSlotIndex, firstSlotItemData.ItemData.SlotSprite);
        }

        if (secondSlotItemCount == 0) 
        {
            secondInventory.PaintSlotImage(secondSlotIndex, null);
        }
        else
        {
            secondInventory.PaintSlotImage(secondSlotIndex, secondSlotItemData.ItemData.SlotSprite);
        }
    }
}
