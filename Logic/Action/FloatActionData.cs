using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro.Logic.Action
{
    [Serializable]
    public class FloatActionData : ActionData
    {
        protected override string title => nameof(FloatActionData);

        [SerializeField]
        [HideReferenceObjectPicker]
        private FloatReference _floatReference;

        private FloatReference floatReference => _floatReference;

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
        
        [SerializeField]
        private bool _usePersistentVariable;

        protected bool usePersistentVariable
        {
            get => _usePersistentVariable;
            set => _usePersistentVariable = value;
        }

        [HideIf(nameof(usePersistentVariable))]
        [SerializeField]
        [HideIf(nameof(floatActionType), FloatActionType.SetToSquareMagnitude)]
        private float _operatorValue;

        private float operatorValue => _operatorValue;

        [SerializeField]
        [ShowIf(nameof(usePersistentVariable))]
        [HideIf(nameof(floatActionType), FloatActionType.SetToSquareMagnitude)]
        private FloatVariable _operatorFloatVariable;

        private FloatVariable operatorFloatVariable => _operatorFloatVariable;

        [SerializeField]
        [ShowIf(nameof(floatActionType), FloatActionType.SetToSquareMagnitude)]
        private V2Variable _operatorV2Variable;

        private V2Variable operatorV2Variable => _operatorV2Variable;

        public FloatActionData(int priority) : base(priority) { }

        public override void SyncEditorActionHeadings()
        {
            if (string.IsNullOrEmpty(floatReference.referenceName) == false) {
                actionDescription = floatReference.referenceName + " > ";
                if (usePersistentVariable == true || floatActionType == FloatActionType.SetToSquareMagnitude) {
                    actionDescription += GetPersistentOperationDescription();
                }
                else {
                    actionDescription += $"{floatActionType} ({operatorValue})";
                }
            }
            else {
                actionDescription = "Inactive - please populate a float reference.";
            }
        }

        private string GetPersistentOperationDescription()
        {
            string operationDescription = $"{floatActionType}";

            if (floatActionType == FloatActionType.SetToSquareMagnitude) {
                
                if (operatorV2Variable != null) {
                    return operationDescription += $" ({operatorV2Variable.name})";
                }
                return operationDescription += $" (V2 variable required)";
            }

            if (operatorFloatVariable != null) {
                return operationDescription += $" ({operatorFloatVariable.name})";
            }

            return operationDescription += $" (No float variable populated)";
        }
        
        public override void PerformAction(GameObject callingObject)
        {
            if (CanPerformAction(callingObject) == false) {
                Debug.Log($"Required variable(s) not specified in {title}, canceling operation", callingObject);
                return;
            }
            
            switch (floatActionType) {
                
                case FloatActionType.SetValue:
                    if (usePersistentVariable == false) {
                        floatReference.SetValue(callingObject, operatorValue);
                    }
                    else {
                        floatReference.SetValue(callingObject, operatorFloatVariable);
                    }
                    break;
                
                case FloatActionType.ApplyChange:
                    if (usePersistentVariable == false) {
                        floatReference.ApplyChange(callingObject, operatorValue);
                    }
                    else {
                        floatReference.ApplyChange(callingObject, operatorFloatVariable);
                    }
                    break;
                
                case FloatActionType.Multiply:
                    if (usePersistentVariable == false) {
                        floatReference.Multiply(callingObject, operatorValue);
                    }
                    else {
                        floatReference.Multiply(callingObject, operatorFloatVariable);
                    }
                    break;
                
                case FloatActionType.ClampMax:
                    if (usePersistentVariable == false) {
                        floatReference.ClampMax(callingObject, operatorValue);
                    }
                    else {
                        floatReference.ClampMax(callingObject, operatorFloatVariable);
                    }
                    break;
                
                case FloatActionType.ClampMin:
                    if (usePersistentVariable == false) {
                        floatReference.ClampMin(callingObject, operatorValue);
                    }
                    else {
                        floatReference.ClampMin(callingObject, operatorFloatVariable);
                    }
                    break;
                
                case FloatActionType.SetToDistance:
                    if (usePersistentVariable == false) {
                        floatReference.SetToDistance(callingObject, operatorValue);
                    }
                    else {
                        floatReference.SetToDistance(callingObject, operatorFloatVariable);
                    }
                    break;
                
                case FloatActionType.SetToRandom:
                    if (usePersistentVariable == false) {
                        floatReference.SetToRandom(callingObject);
                    }
                    else {
                        floatReference.SetToRandom(callingObject);
                    }
                    break;
                
                case FloatActionType.SetToSquareMagnitude:
                    if (usePersistentVariable == false) {
                        floatReference.SetToSquareMagnitude(callingObject, operatorV2Variable);
                    }
                    else {
                        floatReference.SetToSquareMagnitude(callingObject, operatorV2Variable);
                    }
                    break;
                
                case FloatActionType.SetToDefaultValue:
                    if (usePersistentVariable == false) {
                        floatReference.SetToDefaultValue(callingObject);
                    }
                    else {
                        floatReference.SetToDefaultValue(callingObject);
                    }
                    break;
            }
        }

        private bool CanPerformAction(GameObject callingObject)
        {
            if (floatReference.GetVariable(callingObject) == null) {
                return false;
            }
            
            if (usePersistentVariable == true) {

                if (floatActionType == FloatActionType.SetToSquareMagnitude) {
                    return operatorV2Variable != null;
                }

                return operatorFloatVariable != null;
            }

            return true;
        }
    }
}