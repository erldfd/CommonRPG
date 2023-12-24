using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CommonRPG
{
    public class TradeAmountDecisionWindow : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI totalPriceInfoText = null;

        [SerializeField]
        private Slider amountDecisionSlider = null;
        public Slider AmountDecisionSlider { get { return amountDecisionSlider; } }

        [SerializeField]
        private TMP_InputField amountInputField = null;

        [SerializeField]
        private Button minButton = null;

        [SerializeField]
        private Button maxButton = null;

        [SerializeField]
        private Button tradeAcceptButton = null;

        [SerializeField]
        private Button tradeCancelButton = null;

        [SerializeField]
        private Button amountIncreaseButton = null;

        [SerializeField]
        private Button amountDecreaseButton = null;

        private int decidedAmount = 0;
        public int DecidedAmount { get { return decidedAmount; } }

        private int itemPriceAPiece = 0;
        public int ItemPriceAPiece { get { return itemPriceAPiece; } }

        /// <summary>
        /// int tradeSlotIndexFrom,InventorySlotItemData tradeSlotItemData, EInventoryType tradeInventoryTypeFrom, EInventoryType tradeInventoryTypeTo, int totalPrice, int tradeItemAmount
        /// </summary>
        public event Action<int, InventorySlotItemData, EInventoryType, EInventoryType, int, int> OnTradeAcceptButtonClickedDelegate = null;

        // initial state is -1
        private int tradeSlotIndex = -1;
        private InventorySlotItemData tradeSlotItemData = null;
        private EInventoryType tradeInventoryTypeFrom;
        private EInventoryType tradeInventoryTypeTo;

        private void Awake()
        {
            Debug.Assert(totalPriceInfoText);
            Debug.Assert(amountDecisionSlider);
            Debug.Assert(amountInputField);

            Debug.Assert(minButton);
            Debug.Assert(maxButton);
            Debug.Assert(tradeAcceptButton);
            Debug.Assert(tradeCancelButton);
            Debug.Assert(amountIncreaseButton);
            Debug.Assert(amountDecreaseButton);
        }

        private void OnEnable()
        {
            AmountDecisionSlider.onValueChanged.AddListener(OnSliderValueChanged);
            amountInputField.onEndEdit.AddListener(OnEndInputFieldValueEdit);

            minButton.onClick.AddListener(OnMinButtonClicked);
            maxButton.onClick.AddListener(OnMaxButtonClicked);

            tradeAcceptButton.onClick.AddListener(OnTradeAcceptButtonClicked);
            tradeCancelButton.onClick.AddListener(OnTradeCancelButtonClicked);
            amountIncreaseButton.onClick.AddListener(OnAmountIncreaseButtonClicked);
            amountDecreaseButton.onClick.AddListener(OnAmountDecreaseButtonClicked);

        }

        private void OnDisable()
        {
            AmountDecisionSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
            amountInputField.onEndEdit.RemoveListener(OnEndInputFieldValueEdit);

            minButton.onClick.RemoveListener(OnMinButtonClicked);
            maxButton.onClick.RemoveListener(OnMaxButtonClicked);

            tradeAcceptButton.onClick.RemoveListener(OnTradeAcceptButtonClicked);
            tradeCancelButton.onClick.RemoveListener(OnTradeCancelButtonClicked);
            amountIncreaseButton.onClick.RemoveListener(OnAmountIncreaseButtonClicked);
            amountDecreaseButton.onClick.RemoveListener(OnAmountDecreaseButtonClicked);
        }

        public void SetSliderMaxValue(int max)
        {
            AmountDecisionSlider.maxValue = max;
        }

        public void ReadyToShowWindow(int tradeSlotIndexFrom, InventorySlotItemData tradeSlotItemData, EInventoryType tradeInventoryTypeFrom, EInventoryType tradeInventoryTypeTo, int currnetHoldingCoins, int itemPriceAPiece)
        {
            amountInputField.text = "0";
            AmountDecisionSlider.value = 0;
            decidedAmount = 0;
            totalPriceInfoText.text = "0";

            int maxItemTradeAmount;

            // buy
            if (tradeInventoryTypeFrom == EInventoryType.MerchantInventory)
            {
                maxItemTradeAmount = currnetHoldingCoins / itemPriceAPiece;

                if (maxItemTradeAmount > tradeSlotItemData.ItemData.MaxItemCount)
                {
                    maxItemTradeAmount = tradeSlotItemData.ItemData.MaxItemCount;
                }
            }
            // sell
            else
            {
                maxItemTradeAmount = tradeSlotItemData.CurrentItemCount;
            }
             

            if (maxItemTradeAmount > tradeSlotItemData.ItemData.MaxItemCount)
            {
                maxItemTradeAmount = tradeSlotItemData.ItemData.MaxItemCount;
            }

            SetSliderMaxValue(maxItemTradeAmount);

            this.itemPriceAPiece = itemPriceAPiece;

            this.tradeSlotIndex = tradeSlotIndexFrom;
            this.tradeSlotItemData = tradeSlotItemData;
            this.tradeInventoryTypeFrom = tradeInventoryTypeFrom;
            this.tradeInventoryTypeTo = tradeInventoryTypeTo;

        }

        private void OnSliderValueChanged(float changedValue)
        {
            decidedAmount = (int)changedValue;
            amountInputField.text = decidedAmount.ToString();
            totalPriceInfoText.text = (decidedAmount * ItemPriceAPiece).ToString();
        }

        private void OnEndInputFieldValueEdit(string value)
        {
            bool pharseSucceeded = int.TryParse(value, out decidedAmount);

            if (pharseSucceeded == false) 
            {
                amountInputField.text = "0";
                AmountDecisionSlider.value = 0;
                decidedAmount = 0;
            }
            else
            {
                AmountDecisionSlider.value = decidedAmount;
            }

            totalPriceInfoText.text = (decidedAmount * ItemPriceAPiece).ToString();
        }

        private void OnMinButtonClicked()
        {
            AmountDecisionSlider.value = 0;
            decidedAmount = 0;
            amountInputField.text = "0";
        }

        private void OnMaxButtonClicked()
        {
            AmountDecisionSlider.value = AmountDecisionSlider.maxValue;
            decidedAmount = (int)AmountDecisionSlider.maxValue;
            amountInputField.text = decidedAmount.ToString();
        }

        private void OnTradeAcceptButtonClicked()
        {
            OnTradeAcceptButtonClickedDelegate.Invoke(tradeSlotIndex, tradeSlotItemData, tradeInventoryTypeFrom, tradeInventoryTypeTo, DecidedAmount * ItemPriceAPiece, DecidedAmount);

            amountInputField.text = "0";
            AmountDecisionSlider.value = 0;
            decidedAmount = 0;
            totalPriceInfoText.text = "0";

            tradeSlotIndex = -1;
            tradeSlotItemData = null;
            tradeInventoryTypeFrom = EInventoryType.None;
            tradeInventoryTypeTo = EInventoryType.None;

            gameObject.SetActive(false);
        }

        private void OnTradeCancelButtonClicked()
        {
            amountInputField.text = "0";
            AmountDecisionSlider.value = 0;
            decidedAmount = 0;
            totalPriceInfoText.text = "0";

            tradeSlotIndex = -1;
            tradeSlotItemData = null;
            tradeInventoryTypeFrom = EInventoryType.None;
            tradeInventoryTypeTo = EInventoryType.None;

            gameObject.SetActive(false);
        }

        private void OnAmountIncreaseButtonClicked()
        {
            if (decidedAmount >= AmountDecisionSlider.maxValue)
            {
                return;
            }

            AmountDecisionSlider.value++;
            decidedAmount = (int)AmountDecisionSlider.value;

            amountInputField.text = decidedAmount.ToString();
            totalPriceInfoText.text = (decidedAmount * ItemPriceAPiece).ToString();
        }

        private void OnAmountDecreaseButtonClicked()
        {
            if (decidedAmount <= 0) 
            {
                return;
            }

            AmountDecisionSlider.value--;
            decidedAmount = (int)AmountDecisionSlider.value;

            amountInputField.text = decidedAmount.ToString();
            totalPriceInfoText.text = (decidedAmount * ItemPriceAPiece).ToString();
        }
    }
}
