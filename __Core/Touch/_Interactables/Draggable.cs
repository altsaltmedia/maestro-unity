using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(ResizableBoxCollider2D))]
    public class Draggable : InteractableRectTransform, IDragHandler
    {
        BoxCollider2D boxCollider;

        [SerializeField]
        [ValidateInput("IsPopulated")]
        FloatReference dragSensitivity;

        [SerializeField]
        bool horizontalDrag;

        [SerializeField]
        bool verticalDrag;

        public void OnDrag(PointerEventData data)
        {
            float newXPosition = 0f;
            float newYPosition = 0f;

            if(horizontalDrag == true) {
                newXPosition = rectTransform.anchoredPosition.x + (data.delta.x * dragSensitivity.Value);
            } else {
                newXPosition = rectTransform.anchoredPosition.x;
            }

            if(verticalDrag == true) {
                newYPosition = rectTransform.anchoredPosition.y + (data.delta.y * dragSensitivity.Value);
            } else {
                newYPosition = rectTransform.anchoredPosition.y;
            }

            Vector2 newPosition = new Vector2(newXPosition, newYPosition);
            rectTransform.anchoredPosition = newPosition;
        }

        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }
}