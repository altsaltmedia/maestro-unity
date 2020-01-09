using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro.Logic.ConditionResponse
{
    [Serializable]
    [ExecuteInEditMode]
    public class LayoutConditionResponse : ConditionResponseBase
    {
        [SerializeField]
        [Title("$"+nameof(conditionEventTitle))]
        [Title("Layout Reference")]
        private LayoutConfig _layoutReference;

        private LayoutConfig layoutReference
        {
            get => _layoutReference;
            set => _layoutReference = value;
        }

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private BoolReference _activeLayoutCondition;

        [Title("Layout Status Condition")]
        private BoolReference activeLayoutCondition => _activeLayoutCondition;

        public override void SyncValues(Object callingObject)
        {
            base.SyncValues(callingObject);
            if (layoutReference == null) {
                conditionEventTitle = "Please populate a layout as your condition.";
                return;
            }

            conditionEventTitle = "Trigger Condition : Layout " + layoutReference.name + " active is " + activeLayoutCondition.GetValue(this.parentObject);
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