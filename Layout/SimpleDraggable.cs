using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Layout
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(ResizableBoxCollider2D))]
    public class SimpleDraggable : DraggableBase
    {
        [SerializeField]
        UnityEvent onDragEvent;

        [SerializeField]
        UnityEvent onEndDragEvent;

        public override void OnDrag(PointerEventData data)
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
            onDragEvent.Invoke();
        }

        public override void OnEndDrag(PointerEventData data)
        {
            onEndDragEvent.Invoke();
        }

        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }
}