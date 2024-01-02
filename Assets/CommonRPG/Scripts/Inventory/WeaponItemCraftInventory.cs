using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public class WeaponItemCraftInventory : AInventory
    {
        public enum EWeaponItemCraftingSlot
        {
            CraftingMaterialSlot1,
            CraftingMaterialSlot2,
            CraftedItemSlot,
        }

        private List<CraftingMaterialInfo> materialInfoList = new List<CraftingMaterialInfo>();

        public override void InitInventory()
        {
            base.InitInventory();

            int slotUIsLength = base.slotUiList.Count;
            materialInfoList.Capacity = slotUIsLength;

            for (int i = 0; i < slotUIsLength; i++)
            {
                base.slotUiList[i].CurrentSlotInventoryType = inventoryType;
                materialInfoList.Add(new CraftingMaterialInfo(EItemName.None, 0));
            }

            base.slotUiList[(int)EWeaponItemCraftingSlot.CraftingMaterialSlot1].AllowedItemType = EItemType.Weapon;
            base.slotUiList[(int)EWeaponItemCraftingSlot.CraftingMaterialSlot2].AllowedItemType = EItemType.Weapon;
            base.slotUiList[(int)EWeaponItemCraftingSlot.CraftedItemSlot].AllowedItemType = EItemType.None;
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

        public void TryCrafting()
        {
            int inventoryItemDataListCount = InventoryItemDataList.Count;
            if (inventoryItemDataListCount == 0)
            {
                Debug.Log("Crafting Access Denied");
                return;
            }

            int resultSlotIndex = SlotUiList.Count - 1;
            if (SlotUiList[resultSlotIndex].IsEmpty == false)
            {
                Debug.Log("Crafting Access Denied");
                return;
            }

            int index = 0;

            foreach (InventorySlotItemData itemData in InventoryItemDataList) 
            {
                if (itemData.CurrentItemCount == 0) 
                {
                    continue;
                }

                materialInfoList[index++].SetInfos(itemData.ItemData.ItemName, itemData.CurrentItemCount);
            }

            SItemRecipeResultInfo recipeResultInfo;
            bool isSucceeded = GameManager.TryGetRecipe(materialInfoList, out recipeResultInfo);

            if (isSucceeded == false) 
            {
                int randomIndex = Random.Range(0, resultSlotIndex);

                DeleteItem(randomIndex, 9999);
                materialInfoList[randomIndex].SetInfos(EItemName.None, 0);

                return;
            }

            Debug.Log($"Crafting succeeded : {recipeResultInfo.ResultItem}, {recipeResultInfo.ItemCount}");

            InventorySlotItemData slotItemData = InventoryItemDataList[resultSlotIndex];
            slotItemData.CopyFrom(GameManager.GetItemData(recipeResultInfo.ResultItem).Data, recipeResultInfo.ItemCount);

            SetItemInSlot(resultSlotIndex, slotItemData);

            for (int i = 0; i < resultSlotIndex; ++i) 
            {
                DeleteItem(i, 9999);
                materialInfoList[i].SetInfos(EItemName.None, 0);
            }
        }
    }
}
