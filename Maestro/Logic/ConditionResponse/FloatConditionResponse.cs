using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro
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

        public override void SyncValues()
        {
            if (floatReference.Variable == null && floatReference.UseConstant == false) {
                conditionEventTitle = "Please populate a bool reference.";
                return;
            }

            if (floatConditionVar.Variable == null && floatConditionVar.UseConstant == false) {
                conditionEventTitle = "Please populate a comparison condition.";
                return;
            }

            if (floatReference.UseConstant == true) {
                conditionEventTitle = "Trigger Condition : " + floatReference.Value + " is " + operation.ToString() + " " + floatConditionVar.Value;
            } else {
                conditionEventTitle = "Trigger Condition : " + floatReference.Variable.name + " is " + operation.ToString() + " " + floatConditionVar.Value;
            }
        }

        public override bool CheckCondition()
        {
            switch (operation) {

                case ComparisonValues.EqualTo:
                    if (Mathf.Approximately(floatReference.Value, floatConditionVar.Value) == true) {
                        return true;
                    }
                    break;

                case ComparisonValues.GreaterThan:
                    if (floatReference.Value > floatConditionVar.Value) {
                        return true;
                    }
                    break;

                case ComparisonValues.LessThan:

                    if (floatReference.Value < floatConditionVar.Value) {
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