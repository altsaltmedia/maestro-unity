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

        public override void SyncValues(Object callingObject)
        {
            base.SyncValues(callingObject);
            if (intReference.variable == null && intReference.useConstant == false) {
                conditionEventTitle = "Please populate an int reference.";
                return;
            }

            if (intConditionVar.variable == null && intConditionVar.useConstant == false) {
                conditionEventTitle = "Please populate a comparison condition.";
                return;
            }

            if (intReference.useConstant == true) {
                conditionEventTitle = "Trigger Condition : " + intReference.value + " is " + operation.ToString() + " " + intConditionVar.value;
            } else {
                conditionEventTitle = "Trigger Condition : " + intReference.variable.name + " is " + operation.ToString() + " " + intConditionVar.value;
            }
        }

        public override bool CheckCondition()
        {
            switch (operation) {

                case ComparisonValues.EqualTo:
                    if (intReference.value == intConditionVar.value) {
                        return true;
                    }
                    break;

                case ComparisonValues.GreaterThan:
                    if (intReference.value > intConditionVar.value) {
                        return true;
                    }
                    break;

                case ComparisonValues.LessThan:

                    if (intReference.value < intConditionVar.value) {
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