using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro
{
    [Serializable]
    [ExecuteInEditMode]
    public class IntConditionResponse : ConditionResponseBase
    {
        [SerializeField]
        [Title("$conditionEventTitle")]
        [Title("Int Reference")]
        [InfoBox("Int value that will be compared against condition")]
        IntReference intReference;

        [PropertySpace]

        [SerializeField]
        [Title("Int Condition Variable")]
        [InfoBox("Condition the reference value will be compared to when determining whether to execute response")]
        IntReference intConditionVar;

        [PropertySpace]
        [SerializeField]
        ComparisonValues operation;

        public override void SyncValues()
        {
            if (intReference.Variable == null && intReference.UseConstant == false) {
                conditionEventTitle = "Please populate an int reference.";
                return;
            }

            if (intConditionVar.Variable == null && intConditionVar.UseConstant == false) {
                conditionEventTitle = "Please populate a comparison condition.";
                return;
            }

            if (intReference.UseConstant == true) {
                conditionEventTitle = "Trigger Condition : " + intReference.Value + " is " + operation.ToString() + " " + intConditionVar.Value;
            } else {
                conditionEventTitle = "Trigger Condition : " + intReference.Variable.name + " is " + operation.ToString() + " " + intConditionVar.Value;
            }
        }

        public override bool CheckCondition()
        {
            switch (operation) {

                case ComparisonValues.EqualTo:
                    if (Mathf.Approximately(intReference.Value, intConditionVar.Value) == true) {
                        return true;
                    }
                    break;

                case ComparisonValues.GreaterThan:
                    if (intReference.Value > intConditionVar.Value) {
                        return true;
                    }
                    break;

                case ComparisonValues.LessThan:

                    if (intReference.Value < intConditionVar.Value) {
                        return true;
                    }
                    break;
            }

            return false;
        }

        public IntReference GetReference()
        {
            return intReference;
        }

        public IntReference GetCondition()
        {
            return intConditionVar;
        }
    }
}