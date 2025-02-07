using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro.Logic
{
    [Serializable]
    public class ColorActionData : ActionData
    {
        protected override string title => nameof(ColorActionData);

        [SerializeField]
        [HideReferenceObjectPicker]
        private ColorReference _colorReference = new ColorReference();

        private ColorReference colorReference => _colorReference;

        [PropertySpace]
        
        private enum ColorActionType { SetValue, SetAlpha }

        [SerializeField]
        private ColorActionType _colorActionType;

        private ColorActionType colorActionType
        {
            get => _colorActionType;
            set => _colorActionType = value;
        }
        
        [PropertySpace]

        [SerializeField]
        [HideReferenceObjectPicker]
        [ShowIf(nameof(colorActionType), ColorActionType.SetValue)]
        private ColorReference _targetValue = new ColorReference();

        private ColorReference targetValue => _targetValue;
        
        [SerializeField]
        [HideReferenceObjectPicker]
        [ShowIf(nameof(colorActionType), ColorActionType.SetAlpha)]
        private FloatReference _targetAlpha = new FloatReference();

        private FloatReference targetAlpha => _targetAlpha;

        public ColorActionData(int priority) : base(priority) { }
        

        public override void PerformAction(GameObject callingObject)
        {
            if (CanPerformAction(callingObject) == false) {
                Debug.Log($"Required variable(s) not specified in {title}, canceling operation", callingObject);
                return;
            }

            switch (colorActionType) {
                
                case ColorActionType.SetValue:
                    colorReference.SetValue(callingObject, targetValue.GetValue());
                    break;
                
                case ColorActionType.SetAlpha:
                    colorReference.SetAlpha(callingObject, targetAlpha.GetValue());
                    break;
            }
        }

        private bool CanPerformAction(GameObject callingObject)
        {
            if (colorReference.GetVariable() == null) {
                return false;
            }
            
            if (colorActionType == ColorActionType.SetValue) {
                if (targetValue.useConstant == false && targetValue.GetVariable() == null) {
                    return false;
                }
            }
            // else {
            //     if (targetAlpha.useConstant == false && targetAlpha.GetVariable() == null) {
            //         return false;
            //     }
            // }

            return true;
        }
        
        
#if UNITY_EDITOR
        
        public override void SyncEditorActionHeadings()
        {
            colorReference.hideConstantOptions = true;
            
            if (string.IsNullOrEmpty(colorReference.referenceName) == false) {
                actionDescription = colorReference.referenceName + $" > {GetOperationDescription()} ";
            }
            else {
                actionDescription = "Inactive - please populate a color reference.";
            }
        }
        
        private string GetOperationDescription()
        {
            string operationDescription = $"{colorActionType}";
            
            
            if (targetValue.GetVariable() == null && targetValue.useConstant == false) {
                return operationDescription += $" (No target value specified)";
            }


            if (targetValue.useConstant == true) {
                return operationDescription += $" ({targetValue.GetValue()})";
            }
            
            return operationDescription += $" ({targetValue.GetVariable().name})";
        }
        
        public override ActionData PopulateReferences(Object parentObject, string serializedPropertyPath)
        {
            string referencePath = serializedPropertyPath;
            referencePath += $".{nameof(_colorReference)}";
            _colorReference.PopulateVariable(parentObject, referencePath);

            string valuePath = serializedPropertyPath;
            valuePath += $".{nameof(_targetValue)}";
            _targetValue.PopulateVariable(parentObject, valuePath);
            
            return this;
        }
#endif
        
    }
}