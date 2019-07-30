﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [Serializable]
    [ExecuteInEditMode]
    public class TextFamilyConditionResponse : ModifyConditionResponse
    {
        [SerializeField]
        [Title("$conditionEventTitle")]
        [Title("$activeTextFamilyName")]
        [Title("Scriptable Object Reference")]
        [InfoBox("ActiveTextFamily in Modify Settings will be compared against this condition")]
        TextFamily activeTextFamilyCondition;

        public override void SyncValues()
        {
            base.SyncValues();
            if (activeTextFamilyCondition == null) {
                conditionEventTitle = "Please populate a text family as your condition.";
                return;
            }

            conditionEventTitle = "Trigger Condition : Active text family is " + activeTextFamilyCondition.name;
        }

        public override bool CheckCondition()
        {
            base.CheckCondition();
            if (modifySettings.activeTextFamily == activeTextFamilyCondition) {
                return true;
            }

            return false;
        }

        public TextFamily GetCondition()
        {
            return activeTextFamilyCondition;
        }

    }
}