using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro.Logic.ConditionResponse
{
    [Serializable]
    [ExecuteInEditMode]
    public class LayoutConditionResponse : ConditionResponseBase
    {
        [SerializeField]
        [Title("Layout Reference")]
        [HideReferenceObjectPicker]
        private LayoutConfig _layoutReference;

        private LayoutConfig layoutReference => _layoutReference;

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        [HideReferenceObjectPicker]
        private BoolReference _activeLayoutCondition;

        [Title("Layout Status Condition")]
        private BoolReference activeLayoutCondition => _activeLayoutCondition;
        

        public override void SyncConditionHeading(Object callingObject)
        {
            base.SyncConditionHeading(callingObject);
            if (layoutReference == null) {
                conditionEventTitle = "Please populate a layout as your condition.";
                return;
            }

            conditionEventTitle = "Layout " + layoutReference.name + " active is " + activeLayoutCondition.GetValue(this.parentObject);
        }

        public override bool CheckCondition(Object callingObject)
        {
            base.CheckCondition(callingObject);
            
            if (layoutReference.active == activeLayoutCondition.GetValue(this.parentObject)) {
                return true;
            }

            return false;
        }
        
        public LayoutConfig GetReference()
        {
            return layoutReference;
        }

        public BoolReference GetCondition()
        {
            return activeLayoutCondition;
        }

        private static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }
}