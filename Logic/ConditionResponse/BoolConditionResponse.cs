﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro.Logic.ConditionResponse
{
    [Serializable]
    [ExecuteInEditMode]
    public class BoolConditionResponse : ConditionResponseBase
    {
        [SerializeField]
        [Title("$"+nameof(conditionEventTitle))]
        [Title("Bool Reference")]
//        [InfoBox("Bool value that will be compared against condition")]
        [FormerlySerializedAs("boolReference")]
        private BoolReference _boolReference = new BoolReference();

        private BoolReference boolReference => _boolReference;

        [PropertySpace]

        [SerializeField]
        [Title("Bool Condition")]
//        [InfoBox("Condition the bool value should match in order to execute response")]
        [FormerlySerializedAs("boolCondition")]
        private BoolReference _boolCondition = new BoolReference();

        private BoolReference boolCondition => _boolCondition;

        public override void SyncValues(Object callingObject)
        {
            base.SyncValues(callingObject);
            if(boolReference.GetVariable(callingObject) == null && boolReference.useConstant == false) {
                conditionEventTitle = "Please populate a bool reference.";
                return;
            }

            if (boolCondition.GetVariable(callingObject) == null && boolCondition.useConstant == false) {
                conditionEventTitle = "Please populate a comparison condition.";
                return;
            }

            if (boolReference.useConstant == true) {
                conditionEventTitle = "Trigger Condition : " + boolReference.GetValue(callingObject) + " is " + boolCondition.GetValue(this.parentObject);
            } else {
                conditionEventTitle = "Trigger Condition : " + boolReference.GetVariable(callingObject).name + " is " + boolCondition.GetValue(this.parentObject);
            }
        }

        public override bool CheckCondition(Object callingObject)
        {
            base.CheckCondition(callingObject);
            
            if (boolReference.GetValue(callingObject) == boolCondition.GetValue(callingObject)) {
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
        
        public override void TriggerResponse(GameObject caller, bool triggerOnStart)
        {
            boolReference.parentObject = caller;
            boolCondition.parentObject = caller;
            base.TriggerResponse(caller, triggerOnStart);
        }
    }
}