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
    }
}