using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro
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

        public override void SyncValues()
        {
            base.SyncValues();
            if (activeLayoutCondition == null) {
                conditionEventTitle = "Please populate a layout as your condition.";
                return;
            }

            conditionEventTitle = "Trigger Condition : Active layout is " + activeLayoutCondition.name;
        }

        public override bool CheckCondition()
        {
            base.CheckCondition();
            if (modifySettings.activeLayout == activeLayoutCondition) {
                return true;
            }

            return false;
        }

        public Layout GetCondition()
        {
            return activeLayoutCondition;
        }

    }
}