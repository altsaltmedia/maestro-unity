using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

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
        
        [PropertySpace]

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
        
        [PropertySpace]

        [SerializeField]
        [HideReferenceObjectPicker]
        [HideIf(nameof(intActionType), IntActionType.SetToDefaultValue)]
        private IntReference _operatorValue;

        private IntReference operatorValue => _operatorValue;

        public IntActionData(int priority) : base(priority) { }

        public override void PerformAction(GameObject callingObject)
        {
            if (CanPerformAction() == false) {
                Debug.Log($"Required variable(s) not specified in {title}, canceling operation", callingObject);
                return;
            }
            
            switch (intActionType) {
                
                case IntActionType.SetValue:
                    intReference.SetValue(callingObject, operatorValue.GetValue());
                    break;
                
                case IntActionType.ApplyChange:
                    intReference.ApplyChange(callingObject, operatorValue.GetValue());
                    break;
                
                case IntActionType.Multiply:
                    intReference.Multiply(callingObject, operatorValue.GetValue());
                    break;
                
                case IntActionType.ClampMax:
                    intReference.ClampMax(callingObject, operatorValue.GetValue());
                    break;
                
                case IntActionType.ClampMin:
                    intReference.ClampMin(callingObject, operatorValue.GetValue());
                    break;
                
                case IntActionType.SetToDistance:
                    intReference.SetToDistance(callingObject, operatorValue.GetValue());
                    break;
                
                case IntActionType.SetToRandom:
                    intReference.SetToRandom(callingObject);
                    break;

                case IntActionType.SetToDefaultValue:
                    intReference.SetToDefaultValue(callingObject);
                    break;
            }
        }
        
        private bool CanPerformAction()
        {
            if (intReference.GetVariable() == null) {
                return false;
            }
            
            if (intActionType == IntActionType.SetToDefaultValue == false && 
                operatorValue.useConstant == false && operatorValue.GetVariable() == null) {
                return false;
            }

            return true;
        }
        
#if UNITY_EDITOR        
        public override void SyncEditorActionHeadings()
        {
            intReference.hideConstantOptions = true;
            
            if (string.IsNullOrEmpty(intReference.referenceName) == false) {
                actionDescription = intReference.referenceName + $" > {GetOperationDescription()}";
            }
            else {
                actionDescription = "Inactive - please populate a int reference.";
            }
        }
        
        private string GetOperationDescription()
        {
            string operationDescription = $"{intActionType}";

            if (intActionType == IntActionType.SetToDefaultValue ||
                intActionType == IntActionType.SetToRandom) {
                return operationDescription += "()";
            }

            if (operatorValue.useConstant == false && operatorValue.GetVariable() == null) {
                return operationDescription += $" (No int variable populated)";
            }

            if (operatorValue.useConstant == true) {
                return operationDescription += $" ({operatorValue.GetValue()})";
            }

            return operationDescription += $" ({operatorValue.GetVariable().name})";
        }
        
        public override ActionData PopulateReferences(Object parentObject, string serializedPropertyPath)
        {
            string referencePath = serializedPropertyPath;
            referencePath += $".{nameof(_intReference)}";
            _intReference.PopulateVariable(parentObject, referencePath);

            string valuePath = serializedPropertyPath;
            valuePath += $".{nameof(_operatorValue)}";
            _operatorValue.PopulateVariable(parentObject, valuePath);
            
            return this;
        }
#endif
  
    }
}