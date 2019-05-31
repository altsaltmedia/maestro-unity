using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [ExecuteInEditMode]
    public class ConditionResponseTriggerBehaviour : MonoBehaviour
    {
        [ValueDropdown("boolValueList")]
        [SerializeField]
        bool triggerOnStart = true;

        private ValueDropdownList<bool> boolValueList = new ValueDropdownList<bool>(){
            {"YES", true },
            {"NO", false }
        };

        [SerializeField]
        enum EventExecutionType { ExecuteAll, CancelAfterFirstSuccess }

        [SerializeField]
        EventExecutionType eventExecutionType;

        [SerializeField]
        ConditionResponseTrigger conditionResponseTrigger;

        void Start()
        {
            if (triggerOnStart == true) {
                CallTriggerResponses();
            }
        }

        public void CallTriggerResponses()
        {
            if(eventExecutionType == EventExecutionType.ExecuteAll) {
                conditionResponseTrigger.TriggerAllResponses();
            } else {
                conditionResponseTrigger.TriggerUntilFirstSuccess();
            }
        }

#if UNITY_EDITOR
        void OnGUI()
        {
            conditionResponseTrigger.CallSyncValues();
        }
#endif
    }

}