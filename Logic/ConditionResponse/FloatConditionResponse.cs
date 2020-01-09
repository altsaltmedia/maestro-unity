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
    public class FloatConditionResponse : ConditionResponseBase
    {
        [SerializeField]
        [Title("$conditionEventTitle")]
        [Title("Float Reference")]
        [InfoBox("Float value that will be compared against condition")]
        FloatReference floatReference;

        [PropertySpace]

        [SerializeField]
        [Title("Float Condition Variable")]
        [InfoBox("Condition the reference value will be compared to when determining whether to execute response")]
        FloatReference floatConditionVar;

        [PropertySpace]
        [SerializeField]
        ComparisonValues operation;

        public override void SyncValues(Object callingObject)
        {
            base.SyncValues(callingObject);
            if (floatReference.variable == null && floatReference.useConstant == false) {
                conditionEventTitle = "Please populate a bool reference.";
                return;
            }

            if (floatConditionVar.variable == null && floatConditionVar.useConstant == false) {
                conditionEventTitle = "Please populate a comparison condition.";
                return;
            }

            if (floatReference.useConstant == true) {
                conditionEventTitle = "Trigger Condition : " + floatReference.value + " is " + operation.ToString() + " " + floatConditionVar.value;
            } else {
                conditionEventTitle = "Trigger Condition : " + floatReference.variable.name + " is " + operation.ToString() + " " + floatConditionVar.value;
            }
        }

        public override bool CheckCondition()
        {
            switch (operation) {

                case ComparisonValues.EqualTo:
                    if (Mathf.Approximately(floatReference.value, floatConditionVar.value) == true) {
                        return true;
                    }
                    break;

                case ComparisonValues.GreaterThan:
                    if (floatReference.value > floatConditionVar.value) {
                        return true;
                    }
                    break;

                case ComparisonValues.LessThan:

                    if (floatReference.value < floatConditionVar.value) {
                        return true;
                    }
                    break;
            }

            return false;
        }

        public FloatReference GetReference()
        {
            return floatReference;
        }

        public FloatReference GetCondition()
        {
            return floatConditionVar;
        }
    }
}