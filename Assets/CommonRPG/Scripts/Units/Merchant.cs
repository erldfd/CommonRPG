using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public class Merchant : AUnit
    {
        [SerializeField]
        private List<InventorySlotItemData> merchantGoodsDataList;
    }
}
