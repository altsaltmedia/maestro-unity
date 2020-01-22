using System;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace AltSalt.Maestro
{
    [Serializable]
    public class InputGroup
    {

    #region Calculated Values
    
        [SerializeField]
        [FoldoutGroup("Configuration")]
        private BoolReference _isSwiping = new BoolReference();
        
        public BoolReference isSwiping => _isSwiping;
        
                
        [SerializeField]
        [FoldoutGroup("Configuration")]
        private BoolReference _isFlicked = new BoolReference();
        
        public BoolReference isFlicked => _isFlicked;
        
        
        [SerializeField]
        [FoldoutGroup("Calculations")]
        private V2Reference _touchStartPosition = new V2Reference();

        public V2Reference touchStartPosition => _touchStartPosition;
        
        
        [SerializeField]
        [FoldoutGroup("Calculations")]
        private V2Reference _swipeForce = new V2Reference();

        public V2Reference swipeForce => _swipeForce;
        
        
        [SerializeField]
        [FoldoutGroup("Calculations")]
        private V2Reference _swipeMonitorMomentum = new V2Reference();

        public V2Reference swipeMonitorMomentum => _swipeMonitorMomentum;
        
        
        [SerializeField]
        [FoldoutGroup("Calculations")]
        private V2Reference _swipeMonitorMomentumCache = new V2Reference();

        public V2Reference swipeMonitorMomentumCache => _swipeMonitorMomentumCache;
        
        
        [SerializeField]
        [FoldoutGroup("Calculations")]
        private StringReference _swipeDirection = new StringReference();

        public StringReference swipeDirection => _swipeDirection;
        
        
        [SerializeField]
        [FoldoutGroup("Calculations")]
        private BoolReference _isReversing = new BoolReference();

        public BoolReference isReversing => _isReversing;

        
        [SerializeField]
        [FoldoutGroup("Calculations")]
        private FloatReference _swipeModifierOutput = new FloatReference();
        
        public FloatReference swipeModifierOutput => _swipeModifierOutput;

        
        [SerializeField]
        [FoldoutGroup("Calculations")]
        private FloatReference _momentumModifierOutput = new FloatReference();

        public FloatReference momentumModifierOutput => _momentumModifierOutput;
        
        
        [SerializeField]
        [FoldoutGroup("Calculations")]
        private BoolReference _axisTransitionActive = new BoolReference();

        public BoolReference axisTransitionActive => _axisTransitionActive;
        
        
        [SerializeField]
        [FoldoutGroup("Calculations")]
        private BoolReference _forkTransitionActive = new BoolReference();

        public BoolReference forkTransitionActive => _forkTransitionActive;
        
        
        [SerializeField]
        [FoldoutGroup("Calculations")]
        private BoolReference _scrubberActive = new BoolReference();

        public BoolReference scrubberActive => _scrubberActive;

    #endregion

    
    #region Events
    
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
            
        [Required]
        [FoldoutGroup("Touch Events")]
        [SerializeField]
        private SimpleEventTrigger _momentumUpdate = new SimpleEventTrigger();

        public SimpleEventTrigger momentumUpdate => _momentumUpdate;
        
        [Required]
        [FoldoutGroup("Touch Events")]
        [SerializeField]
        private SimpleEventTrigger _momentumAppliedToSequences = new SimpleEventTrigger();

        public SimpleEventTrigger momentumAppliedToSequences => _momentumAppliedToSequences;
        
        [Required]
        [FoldoutGroup("Touch Events")]
        [SerializeField]
        private SimpleEventTrigger _momentumDepleted = new SimpleEventTrigger();

        public SimpleEventTrigger momentumDepleted => _momentumDepleted;
        
        [Required]
        [FoldoutGroup("Touch Events")]
        [SerializeField]
        private SimpleEventTrigger _boundaryReached = new SimpleEventTrigger();

        public SimpleEventTrigger boundaryReached => _boundaryReached;
        
        [Required]
        [FoldoutGroup("Touch Events")]
        [SerializeField]
        private ComplexEventManualTrigger _sequenceModified = new ComplexEventManualTrigger();

        public ComplexEventManualTrigger sequenceModified => _sequenceModified;
        
    #endregion


    #region Configuration

        [SerializeField]
        [FoldoutGroup("Configuration")]
        private FloatReference _swipeMinMax = new FloatReference();
        
        public FloatReference swipeMinMax => _swipeMinMax;
        
        
        [SerializeField]
        [FoldoutGroup("Configuration")]
        private FloatReference _momentumMinMax = new FloatReference();
        
        public FloatReference momentumMinMax => _momentumMinMax;
        
        
        [SerializeField]
        [FoldoutGroup("Configuration")]
        private FloatReference _momentumDecay = new FloatReference();
        
        public FloatReference momentumDecay => _momentumDecay;
        
        
        [SerializeField]
        [FoldoutGroup("Configuration")]
        private FloatReference _momentumSensitivity = new FloatReference();
        
        public FloatReference momentumSensitivity => _momentumSensitivity;

        
        [SerializeField]
        [FoldoutGroup("Configuration")]
        private FloatReference _gestureTimeMultiplier = new FloatReference();

        public FloatReference gestureTimeMultiplier => _gestureTimeMultiplier;
        

        [SerializeField]
        [FoldoutGroup("Configuration")]
        private FloatReference _cancelMomentumTimeThreshold = new FloatReference();

        public FloatReference cancelMomentumTimeThreshold => _cancelMomentumTimeThreshold;
        
        
        [SerializeField]
        [FoldoutGroup("Configuration")]
        private FloatReference _cancelMomentumMagnitudeThreshold = new FloatReference();

        public FloatReference cancelMomentumMagnitudeThreshold => _cancelMomentumMagnitudeThreshold;

        
        [SerializeField]
        [FoldoutGroup("Configuration")]
        private FloatReference _pauseMomentumThreshold = new FloatReference();

        public FloatReference pauseMomentumThreshold => _pauseMomentumThreshold;
        
        
        [SerializeField]
        [FoldoutGroup("Configuration")]
        private FloatReference _flickThreshold = new FloatReference();
        
        public FloatReference flickThreshold => _flickThreshold;
        
        
        [SerializeField, Required]
        private FloatReference _axisTransitionSpread = new FloatReference();
        
        public FloatReference axisTransitionSpread => _axisTransitionSpread;
        
        
        [SerializeField, Required]
        private FloatReference _forkTransitionSpread = new FloatReference();
        
        public FloatReference forkTransitionSpread => _forkTransitionSpread;

        
        [SerializeField, Required]
        private FloatReference _frameStepValue = new FloatReference();
        
        public FloatReference frameStepValue => _frameStepValue;

    #endregion


    #region Axes

        [SerializeField]
        [FoldoutGroup("Axes")]
        private AxisReference _ySwipeAxis = new AxisReference();

        public AxisReference ySwipeAxis => _ySwipeAxis;

        [SerializeField]
        [FoldoutGroup("Axes")]
        private AxisReference _yMomentumAxis = new AxisReference();

        public AxisReference yMomentumAxis => _yMomentumAxis;

        [SerializeField]
        [FoldoutGroup("Axes")]
        private AxisReference _xSwipeAxis = new AxisReference();

        public AxisReference xSwipeAxis => _xSwipeAxis;

        [SerializeField]
        [FoldoutGroup("Axes")]
        private AxisReference _xMomentumAxis = new AxisReference();

        public AxisReference xMomentumAxis => _xMomentumAxis;

    #endregion
        

        public InputGroup(InputData inputData, InputGroupKey inputGroupKey)
        {
#if UNITY_EDITOR
            RefreshDependencies(inputGroupKey);
            SetDefaults(inputData, inputGroupKey);
#endif
        }

        public void RefreshDependencies(InputGroupKey inputGroupKey)
        {
            FieldInfo[] referenceFields = typeof(InputGroup).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

            for (int i = 0; i < referenceFields.Length; i++) {
                
                string name = referenceFields[i].Name.Replace("_", "").Capitalize();
                var variableField = Utils.GetVariableFieldFromReference(referenceFields[i], this, out var referenceValue);
                var variableValue = variableField.GetValue(referenceValue) as ScriptableObject;

                if (variableValue == null) {
                    variableField.SetValue(referenceValue, CreateInputDependency(variableField.FieldType, $"{inputGroupKey.name}-{name}", inputGroupKey.name));
                }
            }
        }
        
        public InputGroup SetDefaults(InputData inputData, InputGroupKey inputGroupKey)
        {
            FieldInfo[] fields = typeof(InputGroup).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            
            swipeMinMax.GetVariable(inputData).defaultValue = 80f;
            momentumMinMax.GetVariable(inputData).defaultValue = 2000f;
            momentumDecay.GetVariable(inputData).defaultValue = .95f;
            momentumSensitivity.GetVariable(inputData).defaultValue = 1f;
            gestureTimeMultiplier.GetVariable(inputData).defaultValue = 50f;
            cancelMomentumTimeThreshold.GetVariable(inputData).defaultValue = .2f;
            cancelMomentumMagnitudeThreshold.GetVariable(inputData).defaultValue = 815f;
            pauseMomentumThreshold.GetVariable(inputData).defaultValue = .03f;
            flickThreshold.GetVariable(inputData).defaultValue = 2500f;
            axisTransitionSpread.GetVariable(inputData).defaultValue = .5f;
            frameStepValue.GetVariable(inputData).defaultValue = .02f;

            // With the axes, since every scene should set 
            ySwipeAxis.GetVariable(inputData).SetAxisType(inputData, AxisType.Y);
            yMomentumAxis.GetVariable(inputData).SetAxisType(inputData, AxisType.Y);
            
            xSwipeAxis.GetVariable(inputData).SetAxisType(inputData, AxisType.X);
            xMomentumAxis.GetVariable(inputData).SetAxisType(inputData, AxisType.X);

            for (int i = 0; i < fields.Length; i++) {

                var variableField = Utils.GetVariableFieldFromReference(fields[i], this, out var referenceValue);
                var variableValue = variableField.GetValue(referenceValue) as ScriptableObject;

                if (variableValue is ModifiableEditorVariable modifiableEditorVariable) {
                    //serializedObject.FindProperty("_" + nameof(modifiableEditorVariable.hasDefault)).boolValue = true;
                    modifiableEditorVariable.StoreCaller(inputData, "setting default from refresh dependencies",
                        "app settings");
                    modifiableEditorVariable.hasDefault = true;
                    modifiableEditorVariable.SetToDefaultValue();
                }
            }
        
            return this;
        }

#if UNITY_EDITOR
        private static dynamic CreateInputDependency(Type assetType, string name, string groupName)
        {
            return Utils.CreateScriptableObjectAsset(assetType, name, $"{Utils.settingsPath}/InputSettings/{groupName}");
        }
#endif
    }
}