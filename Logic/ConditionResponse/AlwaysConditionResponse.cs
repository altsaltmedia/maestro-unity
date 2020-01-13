using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro.Logic.ConditionResponse {

    [Serializable]
    [ExecuteInEditMode]
    public class AlwaysConditionResponse : ConditionResponseBase
    {
#if UNITY_EDITOR
        public override void SyncConditionHeading(Object callingObject)
        {
            base.SyncConditionHeading(callingObject);
            conditionEventTitle = "ALWAYS";
        }
#endif        
    }

}