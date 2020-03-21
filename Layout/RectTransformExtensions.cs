using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro.Layout
{
    public class RectTransformExtensions : InteractableRectTransform
    {
        [SerializeField]
        [TextArea]
        string description;

        [Button(ButtonSizes.Large)]
        public void SetPosition(RectTransform targetPosition)
        {
            rectTransform.position = targetPosition.position;
        }
        
        [Button(ButtonSizes.Large)]
        public void SetPosition(V3Variable targetPosition)
        {
            rectTransform.position = targetPosition.value;
        }
        
        [Button(ButtonSizes.Large)]
        public void SetRotation(RectTransform targetValue)
        {
            rectTransform.localEulerAngles = targetValue.localEulerAngles;
        }
        
        [Button(ButtonSizes.Large)]
        public void SetRotation(V3Variable targetValue)
        {
            rectTransform.localEulerAngles = targetValue.value;
        }
    }
}