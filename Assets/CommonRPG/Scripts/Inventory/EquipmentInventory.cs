using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentInventory : AInventory
{
    protected override void Awake()
    {
        base.Awake();

        //int slotUIsLength = base.slotUiList.Count;
        //for (int i = 0; i < slotUIsLength; i++)
        //{
        //    base.slotUiList[i].CurrentSlotInventoryType = inventoryType;
        //    base.slotUiList[i].AllowedItemType = allowedItemType;
        //}

    }

    public override void InitInventory()
    {
        base.InitInventory();

        int slotUIsLength = base.slotUiList.Count;
        for (int i = 0; i < slotUIsLength; i++)
        {
            base.slotUiList[i].CurrentSlotInventoryType = inventoryType;
            base.slotUiList[i].AllowedItemType = allowedItemType;
        }
    }
}
