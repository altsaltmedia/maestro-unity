using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt {

    [Serializable]
    [ExecuteInEditMode]
    public class AlwaysConditionResponse : ConditionResponse
    {
        [Title("$conditionEventTitle")]

        public override void SyncValues()
        {
            conditionEventTitle = "Has no conditions and will always execute.";
        }

        public override bool CheckCondition()
        {
            return true;
        }

    }

}