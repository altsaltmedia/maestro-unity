using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Logic.ConditionResponse
{
    public enum EventExecutionType { ExecuteAll, CancelAfterFirstSuccess }
    
    [ExecuteInEditMode]
    public class ConditionResponseTriggerBehaviour : MonoBehaviour
    {
        [ValueDropdown("boolValueList")]
        [SerializeField]
        [FormerlySerializedAs("triggerOnStart")]
        [OnValueChanged(nameof(SetTriggerOnStart))]
        private bool _triggerOnStart = true;

        private bool triggerOnStart
        {
            get => _triggerOnStart;
            set => _triggerOnStart = value;
        }

        private ValueDropdownList<bool> boolValueList = new ValueDropdownList<bool>(){
            {"YES", true },
            {"NO", false }
        };

        [SerializeField]
        [FormerlySerializedAs("eventExecutionType")]
        [OnValueChanged(nameof(SetEventExecutionType))]
        private EventExecutionType _eventExecutionType;

        private EventExecutionType eventExecutionType => _eventExecutionType;

        [SerializeField]
        [FormerlySerializedAs("conditionResponseTrigger")]
        private ConditionResponseTrigger _conditionResponseTrigger;

        private ConditionResponseTrigger conditionResponseTrigger => _conditionResponseTrigger;

        private void Start()
        {
            if (triggerOnStart == true) {
                CallTriggerResponses();
            }
            SetEventExecutionType();
            SetTriggerOnStart();
        }

        private void SetEventExecutionType()
        {
            conditionResponseTrigger.eventExecutionType = eventExecutionType;
        }

        private void SetTriggerOnStart()
        {
            conditionResponseTrigger.triggerOnStart = triggerOnStart;
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
        private void Update()
        {   
            conditionResponseTrigger.SyncEditorActionHeadings();
            conditionResponseTrigger.CallSyncConditionHeadings(this.gameObject);
            conditionResponseTrigger.SyncUnityEventHeadings(new SerializedObject(this).FindProperty(nameof(_conditionResponseTrigger)));
        }
#endif
    }

}