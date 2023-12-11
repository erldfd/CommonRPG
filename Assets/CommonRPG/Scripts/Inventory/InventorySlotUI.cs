using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    public int SlotIndex { get; set; }

    private Image slotImage = null;
    private TextMeshProUGUI itemCountText = null;
    

    private void Awake()
    {
        slotImage = GetComponent<Image>();
        Debug.Assert(slotImage);

        itemCountText = GetComponentInChildren<TextMeshProUGUI>();
        Debug.Assert(itemCountText);
    }

    public void SetSlotImageSprite(Sprite newSprite)
    {
        slotImage.sprite = newSprite;
    }

    public void SetSlotItemCountText(int newItemCount)
    {
        itemCountText.text = newItemCount.ToString();
    }
}
