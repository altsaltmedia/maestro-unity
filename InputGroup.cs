using System;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro
{
    [Serializable]
    public class InputGroup
    {
        
        // Swipe Monitor

        [SerializeField]
        [FoldoutGroup("Calculations")]
        private V2Reference _swipeForce = new V2Reference();

        public V2Reference swipeForce
        {
            get => _swipeForce;
            set => _swipeForce = value;
        }
        
        [SerializeField]
        [FoldoutGroup("Calculations")]
        private StringReference _swipeDirection = new StringReference();

        public StringReference swipeDirection
        {
            get => _swipeDirection;
            set => _swipeDirection = value;
        }

        [SerializeField]
        [FoldoutGroup("Configuration")]
        private FloatReference _swipeMinMax = new FloatReference();
        
        public FloatReference swipeMinMax
        {
            get => _swipeMinMax;
            set => _swipeMinMax = value;
        }
        
        [SerializeField]
        [FoldoutGroup("Configuration")]
        private FloatReference _gestureTimeMultiplier = new FloatReference();

        public FloatReference gestureTimeMultiplier
        {
            get => _gestureTimeMultiplier;
            set => _gestureTimeMultiplier = value;
        }
        
        [SerializeField]
        [FoldoutGroup("Configuration")]
        private FloatReference _cancelMomentumTimeThreshold = new FloatReference();

        public FloatReference cancelMomentumTimeThreshold
        {
            get => _cancelMomentumTimeThreshold;
            set => _cancelMomentumTimeThreshold = value;
        }
        
        [SerializeField]
        [FoldoutGroup("Configuration")]
        private FloatReference _cancelMomentumMagnitudeThreshold = new FloatReference();

        public FloatReference cancelMomentumMagnitudeThreshold
        {
            get => _cancelMomentumMagnitudeThreshold;
            set => _cancelMomentumMagnitudeThreshold = value;
        }
        
        [SerializeField]
        [FoldoutGroup("Configuration")]
        private FloatReference _pauseMomentumTimeThreshold = new FloatReference();

        public FloatReference pauseMomentumTimeThreshold
        {
            get => _pauseMomentumTimeThreshold;
            set => _pauseMomentumTimeThreshold = value;
        }
        
        [Required]
        [FoldoutGroup("Touch Events")]
        [SerializeField]
        private SimpleEventTrigger _onTouchStart = new SimpleEventTrigger();

        public SimpleEventTrigger onTouchStart => _onTouchStart;

        [Required]
        [FoldoutGroup("Touch Events")]
        [SerializeField]
        private SimpleEventTrigger _onLongTouch = new SimpleEventTrigger();

        public SimpleEventTrigger onLongTouch => _onLongTouch;

        [Required]
        [FoldoutGroup("Touch Events")]
        [SerializeField]
        private SimpleEventTrigger _onSwipe = new SimpleEventTrigger();

        public SimpleEventTrigger onSwipe => _onSwipe;

        [Required]
        [FoldoutGroup("Touch Events")]
        [SerializeField]
        private SimpleEventTrigger _onSwipeEnd = new SimpleEventTrigger();

        public SimpleEventTrigger onSwipeEnd => _onSwipeEnd;
        
        
        
        // Sequencing
        
        [SerializeField]
        [FoldoutGroup("Axes")]
        private AxisReference _ySwipeAxis = new AxisReference();

        public AxisReference ySwipeAxis
        {
            get => _ySwipeAxis;
            set => _ySwipeAxis = value;
        }
        
        [SerializeField]
        [FoldoutGroup("Axes")]
        private AxisReference _yMomentumAxis = new AxisReference();

        public AxisReference yMomentumAxis
        {
            get => _yMomentumAxis;
            set => _yMomentumAxis = value;
        }

        [SerializeField]
        [FoldoutGroup("Axes")]
        private AxisReference _xSwipeAxis = new AxisReference();

        public AxisReference xSwipeAxis
        {
            get => _xSwipeAxis;
            set => _xSwipeAxis = value;
        }

        [SerializeField]
        [FoldoutGroup("Axes")]
        private AxisReference _xMomentumAxis = new AxisReference();

        public AxisReference xMomentumAxis
        {
            get => _xMomentumAxis;
            set => _xMomentumAxis = value;
        }
        
        [SerializeField]
        [FoldoutGroup("Calculations")]
        private BoolReference _isReversing = new BoolReference();

        public BoolReference isReversing
        {
            get => _isReversing;
            set => _isReversing = value;
        }
        
        [SerializeField]
        [FoldoutGroup("Calculations")]
        private FloatReference _swipeModifierOutput = new FloatReference();
        
        public FloatReference swipeModifierOutput
        {
            get => _swipeModifierOutput;
            set => _swipeModifierOutput = value;
        }
        
        [SerializeField]
        [FoldoutGroup("Calculations")]
        private FloatReference _momentumModifierOutput = new FloatReference();

        public FloatReference momentumModifierOutput
        {
            get => _momentumModifierOutput;
            set => _momentumModifierOutput = value;
        }

  
        
        [SerializeField, Required]
        public FloatReference _frameStepValue;
        
        public FloatReference frameStepValue
        {
            get
            {
                UpdateDependencies();
                return _frameStepValue;
            }
        }

        public InputGroup(CustomKey inputGroupKey)
        {
#if UNITY_EDITOR
            FieldInfo[] fields = typeof(InputGroup).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

            for (int i = 0; i < fields.Length; i++) {
                
                var varReference = fields[i].GetValue(this);
                string name = fields[i].Name.Replace("_", "").Capitalize();
                
                var variableField = varReference.GetType().GetField("_variable", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                variableField.SetValue(varReference, CreateInputDependency(variableField.FieldType, $"{inputGroupKey.name}-{name}", inputGroupKey.name));
                
            }
#endif
        }
        
#if UNITY_EDITOR
        private static dynamic CreateInputDependency(Type assetType, string name, string groupName)
        {
            return Utils.CreateScriptableObjectAsset(assetType, name, $"{Utils.settingsPath}/InputSettings/{groupName}");
        }
#endif
    }
}