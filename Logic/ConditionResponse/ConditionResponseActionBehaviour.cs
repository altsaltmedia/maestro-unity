using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Logic.ConditionResponse
{
    [ExecuteInEditMode]
    public class ConditionResponseActionBehaviour : MonoBehaviour
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
        [FormerlySerializedAs("_conditionResponseAction")]
        [FormerlySerializedAs("conditionResponseTrigger")]
        [FormerlySerializedAs("_conditionResponseTrigger")]
        private ConditionResponseActionData _conditionResponseActionData;

        private ConditionResponseActionData conditionResponseActionData => _conditionResponseActionData;

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
            conditionResponseActionData.eventExecutionType = eventExecutionType;
        }

        private void SetTriggerOnStart()
        {
            conditionResponseActionData.triggerOnStart = triggerOnStart;
        }

        [HorizontalGroup("Split", 1f)]
        [InfoBox("Trigger the list of events")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void CallTriggerResponses()
        {
            if(conditionResponseActionData == null) {
                Debug.Log("Please populate the condition response trigger", this);
                return;
            }

            if(eventExecutionType == EventExecutionType.ExecuteAll) {
                conditionResponseActionData.TriggerAllResponses(this.gameObject, triggerOnStart);
            } else {
                conditionResponseActionData.TriggerUntilFirstSuccess(this.gameObject, triggerOnStart);
            }
        }

#if UNITY_EDITOR
        private void Update()
        {   
            conditionResponseActionData.SyncEditorActionHeadings();
            conditionResponseActionData.CallSyncConditionHeadings(this.gameObject);
            conditionResponseActionData.SyncUnityEventHeadings(new SerializedObject(this).FindProperty(nameof(_conditionResponseActionData)));
        }
#endif
    }

}