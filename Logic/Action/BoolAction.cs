using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro.Logic.Action
{
    [Serializable]
    public class BoolAction : ActionData
    {
        protected override string title => nameof(BoolAction);

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
        [HideIf(nameof(usePersistentVariable))]
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

        public BoolAction(int priority) : base(priority) { }

        public override void SyncEditorActionHeadings()
        {
            if (string.IsNullOrEmpty(boolReference.referenceName) == false) {
                actionDescription = boolReference.referenceName + " - ";
                if (boolActionType == BoolActionType.SetValue) {
                    actionDescription += $"{BoolActionType.SetValue} ({targetValue}) \n";
                }
                else {
                    actionDescription += $"{BoolActionType.Toggle} () \n";
                }
            }
            else {
                actionDescription = "Inactive - please populate a bool reference. \n";
            }
        }

        public override void PerformAction(GameObject callingObject)
        {
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
    }
}