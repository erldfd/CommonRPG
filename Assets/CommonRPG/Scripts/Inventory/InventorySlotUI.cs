using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class InventorySlotUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IDragAndDropEvent
{
    private enum EInvntorySlotElementOrder
    {
        SlotImage = 0,
        ItemCountText
    }

    public int SlotIndex { get; set; }
    public bool IsEmpty { get; set; }

    public event Action<int> OnPointerDownDelegate = null;
    public event Action<int> OnBeginDragDelegate = null;
    public event Action<int> OnEndDragDelegate = null;

    private UnityEngine.UI.Image slotImage = null;
    private UnityEngine.UI.Image dragSlotImage = null;
    private TextMeshProUGUI itemCountText = null;
    
    private void Awake()
    {
        slotImage = transform.GetChild((int)EInvntorySlotElementOrder.SlotImage).GetComponent<UnityEngine.UI.Image>();
        Debug.Assert(slotImage);

        dragSlotImage = transform.root.GetChild(1).GetChild(1).GetComponent<UnityEngine.UI.Image>();
        Debug.Assert(dragSlotImage);
        dragSlotImage.gameObject.SetActive(false);

        itemCountText = transform.GetChild((int)EInvntorySlotElementOrder.ItemCountText).GetComponent<TextMeshProUGUI>();
        Debug.Assert(itemCountText);

        IsEmpty = true;
        itemCountText.gameObject.SetActive(IsEmpty == false);
    }

    public void SetSlotImageSprite(Sprite newSprite)
    {
        slotImage.sprite = newSprite;
        //dragSlotImage.sprite = newSprite;
        //dragImageSpriterenderer.sprite = newSprite;
    }

    public void SetSlotItemCountText(int newItemCount)
    {
        itemCountText.text = newItemCount.ToString();
        IsEmpty = (newItemCount == 0);
        itemCountText.gameObject.SetActive(IsEmpty == false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Debug.Log($"OnPointerDown, SlotIndex : {SlotIndex}");
        if (OnPointerDownDelegate == null)
        {
            return;
        }

        OnPointerDownDelegate.Invoke(SlotIndex);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
       // Debug.Log($"OnPointerUp, SlotIndex : {SlotIndex}");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log($"OnPointerEnter, SlotIndex : {SlotIndex}");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log($"OnPointerExit, SlotIndex : {SlotIndex}");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log($"OnBeginDrag, SlotIndex : {SlotIndex}");

        if (IsEmpty) 
        {
            return;
        }

        if (eventData.button != PointerEventData.InputButton.Left) 
        {
            return;
        }

        dragSlotImage.gameObject.SetActive(true);
        dragSlotImage.transform.position = eventData.position;
        dragSlotImage.sprite = slotImage.sprite;
        dragSlotImage.rectTransform.rect.Set(slotImage.rectTransform.rect.x, slotImage.rectTransform.rect.y, slotImage.rectTransform.rect.width, slotImage.rectTransform.rect.height);

        if (OnBeginDragDelegate == null)
        {
            return;
        }

        OnBeginDragDelegate.Invoke(SlotIndex);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (IsEmpty)
        {
            return;
        }

        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        dragSlotImage.transform.position = eventData.position;

        Debug.Log($"OnDrag, SlotIndex : {SlotIndex}");
        Debug.Log($"Current Raycast : {eventData.pointerCurrentRaycast}");
        Debug.Log($"Drag : {eventData.pointerDrag}");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsEmpty)
        {
            return;
        }

        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        dragSlotImage.gameObject.SetActive(false);
        dragSlotImage.transform.position = eventData.position;

        //Debug.Log($"OnEndDrag, SlotIndex : {SlotIndex}");

        if (OnEndDragDelegate == null)
        {
            return;
        }

        OnEndDragDelegate.Invoke(SlotIndex);
    }
}
