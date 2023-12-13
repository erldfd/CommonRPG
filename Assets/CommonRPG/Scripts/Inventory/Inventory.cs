using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TreeEditor;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum EInventoryType
{
    None = -1,
    Equipment = 0,
    EquipmentScreen = 1,
}


public class Inventory : MonoBehaviour, IComparable<Inventory>
{
    [SerializeField]
    protected EInventoryType inventoryType = EInventoryType.None;
    public EInventoryType InventoryType { get { return inventoryType; } }

    [SerializeField]
    protected EItemType allowedItemType = EItemType.None;

    [SerializeField]
    protected List<InventorySlotUI> slotUiList = null;
    public List<InventorySlotUI> SlotUiList { get { return slotUiList; } }

    protected List<InventorySlotItemData> inveitoryItemDataList = null;
    public List<InventorySlotItemData> InventoryItemDataList { get { return inveitoryItemDataList; } }

    protected virtual void Awake()
    {
        Debug.Assert(slotUiList != null && slotUiList.Count > 0);

        int slotUIsLength = slotUiList.Count;
        for (int i = 0; i < slotUIsLength; i++)
        {
            slotUiList[i].SlotIndex = i;
            slotUiList[i].IsEmpty = true;
        }

        inveitoryItemDataList = new List<InventorySlotItemData>(slotUIsLength);
        for (int i = 0; i < slotUIsLength; i++)
        {
            inveitoryItemDataList.Add(new InventorySlotItemData());
        }
    }

    public virtual void ObtainItem(int slotIndex, int itemAddCount, in SItemData slotItemData)
    {
        Debug.AssertFormat(slotIndex < inveitoryItemDataList.Count, $"slotIndex : {slotIndex}, equipmentItemDataList.Count : {inveitoryItemDataList.Count}");

        int totalItemCount = inveitoryItemDataList[slotIndex].CurrentItemCount + itemAddCount;
        if (totalItemCount > slotItemData.MaxItemCount)
        {
            Debug.LogWarning($"Too many items... cant obtain items.. CurrentItemCount : {inveitoryItemDataList[slotIndex].CurrentItemCount}, itemAddCount : {itemAddCount}, MaxItemCount : {slotItemData.MaxItemCount}");
            return;
        }

        inveitoryItemDataList[slotIndex].ItemData = slotItemData;
        inveitoryItemDataList[slotIndex].CurrentItemCount = totalItemCount;

        slotUiList[slotIndex].CurrentSlotInventoryType = inventoryType;
        slotUiList[slotIndex].SetSlotImageSprite(slotItemData.SlotSprite);
        slotUiList[slotIndex].SetSlotItemCountText(totalItemCount);
        slotUiList[slotIndex].IsEmpty = (totalItemCount == 0);
    }
    /// <summary>
    ///  add item to inventory automatically as much as itemAddcount until inventory has no space.
    /// </summary>
    /// <returns> remaining item count after adding items until inventory is filled.</returns>
    public virtual int ObtainItem(int itemAddCount, in SItemData slotItemData)
    {
        int TotalItemAddCount = itemAddCount;

        int equipmentItemdataListCount = inveitoryItemDataList.Count;
        for (int i = 0; i < equipmentItemdataListCount; ++i) 
        {
            InventorySlotItemData data = inveitoryItemDataList[i];
            
            if (data.ItemData.ItemName != slotItemData.ItemName && data.ItemData.ItemName != EItemName.None) 
            {
                continue;
            }

            if (data.ItemData.MaxItemCount == data.CurrentItemCount && data.ItemData.ItemName != EItemName.None)  
            {
                continue;
            }

            int addableItemCount = 0;
            if (data.ItemData.ItemName == EItemName.None) 
            {
                addableItemCount = slotItemData.MaxItemCount;
                slotUiList[i].CurrentSlotInventoryType = inventoryType;
                slotUiList[i].SetSlotImageSprite(slotItemData.SlotSprite);
                data.ItemData = slotItemData;
            }
            else
            {
                addableItemCount = data.ItemData.MaxItemCount - data.CurrentItemCount;
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

            slotUiList[i].SetSlotItemCountText(data.CurrentItemCount);

            if (TotalItemAddCount == 0)
            {
                return 0;
            }
        }

        return TotalItemAddCount;
    }

    public virtual void DeleteItem(int slotIndex, int deleteCount)
    {
        if (deleteCount >= inveitoryItemDataList[slotIndex].CurrentItemCount) 
        {
            inveitoryItemDataList[slotIndex].CurrentItemCount = 0;

            inveitoryItemDataList[slotIndex].ItemData.ItemName = EItemName.None;

            slotUiList[slotIndex].SetSlotImageSprite(null);
            slotUiList[slotIndex].SetSlotItemCountText(0);
        }
        else
        {
            inveitoryItemDataList[slotIndex].CurrentItemCount -= deleteCount;
            slotUiList[slotIndex].SetSlotItemCountText(inveitoryItemDataList[slotIndex].CurrentItemCount);
        }
    }

    public virtual void UseItem(int slotIndex)
    {

    }

    public virtual void PaintSlotImage(int slotIndex, Sprite sprite)
    {
        slotUiList[slotIndex].SetSlotImageSprite(sprite);
    }

    public virtual void SetItemCountText(int slotIndex, int itemCount)
    {
        slotUiList[slotIndex].SetSlotItemCountText(itemCount);
    }

    public virtual bool CheckAllowedItem(EItemType checkingItemType)
    {
        return ((allowedItemType & checkingItemType) != 0);
    }

    public int CompareTo(Inventory other)
    {
        if (other == null || (int)InventoryType > (int)other.InventoryType)
        {
            return 1;
        }
        else if ((int)InventoryType == (int)other.InventoryType) 
        {
            return 0;
        }
        else
        {
            return -1;
        }
    }
}

[Serializable]
public class InventorySlotItemData
{
    [SerializeField]
    private SItemData itemData = new SItemData();
    public ref SItemData ItemData { get { return ref itemData; } }

    [HideInInspector]
    public int CurrentItemCount = 0;

    public void CopyFrom(InventorySlotItemData previousData)
    {
        itemData = previousData.ItemData;
        CurrentItemCount = previousData.CurrentItemCount;
    }

    public InventorySlotItemData()
    {
        ItemData.ItemName = EItemName.None;
    }

    public void UseSlotItem()
    {

    }
}