using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public class QuickSlot : AInventory
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

        public override void UseSlotItem(int slotIndex)
        {
            if (inventoryItemDataList[slotIndex].ItemData.ItemType == EItemType.Misc)
            {
                UseMiscItem(slotIndex);
            }
            else if (inventoryItemDataList[slotIndex].ItemData.ItemType == EItemType.Weapon)
            {
                UseWeaponItem(slotIndex);
            }
        }

        public void UseMiscItem(int slotIndex)
        {
            KnightAnimController animController = (KnightAnimController)GameManager.GetPlayer().AnimController;
            if (animController == null || animController.IsDrinking || animController.IsHit)
            {
                return;
            }

            InventorySlotItemData slotItemData = InventoryItemDataList[slotIndex];

            if (slotItemData.CurrentItemCount == 0)
            {
                return;
            }

            EItemName itemName = slotItemData.ItemData.ItemName;

            switch (itemName)
            {
                case EItemName.HpPotion:
                {
                    StatComponent playerStatComponent = GameManager.GetPlayer().StatComponent;

                    if (playerStatComponent.CurrentHealthPoint >= playerStatComponent.TotalHealth)
                    {
                        Debug.Log("Health is already full");
                        return;
                    }

                    playerStatComponent.CurrentHealthPoint += slotItemData.ItemData.HPBonus;

                    if (playerStatComponent.CurrentHealthPoint >= playerStatComponent.TotalHealth)
                    {
                        playerStatComponent.CurrentHealthPoint = playerStatComponent.TotalHealth;
                    }

                    GameManager.SetPlayerHealthBarFillRatio(playerStatComponent.CurrentHealthPoint / playerStatComponent.TotalHealth);
                    GameManager.UpdateStatWindow();

                    animController.PlayDrinkAnim();

                    break;
                }
                default:
                {
                    Debug.LogAssertion("Weird item name");
                    return;
                }
            }

            DeleteItem(slotIndex, 1);
        }

        public void UseWeaponItem(int slotIndex)
        {
            GameManager.InventoryManager.ExchangeOrMoveOrMergeItem(slotIndex, 0, InventoryType, EInventoryType.EquipmentScreen);
        }

        public void UseQuickSlot(EInputKey inputKey)
        {
            UseSlotItem((int)inputKey);
        }
    }
}

