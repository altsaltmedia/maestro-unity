using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro
{
    [Serializable]
    [ExecuteInEditMode]
    public class BoolConditionResponse : ConditionResponseBase
    {
        [SerializeField]
        [Title("$conditionEventTitle")]
        [Title("Bool Reference")]
        [InfoBox("Bool value that will be compared against condition")]
        BoolReference boolReference;

        [PropertySpace]

        [SerializeField]
        [Title("Bool Condition")]
        [InfoBox("Condition the bool value should match in order to execute response")]
        BoolReference boolCondition;

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

        public override bool CheckCondition()
        {
            if (boolReference.Value == boolCondition.Value) {
                return true;
            }

            return false;
        }

        public BoolReference GetReference()
        {
            return boolReference;
        }

        public BoolReference GetCondition()
        {
            return boolCondition;
        }
    }
}