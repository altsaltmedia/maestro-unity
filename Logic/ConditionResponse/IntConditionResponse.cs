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
    public class IntConditionResponse : ConditionResponse
    {
        [SerializeField]
        [Title("Int Reference")]
        [InfoBox("Int value that will be compared against condition")]
        [HideReferenceObjectPicker]
        [FormerlySerializedAs("intReference")]
        private IntReference _intReference = new IntReference();

        private IntReference intReference => _intReference;

        [PropertySpace]
        [SerializeField]
        [FormerlySerializedAs("operation")]
        private ComparisonValues _operation;

        private ComparisonValues operation => _operation;
        
        [PropertySpace]

        [SerializeField]
        [Title("Int Condition Variable")]
        [InfoBox("Condition the reference value will be compared to when determining whether to execute response")]
        [HideReferenceObjectPicker]
        [FormerlySerializedAs("intConditionVar")]
        private IntReference _intConditionVar = new IntReference();

        private IntReference intConditionVar => _intConditionVar;

        public IntConditionResponse(UnityEngine.Object parentObject,
            string serializedPropertyPath) : base(parentObject, serializedPropertyPath) { }

        public override bool CheckCondition(Object callingObject)
        {
            CheckPopulateReferences();
            
            switch (operation) {

                case ComparisonValues.EqualTo:
                    if (intReference.GetValue() == intConditionVar.GetValue()) {
                        return true;
                    }
                    break;
                    
                case ComparisonValues.NotEqualTo:
                    if (intReference.GetValue() != intConditionVar.GetValue()) {
                        return true;
                    }
                    break;

                case ComparisonValues.GreaterThan:
                    if (intReference.GetValue() > intConditionVar.GetValue()) {
                        return true;
                    }
                    break;

                case ComparisonValues.LessThan:

                    if (intReference.GetValue() < intConditionVar.GetValue()) {
                        return true;
                    }
                    break;
            }

            return false;
        }

        public IntReference GetReference()
        {
            CheckPopulateReferences();
            
            return intReference;
        }

        public IntReference GetCondition()
        {
            CheckPopulateReferences();
            
            return intConditionVar;
        }
        
#if UNITY_EDITOR        
        public override void SyncConditionHeading(Object callingObject)
        {
            CheckPopulateReferences();
            
            if (intReference.GetVariable() == null && intReference.useConstant == false) {
                conditionEventTitle = "Please populate an int reference.";
                return;
            }

            if (intConditionVar.GetVariable() == null && intConditionVar.useConstant == false) {
                conditionEventTitle = "Please populate a comparison condition.";
                return;
            }
            
            string newTitle;

            if (intReference.useConstant == true) {
                newTitle = $"{intReference.GetValue()}  is {operation} ";
            } else {
                newTitle = $"{intReference.GetVariable().name} is {operation} ";
            }

            if (intConditionVar.useConstant == true) {
                newTitle += $"{intConditionVar.GetValue()}";
            }
            else {
                newTitle += $"{intConditionVar.GetVariable().name}";
            }

            conditionEventTitle = newTitle;
        }
        
        public override ConditionResponse PopulateReferences()
        {
            base.PopulateReferences();
            
            string referencePath = serializedPropertyPath + $".{nameof(_intReference)}";
            _intReference.PopulateVariable(parentObject, referencePath);
            
            string conditionPath = serializedPropertyPath + $".{nameof(_intConditionVar)}";
            _intConditionVar.PopulateVariable(parentObject, conditionPath);
            
            return this;
        }
#endif   
        
    }
}