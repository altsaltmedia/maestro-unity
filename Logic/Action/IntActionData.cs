using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro.Logic.Action
{
    [Serializable]
    public class IntActionData : ActionData
    {
        protected override string title => nameof(IntActionData);

        [SerializeField]
        [HideReferenceObjectPicker]
        private IntReference _intReference = new IntReference();

        private IntReference intReference => _intReference;

        private enum IntActionType
        {
            SetValue,
            ApplyChange,
            Multiply,
            ClampMax,
            ClampMin,
            SetToDistance,
            SetToRandom,
            SetToDefaultValue
        }
        
        [SerializeField]
        private IntActionType _intActionType;

        private IntActionType intActionType
        {
            get => _intActionType;
            set => _intActionType = value;
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
        private int _operatorValue;

        private int operatorValue => _operatorValue;

        [SerializeField]
        [ShowIf(nameof(usePersistentVariable))]
        private IntVariable _operatorIntVariable;

        private IntVariable operatorIntVariable => _operatorIntVariable;

        public IntActionData(int priority) : base(priority) { }

        public override void SyncEditorActionHeadings()
        {
            if (string.IsNullOrEmpty(intReference.referenceName) == false) {
                actionDescription = intReference.referenceName + " > ";
                if (usePersistentVariable == false) {
                    actionDescription += $"{intActionType} ({operatorValue})";
                }
                else {
                    actionDescription += GetPersistentOperationDescription();
                }
            }
            else {
                actionDescription = "Inactive - please populate a int reference.";
            }
        }
        
        private string GetPersistentOperationDescription()
        {
            string operationDescription = $"{intActionType}";

            if (operatorIntVariable != null) {
                
                return operationDescription += $" ({operatorIntVariable.name})";
                
            }

            return operationDescription += $" (No int variable populated)";
        }

        public override void PerformAction(GameObject callingObject)
        {
            if (CanPerformAction(callingObject) == false) {
                Debug.Log($"Required variable(s) not specified in {title}, canceling operation", callingObject);
                return;
            }
            
            switch (intActionType) {
                
                case IntActionType.SetValue:
                    if (usePersistentVariable == false) {
                        intReference.SetValue(callingObject, operatorValue);
                    }
                    else {
                        intReference.SetValue(callingObject, operatorIntVariable);
                    }
                    break;
                
                case IntActionType.ApplyChange:
                    if (usePersistentVariable == false) {
                        intReference.ApplyChange(callingObject, operatorValue);
                    }
                    else {
                        intReference.ApplyChange(callingObject, operatorIntVariable);
                    }
                    break;
                
                case IntActionType.Multiply:
                    if (usePersistentVariable == false) {
                        intReference.Multiply(callingObject, operatorValue);
                    }
                    else {
                        intReference.Multiply(callingObject, operatorIntVariable);
                    }
                    break;
                
                case IntActionType.ClampMax:
                    if (usePersistentVariable == false) {
                        intReference.ClampMax(callingObject, operatorValue);
                    }
                    else {
                        intReference.ClampMax(callingObject, operatorIntVariable);
                    }
                    break;
                
                case IntActionType.ClampMin:
                    if (usePersistentVariable == false) {
                        intReference.ClampMin(callingObject, operatorValue);
                    }
                    else {
                        intReference.ClampMin(callingObject, operatorIntVariable);
                    }
                    break;
                
                case IntActionType.SetToDistance:
                    if (usePersistentVariable == false) {
                        intReference.SetToDistance(callingObject, operatorValue);
                    }
                    else {
                        intReference.SetToDistance(callingObject, operatorIntVariable);
                    }
                    break;
                
                case IntActionType.SetToRandom:
                    if (usePersistentVariable == false) {
                        intReference.SetToRandom(callingObject);
                    }
                    else {
                        intReference.SetToRandom(callingObject);
                    }
                    break;

                case IntActionType.SetToDefaultValue:
                    if (usePersistentVariable == false) {
                        intReference.SetToDefaultValue(callingObject);
                    }
                    else {
                        intReference.SetToDefaultValue(callingObject);
                    }
                    break;
            }
        }
        
        private bool CanPerformAction(GameObject callingObject)
        {
            if (intReference.GetVariable(callingObject) == null) {
                return false;
            }
            
            if (usePersistentVariable == true && operatorIntVariable == null) {
                return false;
            }

            return true;
        }
    }
}