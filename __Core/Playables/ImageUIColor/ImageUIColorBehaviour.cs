using System;
using UnityEngine;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace AltSalt
{
    [Serializable]
    public class ImageUIColorBehaviour : LerpToTargetBehaviour
    {
        public Color initialColor = new Color(0,0,0,0);
        public Color targetColor = new Color(0,0,0,1);

#if UNITY_EDITOR
        [HorizontalGroup("Row 1", 0.5f)]
        [Button(ButtonSizes.Large), GUIColor(0.7f, 0.4f, 0.7f, 0.9f)]
        public void InitialTransparent()
        {
            initialColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0);
        }

        [HorizontalGroup("Row 1", 0.5f)]
        [Button(ButtonSizes.Large), GUIColor(0.7f, 0.4f, 0.7f)]
        public void InitialOpaque()
        {
            initialColor = new Color(initialColor.r, initialColor.g, initialColor.b, 1);
        }

        [HorizontalGroup("Row 2", 0.5f)]
        [Button(ButtonSizes.Large), GUIColor(0.7f, 0.4f, 0.7f, 0.9f)]
        public void TargetTransparent()
        {
            targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0);
        }

        [HorizontalGroup("Row 2", 0.5f)]
        [Button(ButtonSizes.Large), GUIColor(0.7f, 0.4f, 0.7f)]
        public void TargetOpaque()
        {
            targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 1);
        }

        [HorizontalGroup("Row 3", 0.5f)]
        [Button(ButtonSizes.Large), GUIColor(0.65f, 0.65f, 0.65f)]
        public void InitialBlack()
        {
            initialColor = new Color(0, 0, 0, initialColor.a);
        }

        [HorizontalGroup("Row 3", 0.5f)]
        [Button(ButtonSizes.Large), GUIColor(0.65f, 0.65f, 0.65f)]
        public void TargetBlack()
        {
            targetColor = new Color(0, 0, 0, targetColor.a);
        }

        [HorizontalGroup("Row 4", 0.5f)]
        [Button(ButtonSizes.Large), GUIColor(1f, 1f, 1f)]
        public void InitialWhite()
        {
            initialColor = new Color(1, 1, 1, initialColor.a);
        }


        [HorizontalGroup("Row 4", 0.5f)]
        [Button(ButtonSizes.Large), GUIColor(1f, 1f, 1f)]
        public void TargetWhite()
        {
            targetColor = new Color(1, 1, 1, targetColor.a);
        }
#endif
    }
}