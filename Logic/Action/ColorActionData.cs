using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro.Logic.Action
{
    [Serializable]
    public class ColorActionData : ActionData
    {
        protected override string title => nameof(ColorActionData);

        [SerializeField]
        [HideReferenceObjectPicker]
        private ColorReference _colorReference = new ColorReference();

        private ColorReference colorReference => _colorReference;

        private enum ColorActionType { SetValue }

        [SerializeField]
        private ColorActionType _colorActionType;

        private ColorActionType colorActionType
        {
            get => _colorActionType;
            set => _colorActionType = value;
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
        private Color _targetValue;

        private Color targetValue => _targetValue;

        [SerializeField]
        [ShowIf(nameof(usePersistentVariable))]
        private ColorVariable _targetVariable;

        private ColorVariable targetVariable => _targetVariable;

        public ColorActionData(int priority) : base(priority) { }

        public override void SyncEditorActionHeadings()
        {
            if (string.IsNullOrEmpty(colorReference.referenceName) == false) {
                actionDescription = colorReference.referenceName + " > ";
                if (usePersistentVariable == false) {
                    actionDescription += $"{colorActionType} ({targetValue})";
                }
                else {
                    actionDescription += GetPersistentOperationDescription();
                }
            }
            else {
                actionDescription = "Inactive - please populate a color reference.";
            }
        }
        
        private string GetPersistentOperationDescription()
        {
            string operationDescription = $"{colorActionType}";

            if (targetVariable != null) {
                
                return operationDescription += $" ({targetVariable.name})";
                
            }

            return operationDescription += $" (No int variable populated)";
        }

        private string GetSetValueDescription()
        {
            string setValueDescription = ColorActionType.SetValue.ToString();

            if (usePersistentVariable == true) {

                if (targetVariable != null) {
                    return setValueDescription += $" ({targetVariable.name})";
                }

                return setValueDescription += $" (No color variable populated)";
            }
            
            return setValueDescription += $" ({targetValue})";
        }

        public override ActionData PopulateReferences(Object parentObject, string serializedPropertyPath)
        {
            string referencePath = serializedPropertyPath;
            referencePath += $".{nameof(_colorReference)}";
            _colorReference.PopulateVariable(parentObject, referencePath.Split(new[]{'.'}));
            return this;
        }

        public override void PerformAction(GameObject callingObject)
        {
            if (CanPerformAction(callingObject) == false) {
                Debug.Log($"Required variable(s) not specified in {title}, canceling operation", callingObject);
                return;
            }

            switch (colorActionType) {
                
                case ColorActionType.SetValue:
                    if (usePersistentVariable == false) {
                        colorReference.SetValue(callingObject, targetValue);
                    }
                    else {
                        colorReference.SetValue(callingObject, targetVariable);
                    }
                    break;
            }
        }

        private bool CanPerformAction(GameObject callingObject)
        {
            if (colorReference.GetVariable() == null) {
                return false;
            }
            
            if (usePersistentVariable == true && targetVariable == null) {
                return false;
            }

            return true;
        }
    }
}