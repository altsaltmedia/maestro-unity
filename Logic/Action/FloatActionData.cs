using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro.Logic.Action
{
    [Serializable]
    public class FloatActionData : ActionData
    {
        protected override string title => nameof(FloatActionData);

        [SerializeField]
        [HideReferenceObjectPicker]
        private FloatReference _floatReference = new FloatReference();

        private FloatReference floatReference => _floatReference;
        
        [PropertySpace]

        private enum FloatActionType
        {
            SetValue,
            ApplyChange,
            Multiply,
            ClampMax,
            ClampMin,
            SetToDistance,
            SetToRandom,
            SetToSquareMagnitude,
            SetToDefaultValue
        }
        
        [SerializeField]
        private FloatActionType _floatActionType;

        private FloatActionType floatActionType
        {
            get => _floatActionType;
            set => _floatActionType = value;
        }
        
        [PropertySpace]

        [SerializeField]
        [HideIf(nameof(floatActionType), FloatActionType.SetToSquareMagnitude)]
        [HideIf(nameof(floatActionType), FloatActionType.SetToDefaultValue)]
        [HideIf(nameof(floatActionType), FloatActionType.SetToRandom)]
        [HideReferenceObjectPicker]
        private FloatReference _operatorFloatValue;

        private FloatReference operatorFloatValue => _operatorFloatValue;

        [SerializeField]
        [ShowIf(nameof(floatActionType), FloatActionType.SetToSquareMagnitude)]
        private V2Reference _operatorV2Value;

        private V2Reference operatorV2Value => _operatorV2Value;

        public FloatActionData(int priority) : base(priority) { }

        public override void PerformAction(GameObject callingObject)
        {
            if (CanPerformAction(callingObject) == false) {
                Debug.Log($"Required variable(s) not specified in {title}, canceling operation", callingObject);
                return;
            }
            
            switch (floatActionType) {
                
                case FloatActionType.SetValue:
                    floatReference.SetValue(callingObject, operatorFloatValue.GetValue());
                    break;
                
                case FloatActionType.ApplyChange:
                    floatReference.ApplyChange(callingObject, operatorFloatValue.GetValue());
                    break;
                
                case FloatActionType.Multiply:
                    floatReference.Multiply(callingObject, operatorFloatValue.GetValue());
                    break;
                
                case FloatActionType.ClampMax:
                    floatReference.ClampMax(callingObject, operatorFloatValue.GetValue());
                    break;
                
                case FloatActionType.ClampMin:
                    floatReference.ClampMin(callingObject, operatorFloatValue.GetValue());
                    break;
                
                case FloatActionType.SetToDistance:
                    floatReference.SetToDistance(callingObject, operatorFloatValue.GetValue());
                    break;
                
                case FloatActionType.SetToRandom:
                    floatReference.SetToRandom(callingObject);
                    break;
                
                case FloatActionType.SetToSquareMagnitude:
                    floatReference.SetToSquareMagnitude(callingObject, operatorV2Value.GetVariable() as V2Variable);
                    break;
                
                case FloatActionType.SetToDefaultValue:
                    floatReference.SetToDefaultValue(callingObject);
                    break;
            }
        }

        private bool CanPerformAction(GameObject callingObject)
        {
            if (floatReference.GetVariable() == null) {
                return false;
            }
            
            if (floatActionType == FloatActionType.SetToDefaultValue ||
                floatActionType == FloatActionType.SetToRandom) {
                return true;
            }
            
            if (floatActionType == FloatActionType.SetToSquareMagnitude) {

                if (operatorV2Value.useConstant == false && operatorV2Value.GetVariable() == null) {
                    return false;
                }

                return true;
            }
            
            if (operatorFloatValue.useConstant == false && operatorFloatValue.GetVariable() == null) {
                return false;
            }

            return true;
        }
        
#if UNITY_EDITOR        
        public override void SyncEditorActionHeadings()
        {
            floatReference.hideConstantOptions = true;
            
            if (string.IsNullOrEmpty(floatReference.referenceName) == false) {
                actionDescription = floatReference.referenceName+ $" > {GetOperationDescription()}";
            }
            else {
                actionDescription = "Inactive - please populate a float reference.";
            }
        }

        private string GetOperationDescription()
        {
            string operationDescription = $"{floatActionType}";

            if (floatActionType == FloatActionType.SetToDefaultValue ||
                floatActionType == FloatActionType.SetToRandom) {
                return operationDescription += "()";
            }

            // Special case for square magnitude
            if (floatActionType == FloatActionType.SetToSquareMagnitude) {
                
                if (operatorV2Value.useConstant == false && operatorV2Value.GetVariable() == null) {
                    return operationDescription += $" (V2 variable required)";
                }

                if (operatorV2Value.useConstant == true) {
                    return operationDescription += $" ({operatorV2Value.GetValue()})";
                }
                
                return operationDescription += $" ({operatorV2Value.GetVariable().name})";
            }

            // All normal cases
            if (operatorFloatValue.useConstant == false && operatorFloatValue.GetVariable() == null) {
                return operationDescription += $" (No float variable populated)";
            }
            
            if (operatorFloatValue.useConstant == true) {
                return operationDescription += $" ({operatorFloatValue.GetValue()})";
            }
            
            return operationDescription += $" ({operatorFloatValue.GetVariable().name})";
        }
        
        public override ActionData PopulateReferences(Object parentObject, string serializedPropertyPath)
        {
            string referencePath = serializedPropertyPath;
            referencePath += $".{nameof(_floatReference)}";
            _floatReference.PopulateVariable(parentObject, referencePath);

            string floatValuePath = serializedPropertyPath;
            floatValuePath += $".{nameof(_operatorFloatValue)}";
            _operatorFloatValue.PopulateVariable(parentObject, floatValuePath);
            
            string v2ValuePath = serializedPropertyPath;
            v2ValuePath += $".{nameof(_operatorV2Value)}";
            _operatorV2Value.PopulateVariable(parentObject, v2ValuePath);
            
            return this;
        }
#endif  
    }
}