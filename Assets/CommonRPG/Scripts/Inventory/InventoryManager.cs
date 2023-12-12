using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EInventoryType
{
    Equipment,
    EquipmentScreen,
}

public class InventoryManager : MonoBehaviour
{
    private List<InventorySlotItemData> equipmentItemDataList = null;
    private InventorySlotUI[] equipmentSlotUIs = null;

    private void Awake()
    {
        equipmentSlotUIs = GetComponentsInChildren<InventorySlotUI>(true);
        Debug.Assert(equipmentSlotUIs != null && equipmentSlotUIs.Length > 0);

        int slotUIsLength = equipmentSlotUIs.Length;
        for (int i = 0; i < slotUIsLength; i++) 
        {
            equipmentSlotUIs[i].SlotIndex = i;
            equipmentSlotUIs[i].IsEmpty = true;
        }

        equipmentItemDataList = new List<InventorySlotItemData>(slotUIsLength);
        for (int i = 0; i < slotUIsLength; i++) 
        {
            equipmentItemDataList.Add(new InventorySlotItemData());
        }
    }

    public void ObtainItem(int slotIndex, int itemAddCount, in SItemData slotItemData)
    {
        Debug.AssertFormat(slotIndex < equipmentItemDataList.Count, $"slotIndex : {slotIndex}, equipmentItemDataList.Count : {equipmentItemDataList.Count}");

        int totalItemCount = equipmentItemDataList[slotIndex].CurrentItemCount + itemAddCount;
        if (totalItemCount > slotItemData.MaxItemCount)
        {
            Debug.LogWarning($"Too many items... cant obtain items.. CurrentItemCount : {equipmentItemDataList[slotIndex].CurrentItemCount}, itemAddCount : {itemAddCount}, MaxItemCount : {slotItemData.MaxItemCount}");
            return;
        }

        equipmentItemDataList[slotIndex].slotItemData = slotItemData;
        equipmentItemDataList[slotIndex].CurrentItemCount = totalItemCount;
        equipmentSlotUIs[slotIndex].SetSlotImageSprite(slotItemData.SlotSprite);
        equipmentSlotUIs[slotIndex].SetSlotItemCountText(totalItemCount);
        equipmentSlotUIs[slotIndex].IsEmpty = (totalItemCount == 0);
    }
    /// <summary>
    ///  add item to inventory automatically as much as itemAddcount until inventory has no space.
    /// </summary>
    /// <returns> remaining item count after adding items until inventory is filled.</returns>
    public int ObtainItem(int itemAddCount, in SItemData slotItemData)
    {
        int TotalItemAddCount = itemAddCount;

        int equipmentItemdataListCount = equipmentItemDataList.Count;
        for (int i = 0; i < equipmentItemdataListCount; ++i) 
        {
            InventorySlotItemData data = equipmentItemDataList[i];
            
            if (data.slotItemData.ItemName != slotItemData.ItemName && data.slotItemData.ItemName != EItemName.None) 
            {
                continue;
            }

            if (data.slotItemData.MaxItemCount == data.CurrentItemCount && data.slotItemData.ItemName != EItemName.None)  
            {
                continue;
            }

            int addableItemCount = 0;
            if (data.slotItemData.ItemName == EItemName.None) 
            {
                addableItemCount = slotItemData.MaxItemCount;
                equipmentSlotUIs[i].SetSlotImageSprite(slotItemData.SlotSprite);
                data.slotItemData = slotItemData;
            }
            else
            {
                addableItemCount = data.slotItemData.MaxItemCount - data.CurrentItemCount;
            }

            int remainingItemCount = TotalItemAddCount - addableItemCount;
            if (remainingItemCount > 0)
            {
                data.CurrentItemCount = slotItemData.MaxItemCount;
                TotalItemAddCount = remainingItemCount;
            }
            else
            {
                data.CurrentItemCount = TotalItemAddCount;
                TotalItemAddCount = 0;
            }

            equipmentSlotUIs[i].SetSlotItemCountText(data.CurrentItemCount);

            if (TotalItemAddCount == 0)
            {
                return 0;
            }
        }

        return TotalItemAddCount;
    }

    public void DeleteItem(int slotIndex, int deleteCount)
    {
        if (deleteCount >= equipmentItemDataList[slotIndex].CurrentItemCount) 
        {
            equipmentItemDataList[slotIndex].CurrentItemCount = 0;

            equipmentItemDataList[slotIndex].slotItemData.ItemName = EItemName.None;

            equipmentSlotUIs[slotIndex].SetSlotImageSprite(null);
            equipmentSlotUIs[slotIndex].SetSlotItemCountText(0);
        }
        else
        {
            equipmentItemDataList[slotIndex].CurrentItemCount -= deleteCount;
            equipmentSlotUIs[slotIndex].SetSlotItemCountText(equipmentItemDataList[slotIndex].CurrentItemCount);
        }
    }

    public void UseItem(int slotIndex)
    {

    }
}

public class InventorySlotItemData
{
    public SItemData slotItemData = new SItemData();
    public int CurrentItemCount = 0;

    public InventorySlotItemData()
    {
        slotItemData.ItemName = EItemName.None;
    }

    public void UseSlotItem()
    {
        
    }
}