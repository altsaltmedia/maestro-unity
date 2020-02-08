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
    public class TextFamilyConditionResponse : ConditionResponse
    {
        [SerializeField]
        [Title("Text Family Reference")]
        [HideReferenceObjectPicker]
        private TextFamilyReference _textFamilyReference = new TextFamilyReference();

        private TextFamily textFamilyReference => _textFamilyReference.GetVariable() as TextFamily;

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        [Title("Text Family Status Condition")]
        [HideReferenceObjectPicker]
        private BoolReference _activeTextFamilyCondition = new BoolReference();

        private BoolReference activeTextFamilyCondition => _activeTextFamilyCondition;
        
        public TextFamilyConditionResponse(UnityEngine.Object parentObject,
            string serializedPropertyPath) : base(parentObject, serializedPropertyPath) { }

        public override bool CheckCondition(Object callingObject)
        {
            CheckPopulateReferences();
            
            if (textFamilyReference.active == activeTextFamilyCondition.GetValue()) {
                return true;
            }

            return false;
        }

        public TextFamily GetReference()
        {
            CheckPopulateReferences();
            
            return textFamilyReference;
        }
        
        public BoolReference GetCondition()
        {
            CheckPopulateReferences();
            
            return activeTextFamilyCondition;
        }
        
#if UNITY_EDITOR        
        public override ConditionResponse PopulateReferences()
        {
            base.PopulateReferences();
            
            string referencePath = serializedPropertyPath + $".{nameof(_textFamilyReference)}";
            _textFamilyReference.PopulateVariable(parentObject, referencePath);
            
            string conditionPath = serializedPropertyPath + $".{nameof(_activeTextFamilyCondition)}";
            _activeTextFamilyCondition.PopulateVariable(parentObject, conditionPath);
            
            return this;
        }
        
        public override void SyncConditionHeading(Object callingObject)
        {
            CheckPopulateReferences();
            
            if (textFamilyReference == null) {
                conditionEventTitle = "Please populate a text family as your condition.";
                return;
            }

            if (activeTextFamilyCondition.GetVariable() == null && activeTextFamilyCondition.useConstant == false) {
                conditionEventTitle = "Please populate a condition for your text family.";
                return;
            }

            string newTitle = $"Text family {textFamilyReference.name} active is ";

            if (activeTextFamilyCondition.useConstant == true) {
                newTitle += activeTextFamilyCondition.GetValue();
            }
            else {
                newTitle += $"equal to {activeTextFamilyCondition.GetVariable().name}";
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