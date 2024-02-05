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
    }
}
