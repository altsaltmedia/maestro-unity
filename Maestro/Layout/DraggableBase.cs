using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;

namespace AltSalt.Maestro.Layout
{
    [RequireComponent(typeof(ResizableBoxCollider2D))]
    public abstract class DraggableBase : InteractableRectTransform, IDragHandler, IEndDragHandler
    {
        protected BoxCollider2D boxCollider;

        [SerializeField]
        [ValidateInput("IsPopulated")]
        protected FloatReference dragSensitivity;

        [SerializeField]
        protected bool horizontalDrag;

        [SerializeField]
        protected bool verticalDrag;

        public abstract void OnDrag(PointerEventData data);

        public abstract void OnEndDrag(PointerEventData data);

        protected static Vector2 GetDragModifier(bool xEnabled, bool yEnabled, float dragSensitivity, PointerEventData data)
        {
            float xModifier = 0f;
            float yModifier = 0f;

            if (xEnabled == true) {
                xModifier = dragSensitivity * data.delta.x;
            }

            if (yEnabled == true) {
                yModifier = dragSensitivity * data.delta.y;
            }

            return new Vector2(xModifier, yModifier);
        }

        protected static Vector2 GetNewPosition(RectTransform sourceTransform, Vector2 modifier)
        {
            float xPosition = sourceTransform.anchoredPosition.x;
            float yPosition = sourceTransform.anchoredPosition.y;

            return new Vector2(xPosition + modifier.x, yPosition + modifier.y);
        }

        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}
