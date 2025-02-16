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

            InventorySlotItemData standardItem = inventoryItemDataList[0];
            int inventoryItemDataListCount = inventoryItemDataList.Count;

            // merge
            for (int i = 1; i < inventoryItemDataListCount; ++i)
            {
                if (inventoryItemDataList[i].CurrentItemCount == 0)
                {
                    continue;
                }

                if (standardItem.ItemData.ItemName == inventoryItemDataList[i].ItemData.ItemName)
                {
                    int remainingSpace = standardItem.ItemData.MaxItemCount - standardItem.CurrentItemCount;

                    if (remainingSpace >= inventoryItemDataList[i].CurrentItemCount)
                    {
                        standardItem.CurrentItemCount += inventoryItemDataList[i].CurrentItemCount;
                        inventoryItemDataList[i].CurrentItemCount = 0;
                        inventoryItemDataList[i].ItemData.ItemType = EItemType.None;
                    }
                    else
                    {
                        standardItem.CurrentItemCount += remainingSpace;
                        inventoryItemDataList[i].CurrentItemCount -= remainingSpace;
                        standardItem = inventoryItemDataList[i];
                    }
                }
                else
                {
                    standardItem = inventoryItemDataList[i];
                }
            }

            inventoryItemDataList.Sort();

            for (int i = 0; i < inventoryItemDataListCount; ++i)
            {
                if (inventoryItemDataList[i].CurrentItemCount == 0)
                {
                    slotUiList[i].SetSlotImageSprite(emptySlotSprite);
                    slotUiList[i].SetSlotItemCountText(0);
                    continue;
                }

                slotUiList[i].SetSlotImageSprite(inventoryItemDataList[i].ItemData.SlotSprite);
                slotUiList[i].SetSlotItemCountText(inventoryItemDataList[i].CurrentItemCount);
            }
        }
    }
}
