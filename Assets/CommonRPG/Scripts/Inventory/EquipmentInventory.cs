using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public class EquipmentInventory : AInventory
    {
        protected override void Awake()
        {
            base.Awake();
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

        public override void UseSlotItem(int slotIndex)
        {
            EquipmentScreen.EEquipmentSlot weaponEquipSlot = EquipmentScreen.EEquipmentSlot.Weapon;
            GameManager.InventoryManager.ExchangeOrMoveOrMergeItem(slotIndex, (int)weaponEquipSlot, InventoryType, EInventoryType.EquipmentScreen);
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
