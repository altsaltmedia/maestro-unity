using System;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace AltSalt.Maestro.Animation
{
    [Serializable]
    public class LerpColorVarBehaviour : LerpToTargetBehaviour
    {
        [FormerlySerializedAs("initialColor")]
        [SerializeField]
        private Color _initialValue;

        public Color initialValue
        {
            get => _initialValue;
            set => _initialValue = value;
        }

        [FormerlySerializedAs("targetColor")]
        [SerializeField]
        private Color _targetValue;

        public Color targetValue
        {
            get => _targetValue;
            set => _targetValue = value;
        }

#if UNITY_EDITOR
        
        public override object SetInitialValueToTarget()
        {
            initialValue = targetValue;
            return initialValue;
        }
        
        [HorizontalGroup("Row 1", 0.5f)]
        [Button(ButtonSizes.Large), GUIColor(0.7f, 0.4f, 0.7f, 0.9f)]
        public void InitialTransparent()
        {
            initialValue = new Color(initialValue.r, initialValue.g, initialValue.b, 0);
        }

        [HorizontalGroup("Row 1", 0.5f)]
        [Button(ButtonSizes.Large), GUIColor(0.7f, 0.4f, 0.7f)]
        public void InitialOpaque()
        {
            initialValue = new Color(initialValue.r, initialValue.g, initialValue.b, 1);
        }

        [HorizontalGroup("Row 2", 0.5f)]
        [Button(ButtonSizes.Large), GUIColor(0.7f, 0.4f, 0.7f, 0.9f)]
        public void TargetTransparent()
        {
            targetValue = new Color(initialValue.r, initialValue.g, initialValue.b, 0);
        }

        [HorizontalGroup("Row 2", 0.5f)]
        [Button(ButtonSizes.Large), GUIColor(0.7f, 0.4f, 0.7f)]
        public void TargetOpaque()
        {
            targetValue = new Color(initialValue.r, initialValue.g, initialValue.b, 1);
        }

        [HorizontalGroup("Row 3", 0.5f)]
        [Button(ButtonSizes.Large), GUIColor(0.65f, 0.65f, 0.65f)]
        public void InitialBlack()
        {
            initialValue = new Color(0, 0, 0, initialValue.a);
        }

        [HorizontalGroup("Row 3", 0.5f)]
        [Button(ButtonSizes.Large), GUIColor(0.65f, 0.65f, 0.65f)]
        public void TargetBlack()
        {
            targetValue = new Color(0, 0, 0, initialValue.a);
        }

        [HorizontalGroup("Row 4", 0.5f)]
        [Button(ButtonSizes.Large), GUIColor(1f, 1f, 1f)]
        public void InitialWhite()
        {
            initialValue = new Color(1, 1, 1, initialValue.a);
        }


        [HorizontalGroup("Row 4", 0.5f)]
        [Button(ButtonSizes.Large), GUIColor(1f, 1f, 1f)]
        public void TargetWhite()
        {
            targetValue = new Color(1, 1, 1, initialValue.a);
        }
#endif
    }
}