using System;
using System.Collections.Generic;
using AltSalt.Maestro.Logic.ConditionResponse;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Logic.Action
{
    [Serializable]
    public class ActionTrigger : ISerializationCallbackReceiver
    {
        [Required]
        [SerializeField]
        [ReadOnly]
        private AppSettings _appSettings;
        
        private AppSettings appSettings
        {
            get
            {
                if (_appSettings == null) {
                    _appSettings = Utils.GetAppSettings();
                }

                return _appSettings;
            }
            set => _appSettings = value;
        }

        [ValueDropdown(nameof(boolValueList))]
        [SerializeField]
        [PropertySpace]
        private bool _triggerOnStart = false;

        public bool triggerOnStart => _triggerOnStart;

        private ValueDropdownList<bool> boolValueList = new ValueDropdownList<bool>(){
            {"YES", true },
            {"NO", false }
        };
        
        [ShowInInspector]
        [OnValueChanged(nameof(UpdateActionLists))]
        [HideReferenceObjectPicker]
        [InlineProperty]
        [ListDrawerSettings(HideAddButton = true, Expanded = true)]
        private List<ActionData> _actionData = new List<ActionData>();

        private List<ActionData> actionData => _actionData;
        
        [FormerlySerializedAs("_eventActions")]
        [SerializeField]
        [HideInInspector]
        private List<GenericActionData> _genericActions = new List<GenericActionData>();

        private List<GenericActionData> genericActions => _genericActions;

        [SerializeField]
        [HideInInspector]
        private List<BoolActionData> _boolActions = new List<BoolActionData>();

        private List<BoolActionData> boolActions => _boolActions;

        [SerializeField]
        [HideInInspector]
        private List<SimpleEventTriggerActionData> _simpleEventTriggerActions = new List<SimpleEventTriggerActionData>();

        private List<SimpleEventTriggerActionData> simpleEventTriggerActions => _simpleEventTriggerActions;
        
        [FormerlySerializedAs("_conditionResponseTriggers")]
        [SerializeField]
        [HideInInspector]
        private List<ConditionResponseActionData> _conditionResponseActions = new List<ConditionResponseActionData>();

        private List<ConditionResponseActionData> conditionResponseActions => _conditionResponseActions;

        private enum ActionType { Generic, Bool, Float, Int, SimpleEventTrigger, ConditionResponse }

        [SerializeField]
        [PropertySpace]
        private ActionType _actionType;

        private ActionType actionType => _actionType;

        [PropertySpace]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void AddActionType()
        {
            switch (actionType) {

                case ActionType.Generic:
                {
                    var action = new GenericActionData(actionData.Count + 1);
                    actionData.Add(action);
                    genericActions.Add(action);
                    break;
                }
                
                case ActionType.Bool:
                {
                    var action = new BoolActionData(actionData.Count + 1);
                    actionData.Add(action);
                    boolActions.Add(action);
                    break;
                }

                case ActionType.SimpleEventTrigger:
                {
                    var action = new SimpleEventTriggerActionData(actionData.Count + 1);
                    actionData.Add(action);
                    simpleEventTriggerActions.Add(action);
                    break;
                }
                
                case ActionType.ConditionResponse:
                {
                    var action = new ConditionResponseActionData(actionData.Count + 1);
                    actionData.Add(action);
                    conditionResponseActions.Add(action);
                    break;
                }
            }
        }

        public void PerformActions(GameObject callingObject)
        {
            for (int i = 0; i < actionData.Count; i++) {
                actionData[i].PerformAction(callingObject);
            }
        }

        public void CallSyncEditorActionHeadings()
        {
            for (int i = 0; i < actionData.Count; i++) {
                actionData[i].SyncEditorActionHeadings();
            }
        }

        public void CallSyncComplexSubheadings(UnityEngine.Object parentObject, SerializedProperty serializedActionTrigger)
        {
            for (int i = 0; i < conditionResponseActions.Count; i++) {
                conditionResponseActions[i].CallSyncConditionHeadings(parentObject);
                SerializedProperty serializedActionData = serializedActionTrigger
                    .FindPropertyRelative(nameof(_conditionResponseActions))
                    .GetArrayElementAtIndex(i);
                (conditionResponseActions[i] as ISyncUnityEventHeadings).SyncUnityEventHeadings(serializedActionData);
            }
            
            for (int i = 0; i < genericActions.Count; i++) {
                SerializedProperty serializedActionData = serializedActionTrigger
                    .FindPropertyRelative(nameof(_genericActions))
                    .GetArrayElementAtIndex(i);
                (genericActions[i] as ISyncUnityEventHeadings).SyncUnityEventHeadings(serializedActionData);
            }
        }

        private List<ActionData> UpdateActionLists()
        {
            for (int i = 0; i < actionData.Count; i++) {
                actionData[i].priority = i + 1;
            }

            return actionData;
        }

        public void OnAfterDeserialize()
        {
            List<ActionData> sortedActionData = new List<ActionData>();
            
            sortedActionData.AddRange(boolActions);
            sortedActionData.AddRange(genericActions);
            sortedActionData.AddRange(simpleEventTriggerActions);
            sortedActionData.AddRange(conditionResponseActions);
            
            sortedActionData.Sort((x, y) => x.priority.CompareTo(y.priority) );
            
            actionData.AddRange(sortedActionData);
        }

        public void OnBeforeSerialize()
        {
            boolActions.Sort((x, y) => x.priority.CompareTo(y.priority));
            genericActions.Sort((x, y) => x.priority.CompareTo(y.priority));
            simpleEventTriggerActions.Sort((x, y) => x.priority.CompareTo(y.priority));
            conditionResponseActions.Sort((x, y) => x.priority.CompareTo(y.priority));
        }
    }
}