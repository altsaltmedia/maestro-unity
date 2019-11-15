using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Logic.ConditionResponse
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
        LayoutConfig _activeLayoutConfigCondition;

        public override void SyncValues()
        {
            base.SyncValues();
            if (_activeLayoutConfigCondition == null) {
                conditionEventTitle = "Please populate a layout as your condition.";
                return;
            }

            conditionEventTitle = "Trigger Condition : Active layout is " + _activeLayoutConfigCondition.name;
        }

        public override bool CheckCondition()
        {
            base.CheckCondition();
            if (modifySettings._activeLayoutConfig == _activeLayoutConfigCondition) {
                return true;
            }

            return false;
        }

        public LayoutConfig GetCondition()
        {
            return _activeLayoutConfigCondition;
        }

    }
}