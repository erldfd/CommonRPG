using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public class Merchant : NPC
    {
        [SerializeField]
        private List<InventorySlotItemData> merchantGoodsDataList;
        public List<InventorySlotItemData> MerchantGoodsDataList
        {
            get { return merchantGoodsDataList; }
        }

        public override void InteractWithPlayer()
        {
            base.InteractWithPlayer();

            if (CurrentConversationData)
            {
                GameManager.InGameUI.ReadyToConversate(CurrentConversationData);
                GameManager.SetActiveInteractioUI(false);
                return;
            }

            GameManager.InventoryManager.DisplayMerchantGoods(merchantGoodsDataList);
            GameManager.InventoryManager.OpenAndCloseMerchantInventory(true);
            GameManager.SetActiveInteractioUI(false);
        }
    }
}
