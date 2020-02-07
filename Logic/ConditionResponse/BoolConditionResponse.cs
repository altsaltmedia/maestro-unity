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
    public class BoolConditionResponse : ConditionResponseBase
    {
        [Title("Bool Reference")]
//        [InfoBox("Bool value that will be compared against condition")]
        [FormerlySerializedAs("boolReference")]
        [SerializeField]
        [HideReferenceObjectPicker]
        private BoolReference _boolReference = new BoolReference();

        private BoolReference boolReference => _boolReference;

        [PropertySpace]

        [SerializeField]
        [Title("Bool Condition")]
//        [InfoBox("Condition the bool value should match in order to execute response")]
        [FormerlySerializedAs("boolCondition")]
        [HideReferenceObjectPicker]
        private BoolReference _boolCondition = new BoolReference();

        private BoolReference boolCondition => _boolCondition;

        public BoolConditionResponse(UnityEngine.Object parentObject,
            string serializedPropertyPath) : base(parentObject, serializedPropertyPath)
        {
            
        }

        public override bool CheckCondition(Object callingObject)
        {
            CheckPopulateReferences();
            
            if (boolReference.GetValue() == boolCondition.GetValue()) {
                return true;
            }

            return false;
        }

        public BoolReference GetReference()
        {
            CheckPopulateReferences();

            return boolReference;
        }

        public BoolReference GetCondition()
        {
            CheckPopulateReferences();
            
            return boolCondition;
        }
        
#if UNITY_EDITOR
        public override ConditionResponseBase PopulateReferences()
        {
            base.PopulateReferences();
            
            string referencePath = serializedPropertyPath + $".{nameof(_boolReference)}";
            _boolReference.PopulateVariable(parentObject, referencePath);
            
            string conditionPath = serializedPropertyPath + $".{nameof(_boolCondition)}";
            _boolCondition.PopulateVariable(parentObject, conditionPath);
            
            return this;
        }
        
        public override void SyncConditionHeading(Object callingObject)
        {
            CheckPopulateReferences();

            if(boolReference.GetVariable() == null && boolReference.useConstant == false) {
                conditionEventTitle = "Please populate a bool reference.";
                return;
            }

            if (boolCondition.GetVariable() == null && boolCondition.useConstant == false) {
                conditionEventTitle = "Please populate a comparison condition.";
                return;
            }

            string newTitle;
            
            if (boolReference.useConstant == true) {
                newTitle = $"{boolReference.GetValue()} is ";
            } else {
                newTitle = $"{boolReference.GetVariable().name} is ";
            }

            if (boolCondition.useConstant == true) {
                newTitle += boolCondition.GetValue();
            }
            else {
                newTitle +=  $"equal to {boolCondition.GetVariable().name}";
            }

            conditionEventTitle = newTitle;
        }
#endif
    }
}