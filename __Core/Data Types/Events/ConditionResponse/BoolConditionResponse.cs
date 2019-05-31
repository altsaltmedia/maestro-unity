using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [Serializable]
    [ExecuteInEditMode]
    public class BoolConditionResponse : ConditionResponse
    {
        [SerializeField]
        [Title("$conditionEventTitle")]
        [Title("Bool Reference")]
        [ValidateInput("IsPopulated")]
        [InfoBox("Bool value that will be compared against condition")]
        BoolReference boolReference;

        [PropertySpace]

        [SerializeField]
        [Title("Bool Condition")]
        [ValidateInput("IsPopulated")]
        [InfoBox("Condition the bool value should match in order to execute response")]
        BoolReference boolCondition;

#if UNITY_EDITOR
        public override void SyncValues()
        {
            if(boolReference.Variable == null && boolReference.UseConstant == false) {
                conditionEventTitle = "Please populate a bool reference.";
                return;
            }

            if (boolCondition.Variable == null && boolCondition.UseConstant == false) {
                conditionEventTitle = "Please populate a comparison condition.";
                return;
            }

            if (boolReference.UseConstant == true) {
                conditionEventTitle = "Trigger Condition : " + boolReference.Value + " is " + boolCondition.Value;
            } else {
                conditionEventTitle = "Trigger Condition : " + boolReference.Variable.name + " is " + boolCondition.Value;
            }
        }
#endif

        public override bool CheckCondition()
        {
            if (boolReference.Value == boolCondition.Value) {
                return true;
            }

            return false;
        }

        private static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}