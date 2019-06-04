using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AltSalt {

    [Serializable]
    [ExecuteInEditMode]
    public class AlwaysConditionResponse : ConditionResponse
    {

#if UNITY_EDITOR
        public override void SyncValues()
        {
            conditionEventTitle = "Whenever triggered, this event has no conditions and will always execute.";
        }
#endif

        public override bool CheckCondition()
        {
            return true;
        }

    }

}