using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public class WeaponItemCraftingStation : AInventory
    {
        public enum EWeaponItemCraftingSlot
        {
            CraftedItemSlot,
            CraftingMaterialSlot1,
            CraftingMaterialSlot2
        }

        public override void InitInventory()
        {
            base.InitInventory();

            int slotUIsLength = base.slotUiList.Count;
            for (int i = 0; i < slotUIsLength; i++)
            {
                base.slotUiList[i].CurrentSlotInventoryType = inventoryType;
            }

            base.slotUiList[(int)EWeaponItemCraftingSlot.CraftedItemSlot].AllowedItemType = EItemType.None;
            base.slotUiList[(int)EWeaponItemCraftingSlot.CraftingMaterialSlot1].AllowedItemType = EItemType.Weapon;
            base.slotUiList[(int)EWeaponItemCraftingSlot.CraftingMaterialSlot2].AllowedItemType = EItemType.Weapon;
        }

        public override void ObtainItem(int slotIndex, int itemAddCount, in SItemData itemData)
        {
            // dont use this function
        }

        public override int ObtainItem(int itemAddCount, in SItemData itemData)
        {
            // dont use this function
            return itemAddCount;
        }

        /// <summary>
        /// This method is overridden. Actual function is to move to characterInventory
        /// </summary>
        public override void AbandonItem(int slotIndex)
        {
            int emptySlotIndex = GameManager.InventoryManager.GetEmptySlotInex(EInventoryType.Equipment);
            if (emptySlotIndex == -1)
            {
                Debug.Log("Inventory is full");
                return;
            }

            GameManager.InventoryManager.ExchangeOrMoveOrMergeItem(slotIndex, emptySlotIndex, InventoryType, EInventoryType.Equipment);
        }
    }
}
