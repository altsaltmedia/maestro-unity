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
    public class FloatConditionResponse : ConditionResponseBase
    {
        [SerializeField]
        [Title("Float Reference")]
        [HideReferenceObjectPicker]
        [FormerlySerializedAs("floatReference")]
        private FloatReference _floatReference;

        private FloatReference floatReference => _floatReference;

        [PropertySpace]

        [SerializeField]
        [Title("Float Condition Variable")]
        [HideReferenceObjectPicker]
        [FormerlySerializedAs("floatConditionVar")]
        private FloatReference _floatConditionVar;

        private FloatReference floatConditionVar => _floatConditionVar;

        [PropertySpace]
        [SerializeField]
        [FormerlySerializedAs("operation")]
        private ComparisonValues _operation;

        public ComparisonValues operation => _operation;

        public FloatConditionResponse(UnityEngine.Object parentObject,
            string serializedPropertyPath) : base(parentObject, serializedPropertyPath) { }

        public override ConditionResponseBase PopulateReferences()
        {
            string referencePath = serializedPropertyPath + $".{nameof(_floatReference)}";
            _floatReference.PopulateVariable(parentObject, referencePath.Split('.'));
            
            string conditionPath = serializedPropertyPath + $".{nameof(_floatConditionVar)}";
            _floatConditionVar.PopulateVariable(parentObject, conditionPath.Split('.'));
            
            return this;
        }
        
        public override void SyncConditionHeading(Object callingObject)
        {
            CheckPopulateReferences();
            
            base.SyncConditionHeading(callingObject);
            if (floatReference.GetVariable() == null && floatReference.useConstant == false) {
                conditionEventTitle = "Please populate a float reference.";
                return;
            }

            if (floatConditionVar.GetVariable() == null && floatConditionVar.useConstant == false) {
                conditionEventTitle = "Please populate a comparison condition.";
                return;
            }

            string newTitle;
            
            if (floatReference.useConstant == true) {
                newTitle = $"{floatReference.GetValue()}  is {operation} ";
            } else {
                newTitle = $"{floatReference.GetVariable().name} is {operation} ";
            }

            if (floatConditionVar.useConstant == true) {
                newTitle += $"{floatConditionVar.GetValue()}";
            }
            else {
                newTitle += $"{floatConditionVar.GetVariable().name}";
            }

            conditionEventTitle = newTitle;
        }

        public override bool CheckCondition(Object callingObject)
        {
            CheckPopulateReferences();
            
            base.CheckCondition(callingObject);
            
            switch (operation) {

                case ComparisonValues.EqualTo:
                    if (Mathf.Approximately(floatReference.GetValue(), floatConditionVar.GetValue()) == true) {
                        return true;
                    }
                    break;

                case ComparisonValues.GreaterThan:
                    if (floatReference.GetValue() > floatConditionVar.GetValue()) {
                        return true;
                    }
                    break;

                case ComparisonValues.LessThan:

                    if (floatReference.GetValue() < floatConditionVar.GetValue()) {
                        return true;
                    }
                    break;
            }

            return false;
        }

        public FloatReference GetReference()
        {
            CheckPopulateReferences();
            
            return floatReference;
        }

        public FloatReference GetCondition()
        {
            CheckPopulateReferences();
            
            return floatConditionVar;
        }
    }
}