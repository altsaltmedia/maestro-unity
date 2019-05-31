using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [Serializable]
    [ExecuteInEditMode]
    public abstract class ConditionResponse
    {
#if UNITY_EDITOR
        protected string conditionEventTitle;
#endif

        [PropertySpace]
        [ValidateInput("IsPopulated")]
        [SerializeField]
        [PropertyOrder(8)]
        protected UnityEvent response;

        public abstract void SyncValues();

        public abstract bool CheckCondition();

        public void TriggerResponse()
        {
            if (CheckCondition() == true) {
                response.Invoke();
            }
        }

        private static bool IsPopulated(UnityEvent attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }

}