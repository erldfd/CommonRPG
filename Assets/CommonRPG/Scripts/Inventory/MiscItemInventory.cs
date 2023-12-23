

using UnityEngine;

namespace CommonRPG
{
    public class MiscItemInventory : AInventory
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

        public override void UseSlotItem(int slotIndex)
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
                    break;
                }
            }

            DeleteItem(slotIndex, 1);
        }
    }
}

