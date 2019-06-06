using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [Serializable]
    [ExecuteInEditMode]
    public class LayoutConditionResponse : ModifyConditionResponse
    {
        [SerializeField]
        [Title("$conditionEventTitle")]
        [Title("$activeLayoutName")]
        [Title("Layout Condition")]
        [InfoBox("ActiveLayout in Modify Settings will be compared against this condition")]
        Layout activeLayoutCondition;

#if UNITY_EDITOR
        public override void SyncValues()
        {
            base.SyncValues();
            if (activeLayoutCondition == null) {
                conditionEventTitle = "Please populate a layout as your condition.";
                return;
            }

            conditionEventTitle = "Trigger Condition : Active layout is " + activeLayoutCondition.name;
        }
#endif
        public override bool CheckCondition()
        {
            base.CheckCondition();
            if (modifySettings.activeLayout == activeLayoutCondition) {
                return true;
            }

            return false;
        }
    }
}