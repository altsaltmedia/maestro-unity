using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro.Logic.Action
{
    [Serializable]
    public class AxisActionData : ActionData
    {
        protected override string title => nameof(AxisActionData);

        [SerializeField]
        [HideReferenceObjectPicker]
        private AxisReference _axisReference;

        private AxisReference axisReference => _axisReference;

        private enum AxisActionType { SetStatus, SetInverted }

        [SerializeField]
        private AxisActionType _axisActionType;

        private AxisActionType axisActionType
        {
            get => _axisActionType;
            set => _axisActionType = value;
        }
        
        [SerializeField]
        private bool _usePersistentVariable;

        private bool usePersistentVariable
        {
            get => _usePersistentVariable;
            set => _usePersistentVariable = value;
        }
        
        [HideIf(nameof(usePersistentVariable))]
        [SerializeField]
        private bool _targetValue;

        private bool targetValue => _targetValue;

        [SerializeField]
        [ShowIf(nameof(usePersistentVariable))]
        private BoolVariable _targetVariable;

        private BoolVariable targetVariable => _targetVariable;

        public AxisActionData(int priority) : base(priority) { }

        public override void SyncEditorActionHeadings()
        {
            if (string.IsNullOrEmpty(axisReference.referenceName) == false) {
                actionDescription = axisReference.referenceName + " > ";
                if (axisActionType == AxisActionType.SetInverted) {
                    actionDescription += $"{AxisActionType.SetInverted} ()";
                }
                else {
                    actionDescription += GetSetValueDescription();
                }
            }
            else {
                actionDescription = "Inactive - please populate a axis reference.";
            }
        }

        private string GetSetValueDescription()
        {
            string setValueDescription = AxisActionType.SetStatus.ToString();

            if (usePersistentVariable == true) {

                if (targetVariable != null) {
                    return setValueDescription += $" ({targetVariable.name})";
                }

                return setValueDescription += $" (No axis variable populated)";
            }
            
            return setValueDescription += $" ({targetValue})";
        }

        public override void PerformAction(GameObject callingObject)
        {
            if (CanPerformAction(callingObject) == false) {
                Debug.Log($"Required variable(s) not specified in {title}, canceling operation", callingObject);
                return;
            }

            switch (axisActionType) {
                
                case AxisActionType.SetStatus:
                    if (usePersistentVariable == false) {
                        axisReference.SetStatus(callingObject, targetValue);
                    }
                    else {
                        axisReference.SetStatus(callingObject, targetVariable);
                    }
                    break;
                
                case AxisActionType.SetInverted:
                    if (usePersistentVariable == false) {
                        axisReference.SetInverted(callingObject, targetValue);
                    }
                    else {
                        axisReference.SetInverted(callingObject, targetVariable);
                    }
                    break;
            }
        }

        private bool CanPerformAction(GameObject callingObject)
        {
            if (axisReference.GetVariable(callingObject) == null) {
                return false;
            }
            
            if (usePersistentVariable == true && targetVariable == null) {
                return false;
            }

            return true;
        }
    }
}