using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CommonRPG
{
    public class DraggableMargin : MonoBehaviour, IPointerMoveHandler, IPointerUpHandler, IPointerDownHandler
    {
        [SerializeField]
        private RectTransform parentRectTransform = null;

        private bool isDragging = false;

        public void OnPointerDown(PointerEventData eventData)
        {
            isDragging = true;
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (isDragging == false || parentRectTransform == null) 
            {
                return;
            }

            parentRectTransform.anchoredPosition += eventData.delta;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isDragging = false;
        }
    }
}
