using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace CommonRPG
{
    public class InventorySlotUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IDragAndDropEvent
    {
        private enum EInvntorySlotElementOrder
        {
            SlotImage = 0,
            ItemCountText
        }

        public int SlotIndex { get; set; }

        private bool isEmpty = true;
        public bool IsEmpty { get { return isEmpty; } set { isEmpty = value; } }

        public EInventoryType CurrentSlotInventoryType { get; set; }

        [SerializeField]
        private EItemType allowedItemType = EItemType.None;
        public EItemType AllowedItemType
        {
            get
            {
                return allowedItemType;
            }

            set
            {
                allowedItemType = value;
            }
        }
        /// <summary>
        /// args : int slotIndex, EInventoryType thisInventoryType
        /// </summary>
        public event Action<int, EInventoryType> OnLeftMouseDownDelegate = null;
        /// <summary>
        /// args : int slotIndex, EInventoryType thisInventoryType
        /// </summary>
        public event Action<int, EInventoryType> OnRightMouseDownDelegate = null;

        /// <summary>
        /// args : int slotIndex, EInventoryType thisInventoryType
        /// </summary>
        public event Action<int, EInventoryType> OnLeftMouseDoubleClickDelegate = null;
        /// <summary>
        /// args : int slotIndex, EInventoryType thisInventoryType Vector2 slotPosition, Vector2 slotWidthAndHeight
        /// </summary>
        public event Action<int, EInventoryType, Vector2, Vector2> OnPointerEnterDelegate = null;
        /// <summary>
        /// args : int slotIndex
        /// </summary>
        public event Action<int> OnPointerExitDelegate = null;
        /// <summary>
        /// arg : int slotIndex
        /// </summary>
        public event Action<int> OnBeginDragDelegate = null;
        /// <summary>
        /// args : int thisSlotIndex, int otherSlotIndex, EInventoryType thisInventoryType, EInventoryType otherInventoryType
        /// if otherSlotIndex == -1, it means item is dropped at inventory background
        /// </summary>
        public event Action<int, int, EInventoryType, EInventoryType> OnEndDragDelegate = null;

        [SerializeField]
        private UnityEngine.UI.Image slotImage = null;
        private UnityEngine.UI.Image dragSlotImage = null;

        [SerializeField]
        private TextMeshProUGUI itemCountText = null;

        private RectTransform rectTransform = null;
        private Vector2 widthAndHeight = Vector2.zero;

        private float doubleClickTime = 0.5f;
        private float currentDoubleClickTime = 0;
        private void Awake()
        {
            //slotImage = transform.GetChild((int)EInvntorySlotElementOrder.SlotImage).GetComponent<UnityEngine.UI.Image>();
            Debug.Assert(slotImage);

            GameManager.GetDragSlotImage(out dragSlotImage);

            Debug.Assert(dragSlotImage);
            //dragSlotImage.gameObject.SetActive(false);

            //itemCountText = transform.GetChild((int)EInvntorySlotElementOrder.ItemCountText).GetComponent<TextMeshProUGUI>();
            Debug.Assert(itemCountText);

            //IsEmpty = true;
            itemCountText.gameObject.SetActive(IsEmpty == false);

            rectTransform = GetComponent<RectTransform>();
            widthAndHeight = new Vector2(rectTransform.rect.width, rectTransform.rect.height);
        }

        private void Update()
        {
            if (currentDoubleClickTime > 0) 
            {
                currentDoubleClickTime -= Time.unscaledDeltaTime;
            }
        }

        public void SetSlotImageSprite(Sprite newSprite)
        {
            slotImage.sprite = newSprite;
        }

        public void SetSlotItemCountText(int newItemCount)
        {
            IsEmpty = (newItemCount == 0);
            itemCountText.gameObject.SetActive(IsEmpty == false);
            SetSlotItemCountText(newItemCount.ToString());
        }

        public void SetSlotItemCountText(string newText)
        {
            itemCountText.text = newText;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (currentDoubleClickTime <= 0) 
            {
                currentDoubleClickTime = doubleClickTime;
            }

            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (currentDoubleClickTime == doubleClickTime) 
                {
                    if (OnLeftMouseDownDelegate == null)
                    {
                        return;
                    }
                    
                    OnLeftMouseDownDelegate.Invoke(SlotIndex, CurrentSlotInventoryType);
                }
                else
                {
                    if (OnLeftMouseDoubleClickDelegate == null) 
                    {
                        return;
                    }

                    currentDoubleClickTime = 0;
                    OnLeftMouseDoubleClickDelegate.Invoke(SlotIndex, CurrentSlotInventoryType);
                }
                
            }
            else if (eventData.button == PointerEventData.InputButton.Right) 
            {
                if (OnRightMouseDownDelegate == null) 
                {
                    return;
                }

                OnRightMouseDownDelegate.Invoke(SlotIndex, CurrentSlotInventoryType);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            // Debug.Log($"OnPointerUp, SlotIndex : {SlotIndex}");
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //Debug.Log($"OnPointerEnter, SlotIndex : {SlotIndex}");

            OnPointerEnterDelegate.Invoke(SlotIndex, CurrentSlotInventoryType, rectTransform.position, widthAndHeight);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //Debug.Log($"OnPointerExit, SlotIndex : {SlotIndex}");

            OnPointerExitDelegate.Invoke(SlotIndex);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
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

            if (OnEndDragDelegate == null)
            {
                return;
            }

            //Debug.Log($"OnEndDrag, SlotIndex : {SlotIndex}");
            //Debug.Log($"OnEndDrag, Drag : {eventData.pointerDrag}");

            if (eventData.pointerCurrentRaycast.isValid == false)
            {
                return;
            }

            //Debug.Log($"OnEndDrag, Current Raycast : {eventData.pointerCurrentRaycast}");

            GameObject hitObject = eventData.pointerCurrentRaycast.gameObject;
            InventorySlotUI otherSlotUI = hitObject.transform.parent.GetComponent<InventorySlotUI>();

            int otherIndex;
            EInventoryType inventoryType;

            if (otherSlotUI == null)
            {
                InventoryBackground inventoryBackground = hitObject.GetComponent<InventoryBackground>();
                if (inventoryBackground && inventoryBackground.CurrentInventoryType != CurrentSlotInventoryType) 
                {
                    otherIndex = -1;
                    inventoryType = inventoryBackground.CurrentInventoryType;
                }
                else
                {
                    return;
                }
            }
            else
            {
                otherIndex = otherSlotUI.SlotIndex;
                inventoryType = otherSlotUI.CurrentSlotInventoryType;
            }

            if (OnEndDragDelegate == null) 
            {
                Debug.Log("OnEndDragDelegate == null");
                return;
            }

            //Debug.Log($"firstSlotIndex : {SlotIndex}, secondSlotIndex : {otherSlotUI.SlotIndex}, firstSlotInventoryType : {CurrentSlotInventoryType}, secondSlotInventoryType : {otherSlotUI.CurrentSlotInventoryType}");
            OnEndDragDelegate.Invoke(SlotIndex, otherIndex, CurrentSlotInventoryType, inventoryType);
        }
    }

}
