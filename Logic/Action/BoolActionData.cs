using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro.Logic
{
    [Serializable]
    public class BoolActionData : ActionData
    {
        protected override string title => nameof(BoolActionData);

        [SerializeField]
        [HideReferenceObjectPicker]
        private BoolReference _boolReference = new BoolReference();
        
        private BoolReference boolReference => _boolReference;
        
        [PropertySpace]

        private enum BoolActionType { SetValue, Toggle }

        [SerializeField]
        private BoolActionType _boolActionType;

        private BoolActionType boolActionType
        {
            get => _boolActionType;
            set => _boolActionType = value;
        }
        
        [PropertySpace]

        [SerializeField]
        [HideReferenceObjectPicker]
        [HideIf(nameof(boolActionType), BoolActionType.Toggle)]
        private BoolReference _targetValue = new BoolReference();

        private BoolReference targetValue => _targetValue;

        public BoolActionData(int priority) : base(priority) { }

        public override void PerformAction(GameObject callingObject)
        {
            if (CanPerformAction(callingObject) == false) {
                Debug.Log($"Required variable(s) not specified in {title}, canceling operation", callingObject);
                return;
            }

            switch (boolActionType) {
                
                case BoolActionType.SetValue:
                    boolReference.SetValue(callingObject, targetValue.GetValue());
                    break;
                
                case BoolActionType.Toggle:
                    boolReference.Toggle(callingObject);
                    break;
            }
        }

        private bool CanPerformAction(GameObject callingObject)
        {
            if (boolReference.GetVariable() == null) {
                return false;
            }
            
            if (targetValue.useConstant == false && targetValue.GetVariable() == null) {
                return false;
            }

            return true;
        }
    
    #if UNITY_EDITOR
        
        public override void SyncEditorActionHeadings()
        {
            boolReference.hideConstantOptions = true;
            
            if (string.IsNullOrEmpty(boolReference.referenceName) == false) {
                actionDescription = boolReference.referenceName + " > ";
                if (boolActionType == BoolActionType.Toggle) {
                    actionDescription += $"{BoolActionType.Toggle} ()";
                }
                else {
                    actionDescription += GetSetValueDescription();
                }
            }
            else {
                actionDescription = "Inactive - please populate a bool reference.";
            }
        }

        private string GetSetValueDescription()
        {
            string setValueDescription = BoolActionType.SetValue.ToString();

            if (targetValue.GetVariable() == null && targetValue.useConstant == false) {
                return setValueDescription += $" (No target value specified)";
            }

            if (targetValue.useConstant == true) {
                return setValueDescription += $" ({targetValue.GetValue()})";
            }
            
            return setValueDescription += $" ({targetValue.GetVariable().name})";
        }

        public override ActionData PopulateReferences(Object parentObject, string serializedPropertyPath)
        {
            string referencePath = serializedPropertyPath;
            referencePath += $".{nameof(_boolReference)}";
            _boolReference.PopulateVariable(parentObject, referencePath);

            string valuePath = serializedPropertyPath;
            valuePath += $".{nameof(_targetValue)}";
            _targetValue.PopulateVariable(parentObject, valuePath);
            
            return this;
        }
    #endif
        
    }
}