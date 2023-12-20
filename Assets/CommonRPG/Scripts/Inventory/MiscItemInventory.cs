using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public class MiscItemInventory : AInventory
    {
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

        public override void SortSlotItem()
        {
            inventoryItemDataList.Sort();

            int inventoryItemDataListCount = inventoryItemDataList.Count;
            for (int i = 0; i < inventoryItemDataListCount; ++i)
            {
                if (inventoryItemDataList[i].CurrentItemCount == 0)
                {
                    slotUiList[i].SetSlotImageSprite(null);
                    slotUiList[i].SetSlotItemCountText(0);
                    continue;
                }

                slotUiList[i].SetSlotImageSprite(inventoryItemDataList[i].ItemData.SlotSprite);
                slotUiList[i].SetSlotItemCountText(inventoryItemDataList[i].CurrentItemCount);
            }
        }
    }
}

