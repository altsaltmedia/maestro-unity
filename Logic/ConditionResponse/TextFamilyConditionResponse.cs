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
    public class TextFamilyConditionResponse : ConditionResponseBase
    {
        [SerializeField]
        [Title("Text Family Reference")]
        [HideReferenceObjectPicker]
        private TextFamilyReference _textFamilyReference;

        private TextFamily textFamilyReference => _textFamilyReference.GetVariable(this.parentObject);

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        [Title("Text Family Status Condition")]
        [HideReferenceObjectPicker]
        private BoolReference _activeTextFamilyCondition;

        private BoolReference activeTextFamilyCondition => _activeTextFamilyCondition;
        

        public override void SyncConditionHeading(Object callingObject)
        {
            base.SyncConditionHeading(callingObject);
            if (textFamilyReference == null) {
                conditionEventTitle = "Please populate a text family as your condition.";
                return;
            }

            conditionEventTitle = "Text family " + textFamilyReference.name + " active is " + activeTextFamilyCondition.GetValue(this.parentObject);
        }

        public override bool CheckCondition(Object callingObject)
        {
            base.CheckCondition(callingObject);
            
            if (textFamilyReference.active == activeTextFamilyCondition.GetValue(this.parentObject)) {
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