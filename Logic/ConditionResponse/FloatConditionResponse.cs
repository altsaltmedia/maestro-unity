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
        [Title("$"+nameof(conditionEventTitle))]
        [Title("Float Reference")]
        FloatReference floatReference;

        [PropertySpace]

        [SerializeField]
        [Title("Float Condition Variable")]
        FloatReference floatConditionVar;

        [PropertySpace]
        [SerializeField]
        ComparisonValues operation;

        public override void SyncValues(Object callingObject)
        {
            base.SyncValues(callingObject);
            if (floatReference.GetVariable(callingObject) == null && floatReference.useConstant == false) {
                conditionEventTitle = "Please populate a float reference.";
                return;
            }

            if (floatConditionVar.GetVariable(callingObject) == null && floatConditionVar.useConstant == false) {
                conditionEventTitle = "Please populate a comparison condition.";
                return;
            }

            if (floatReference.useConstant == true) {
                conditionEventTitle = "Trigger Condition : " + floatReference.GetValue(callingObject) + " is " + operation.ToString() + " " + floatConditionVar.GetValue(callingObject);
            } else {
                conditionEventTitle = "Trigger Condition : " + floatReference.GetVariable(callingObject).name + " is " + operation.ToString() + " " + floatConditionVar.GetValue(callingObject);
            }
        }

        public override bool CheckCondition(Object callingObject)
        {
            base.CheckCondition(callingObject);
            
            switch (operation) {

                case ComparisonValues.EqualTo:
                    if (Mathf.Approximately(floatReference.GetValue(this.parentObject), floatConditionVar.GetValue(this.parentObject)) == true) {
                        return true;
                    }
                    break;

                case ComparisonValues.GreaterThan:
                    if (floatReference.GetValue(this.parentObject) > floatConditionVar.GetValue(this.parentObject)) {
                        return true;
                    }
                    break;

                case ComparisonValues.LessThan:

                    if (floatReference.GetValue(this.parentObject) < floatConditionVar.GetValue(this.parentObject)) {
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