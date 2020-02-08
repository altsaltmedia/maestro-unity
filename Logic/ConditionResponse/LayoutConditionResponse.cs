using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro.Logic
{
    [Serializable]
    [ExecuteInEditMode]
    public class LayoutConditionResponse : ConditionResponse
    {
        [SerializeField]
        [Title("Layout Reference")]
        [HideReferenceObjectPicker]
        private LayoutConfigReference _layoutReference = new LayoutConfigReference();

        private LayoutConfig layoutReference => _layoutReference.GetVariable() as LayoutConfig;

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        [HideReferenceObjectPicker]
        private BoolReference _activeLayoutCondition;

        [Title("Layout Status Condition")]
        private BoolReference activeLayoutCondition => _activeLayoutCondition;
        
        public LayoutConditionResponse(UnityEngine.Object parentObject,
            string serializedPropertyPath) : base(parentObject, serializedPropertyPath) { }

        public override bool CheckCondition(Object callingObject)
        {
            CheckPopulateReferences();
            
            if (layoutReference.active == activeLayoutCondition.GetValue()) {
                return true;
            }

            return false;
        }
        
        public LayoutConfig GetReference()
        {
            CheckPopulateReferences();
            
            return layoutReference;
        }

        public BoolReference GetCondition()
        {
            CheckPopulateReferences();
            
            return activeLayoutCondition;
        }
        
#if UNITY_EDITOR
        public override ConditionResponse PopulateReferences()
        {
            base.PopulateReferences();
            
            string referencePath = serializedPropertyPath + $".{nameof(_layoutReference)}";
            _layoutReference.PopulateVariable(parentObject, referencePath);
            
            string conditionPath = serializedPropertyPath + $".{nameof(_activeLayoutCondition)}";
            _activeLayoutCondition.PopulateVariable(parentObject, conditionPath);
   
            return this;
        }
        
        public override void SyncConditionHeading(Object callingObject)
        {
            CheckPopulateReferences();
            
            if (layoutReference == null) {
                conditionEventTitle = "Please populate a layout as your condition.";
                return;
            }
            
            if (activeLayoutCondition.GetVariable() == null && activeLayoutCondition.useConstant == false) {
                conditionEventTitle = "Please populate a condition for your layout.";
                return;
            }

            string newTitle = $"Layout {layoutReference.name} active is ";

            if (activeLayoutCondition.useConstant == true) {
                newTitle += activeLayoutCondition.GetValue();
            }
            else {
                newTitle += $"equal to {activeLayoutCondition.GetVariable().name}";
            }

            conditionEventTitle = newTitle;
        }
#endif

        private static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }
}