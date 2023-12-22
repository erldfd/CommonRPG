using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CommonRPG
{
    public class MerchantInventorySlotUI : InventorySlotUI
    {
        [SerializeField]
        private TextMeshProUGUI sellItemNameInfoText = null;

        [SerializeField]
        private TextMeshProUGUI sellItemPriceInfoText = null;

        public void SetSellItemNameInfoText(string newText)
        {
            sellItemNameInfoText.text = newText;
        }

        public void SetSellItemPriceInfoText(string newText)
        {
            sellItemPriceInfoText.text = newText;
        }
    }
}
