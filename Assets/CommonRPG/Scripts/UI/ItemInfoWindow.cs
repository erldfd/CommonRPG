using CommonRPG;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemInfoWindow : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI itemTypeText = null;

    [SerializeField]
    private TextMeshProUGUI itemNameText = null;

    [SerializeField]
    private TextMeshProUGUI itemGradeText = null;

    [SerializeField]
    private TextMeshProUGUI itemDamageText = null;

    [SerializeField]
    private TextMeshProUGUI itemArmorText = null;

    [SerializeField]
    private TextMeshProUGUI itemHpBonusText = null;

    [SerializeField]
    private TextMeshProUGUI itemMpBonusText = null;

    [SerializeField]
    private TextMeshProUGUI itemBuyPriceText = null;

    [SerializeField]
    private TextMeshProUGUI itemSellPriceText = null;

    [SerializeField]
    private TextMeshProUGUI itemDiscriptionText = null;

    [SerializeField]
    private RectTransform rectTransform = null;

    private void Awake()
    {
        gameObject.SetActive(true);
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetItemInfoData(in SItemData data)
    {
        itemTypeText.text = data.ItemType.ToString();
        itemNameText.text = data.ItemName.ToString();
        itemGradeText.text = data.ItemGrade.ToString();
        itemDamageText.text = data.Damage.ToString();
        itemArmorText.text = data.Defense.ToString();
        itemHpBonusText.text = data.HPBonus.ToString();
        itemMpBonusText.text = data.MPBonus.ToString();
        itemBuyPriceText.text = data.BuyPrice.ToString();
        itemSellPriceText.text = data.SellPrice.ToString();
        itemDiscriptionText.text = data.ItemDiscription;
    }

    public void SetToProperPosition(Vector2 slotPos, Vector2 slotWidthAndHeight)
    {
        Resolution currentResolution = Screen.currentResolution;
        float rightMargin = currentResolution.width - (slotPos.x + slotWidthAndHeight.x / 2);

        float itemInfoWindowPosX;

        if (rightMargin > rectTransform.rect.width) 
        {
            itemInfoWindowPosX = slotPos.x + slotWidthAndHeight.x / 2 + rectTransform.rect.width / 2;
        }
        else
        {
            itemInfoWindowPosX = slotPos.x - slotWidthAndHeight.x / 2 - rectTransform.rect.width / 2;
        }

        float itemInfoWindowPosY;

        if (slotPos.y + rectTransform.rect.height / 2 > currentResolution.height)
        {
            itemInfoWindowPosY = currentResolution.height - rectTransform.rect.height / 2;
        }
        else
        {
            itemInfoWindowPosY = slotPos.y;
        }

        rectTransform.position = new Vector3(itemInfoWindowPosX, itemInfoWindowPosY);
    }

    public void ShowOrHide(bool shouldShow)
    {
        gameObject.SetActive(shouldShow);
    }
}
