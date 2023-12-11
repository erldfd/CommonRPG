using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private List<SInventorySlotItemData> equipmentItemDataList = null;
    private InventorySlotUI[] equipmentSlotUIs = null;

    private void Awake()
    {
        equipmentSlotUIs = GetComponentsInChildren<InventorySlotUI>(true);
        Debug.Assert(equipmentSlotUIs != null && equipmentSlotUIs.Length > 0);

        int slotUIsLength = equipmentSlotUIs.Length;
        for (int i = 0; i < slotUIsLength; i++) 
        {
            equipmentSlotUIs[i].SlotIndex = i;
        }

        equipmentItemDataList = new List<SInventorySlotItemData>(slotUIsLength);
        for (int i = 0; i < slotUIsLength; i++) 
        {
            equipmentItemDataList.Add(new SInventorySlotItemData());
        }
    }

    public void ObtainItem(int slotIndex, in SInventorySlotItemData slotItemData)
    {
        Debug.AssertFormat(slotIndex < equipmentItemDataList.Count, $"slotIndex : {slotIndex}, equipmentItemDataList.Count : {equipmentItemDataList.Count}");
        equipmentItemDataList[slotIndex] = slotItemData;
        equipmentSlotUIs[slotIndex].SetSlotImageSprite(slotItemData.SlotSprite);
        equipmentSlotUIs[slotIndex].SetSlotItemCountText(slotItemData.CurrentItemCount);
    }
}
