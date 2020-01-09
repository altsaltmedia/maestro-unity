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
        [Title("$"+nameof(conditionEventTitle))]
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
            if (intReference.GetVariable(callingObject) == null && intReference.useConstant == false) {
                conditionEventTitle = "Please populate an int reference.";
                return;
            }

            if (intConditionVar.GetVariable(callingObject) == null && intConditionVar.useConstant == false) {
                conditionEventTitle = "Please populate a comparison condition.";
                return;
            }

            if (intReference.useConstant == true) {
                conditionEventTitle = "Trigger Condition : " + intReference.GetValue(callingObject) + " is " + operation.ToString() + " " + intConditionVar.GetValue(callingObject);
            } else {
                conditionEventTitle = "Trigger Condition : " + intReference.GetVariable(callingObject).name + " is " + operation.ToString() + " " + intConditionVar.GetValue(callingObject);
            }
        }

        public override bool CheckCondition(Object callingObject)
        {
            base.CheckCondition(callingObject);
            
            switch (operation) {

                case ComparisonValues.EqualTo:
                    if (intReference.GetValue(this.parentObject) == intConditionVar.GetValue(this.parentObject)) {
                        return true;
                    }
                    break;

                case ComparisonValues.GreaterThan:
                    if (intReference.GetValue(this.parentObject) > intConditionVar.GetValue(this.parentObject)) {
                        return true;
                    }
                    break;

                case ComparisonValues.LessThan:

                    if (intReference.GetValue(this.parentObject) < intConditionVar.GetValue(this.parentObject)) {
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