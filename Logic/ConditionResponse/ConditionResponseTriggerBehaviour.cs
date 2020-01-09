using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Logic.ConditionResponse
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

        [HorizontalGroup("Split", 1f)]
        [InfoBox("Trigger the list of events")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void CallTriggerResponses()
        {
            if(conditionResponseTrigger == null) {
                Debug.Log("Please populate the condition response trigger", this);
                return;
            }

            if(eventExecutionType == EventExecutionType.ExecuteAll) {
                conditionResponseTrigger.TriggerAllResponses(this.gameObject, triggerOnStart);
            } else {
                conditionResponseTrigger.TriggerUntilFirstSuccess(this.gameObject, triggerOnStart);
            }
        }

#if UNITY_EDITOR
        void Update()
        {    
            conditionResponseTrigger.CallSyncValues(this.gameObject);
        }
#endif
    }

}