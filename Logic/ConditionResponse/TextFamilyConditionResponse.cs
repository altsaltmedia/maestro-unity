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
    public class TextFamilyConditionResponse : ConditionResponseBase
    {
        [SerializeField]
        [Title("$conditionEventTitle")]
        [Title("Text Family Reference")]
        private TextFamily _textFamilyReference;

        private TextFamily textFamilyReference
        {
            get => _textFamilyReference;
            set => _textFamilyReference = value;
        }
        
        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        [Title("Text Family Status Condition")]
        private BoolReference _activeTextFamilyCondition;

        private BoolReference activeTextFamilyCondition => _activeTextFamilyCondition;

        public override void SyncValues(Object callingObject)
        {
            base.SyncValues(callingObject);
            if (textFamilyReference == null) {
                conditionEventTitle = "Please populate a text family as your condition.";
                return;
            }

            conditionEventTitle = "Trigger Condition : Text family " + textFamilyReference.name + " active is " + activeTextFamilyCondition.GetValue(this.callingObject);
        }

        public override bool CheckCondition()
        {
            if (textFamilyReference.active == activeTextFamilyCondition.GetValue(this.callingObject)) {
                return true;
            }

            return false;
        }

        public TextFamily GetReference()
        {
            return textFamilyReference;
        }
        
        public BoolReference GetCondition()
        {
            return activeTextFamilyCondition;
        }
        
        private static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}