using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro.Logic.Action
{
    [Serializable]
    public class BoolActionData : ActionData
    {
        protected override string title => nameof(BoolActionData);

        [SerializeField]
        [HideReferenceObjectPicker]
        private BoolReference _boolReference;

        private BoolReference boolReference => _boolReference;

        private enum BoolActionType { SetValue, Toggle }

        [SerializeField]
        private BoolActionType _boolActionType;

        private BoolActionType boolActionType
        {
            get => _boolActionType;
            set => _boolActionType = value;
        }
        
        [SerializeField]
        private bool _usePersistentVariable;

        protected bool usePersistentVariable
        {
            get => _usePersistentVariable;
            set => _usePersistentVariable = value;
        }

        [HideIf(nameof(usePersistentVariable))]
        [HideIf(nameof(boolActionType), BoolActionType.Toggle)]
        [SerializeField]
        private bool _targetValue;

        private bool targetValue => _targetValue;

        [SerializeField]
        [ShowIf(nameof(usePersistentVariable))]
        [HideIf(nameof(boolActionType), BoolActionType.Toggle)]
        private BoolVariable _targetVariable;

        private BoolVariable targetVariable => _targetVariable;

        public BoolActionData(int priority) : base(priority) { }

        public override void SyncEditorActionHeadings()
        {
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

            if (usePersistentVariable == true) {

                if (targetVariable != null) {
                    return setValueDescription += $" ({targetVariable.name})";
                }

                return setValueDescription += $" (No bool variable populated)";
            }
            
            return setValueDescription += $" ({targetValue})";
        }

        public override void PerformAction(GameObject callingObject)
        {
            if (CanPerformAction(callingObject) == false) {
                Debug.Log($"Required variable(s) not specified in {title}, canceling operation", callingObject);
                return;
            }

            switch (boolActionType) {
                
                case BoolActionType.SetValue:
                    if (usePersistentVariable == false) {
                        boolReference.SetValue(callingObject, targetValue);
                    }
                    else {
                        boolReference.SetValue(callingObject, targetVariable);
                    }
                    break;
                
                case BoolActionType.Toggle:
                    if (usePersistentVariable == false) {
                        boolReference.Toggle(callingObject);
                    }
                    else {
                        boolReference.Toggle(callingObject);
                    }
                    break;
            }
        }

        private bool CanPerformAction(GameObject callingObject)
        {
            if (boolReference.GetVariable(callingObject) == null) {
                return false;
            }
            
            if (usePersistentVariable == true && targetVariable == null) {
                return false;
            }

            return true;
        }
    }
}