using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro.Logic.Action
{
    [Serializable]
    public class AxisActionData : ActionData
    {
        protected override string title => nameof(AxisActionData);

        [SerializeField]
        [HideReferenceObjectPicker]
        private AxisReference _axisReference = new AxisReference();

        private AxisReference axisReference => _axisReference;

        [PropertySpace]

        private enum AxisActionType { SetStatus, SetInverted }

        [SerializeField]
        private AxisActionType _axisActionType;

        private AxisActionType axisActionType
        {
            get => _axisActionType;
            set => _axisActionType = value;
        }
        
        [PropertySpace]

        [SerializeField]
        [HideReferenceObjectPicker]
        private BoolReference _targetValue;

        private BoolReference targetValue => _targetValue;

        public AxisActionData(int priority) : base(priority) { }

        public override void SyncEditorActionHeadings()
        {
            if (string.IsNullOrEmpty(axisReference.referenceName) == false) {
                actionDescription = axisReference.referenceName + $" > {GetSetValueDescription()}";
            }
            else {
                actionDescription = "Inactive - please populate a axis reference.";
            }
        }

        private string GetSetValueDescription()
        {
            string setValueDescription = $"{axisActionType}";

            if (targetValue.useConstant == false && targetValue.GetVariable() == null) {
                return setValueDescription += $" (No target value populated)";
            }

            if (targetValue.useConstant == true) {
                return setValueDescription += $" ({targetValue.GetValue()})";
            }
            
            return setValueDescription += $" ({targetValue.GetVariable().name})";
        }
        
        public override ActionData PopulateReferences(Object parentObject, string serializedPropertyPath)
        {
            string referencePath = serializedPropertyPath;
            referencePath += $".{nameof(_axisReference)}";
            _axisReference.PopulateVariable(parentObject, referencePath.Split('.'));

            string valuePath = serializedPropertyPath;
            valuePath += $".{nameof(_targetValue)}";
            _targetValue.PopulateVariable(parentObject, valuePath.Split('.'));
            
            return this;
        }

        public override void PerformAction(GameObject callingObject)
        {
            if (CanPerformAction(callingObject) == false) {
                Debug.Log($"Required variable(s) not specified in {title}, canceling operation", callingObject);
                return;
            }

            switch (axisActionType) {
                
                case AxisActionType.SetStatus:
                    axisReference.SetStatus(callingObject, targetValue.GetValue());
                    break;
                
                case AxisActionType.SetInverted:
                    axisReference.SetInverted(callingObject, targetValue.GetValue());
                    break;
            }
        }

        private bool CanPerformAction(GameObject callingObject)
        {
            if (axisReference.GetVariable() == null) {
                return false;
            }
            
            if (targetValue.useConstant == false && targetValue.GetVariable() == null) {
                return false;
            }

            return true;
        }
    }
}