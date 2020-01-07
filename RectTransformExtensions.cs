using UnityEngine;

namespace AltSalt.Maestro.Layout
{
    public class RectTransformExtensions : InteractableRectTransform
    {
        [SerializeField]
        [TextArea]
        string description;

        public void SetPosition(RectTransform targetPosition)
        {
            rectTransform.position = targetPosition.position;
        }
        
        public void SetPosition(V3Variable targetPosition)
        {
            rectTransform.position = targetPosition.value;
        }
    }
}