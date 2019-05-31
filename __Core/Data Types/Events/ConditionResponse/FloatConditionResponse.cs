using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [Serializable]
    [ExecuteInEditMode]
    public class FloatConditionResponse : ConditionResponse
    {
        [SerializeField]
        [Title("$conditionEventTitle")]
        [Title("Float Reference")]
        [ValidateInput("IsPopulated")]
        [InfoBox("Float value that will be compared against condition")]
        FloatReference floatReference;

        [PropertySpace]

        [SerializeField]
        [Title("Float Condition Variable")]
        [ValidateInput("IsPopulated")]
        [InfoBox("Condition the reference value will be compared to when determining whether to execute response")]
        FloatReference floatConditionVar;

        [PropertySpace]
        [SerializeField]
        ComparisonValues operation;

#if UNITY_EDITOR
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
#endif

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

        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }
}