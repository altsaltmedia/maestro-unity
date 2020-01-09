using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro.Logic.ConditionResponse {

    [Serializable]
    [ExecuteInEditMode]
    public class AlwaysConditionResponse : ConditionResponseBase
    {
        [Title("$"+nameof(conditionEventTitle))]
        public override void SyncValues(Object callingObject)
        {
            base.SyncValues(callingObject);
            conditionEventTitle = "Has no conditions and will always execute.";
        }
        
    }

}