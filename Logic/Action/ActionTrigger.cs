using System;
using System.Collections.Generic;
using AltSalt.Maestro.Logic.ConditionResponse;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace AltSalt.Maestro.Logic.Action
{
    [Serializable]
    public class ActionTrigger : ISerializationCallbackReceiver
    {
        [ValueDropdown(nameof(boolValueList))]
        [SerializeField]
        private bool _triggerOnStart = true;

        public bool triggerOnStart
        {
            get => _triggerOnStart;
            set => _triggerOnStart = value;
        }

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

        [SerializeField]
        [HideInInspector]
        private List<BoolAction> _boolActions = new List<BoolAction>();

        private List<BoolAction> boolActions => _boolActions;
        
        [SerializeField]
        [HideInInspector]
        private List<EventAction> _eventActions = new List<EventAction>();

        private List<EventAction> eventActions => _eventActions;
        
        [SerializeField]
        [HideInInspector]
        private List<SimpleEventTriggerAction> _simpleEventTriggerActions = new List<SimpleEventTriggerAction>();

        private List<SimpleEventTriggerAction> simpleEventTriggerActions => _simpleEventTriggerActions;
        
        [SerializeField]
        [HideInInspector]
        private List<ConditionResponseTrigger> _conditionResponseTriggers = new List<ConditionResponseTrigger>();

        private List<ConditionResponseTrigger> conditionResponseTriggers => _conditionResponseTriggers;

        private enum ActionType { Bool, Float, Int, Event, SimpleEventTrigger, ConditionResponseTrigger }

        [SerializeField]
        [PropertySpace]
        private ActionType _actionType;

        private ActionType actionType => _actionType;

        [PropertySpace]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void AddActionType()
        {
            switch (actionType) {

                case ActionType.Bool:
                {
                    var action = new BoolAction(actionData.Count + 1);
                    actionData.Add(action);
                    boolActions.Add(action);
                    break;
                }

                case ActionType.Event:
                {
                    var action = new EventAction(actionData.Count + 1);
                    actionData.Add(action);
                    eventActions.Add(action);
                    break;
                }
                
                case ActionType.SimpleEventTrigger:
                {
                    var action = new SimpleEventTriggerAction(actionData.Count + 1);
                    actionData.Add(action);
                    simpleEventTriggerActions.Add(action);
                    break;
                }
                
                case ActionType.ConditionResponseTrigger:
                {
                    var action = new ConditionResponseTrigger(actionData.Count + 1);
                    actionData.Add(action);
                    conditionResponseTriggers.Add(action);
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
            for (int i = 0; i < conditionResponseTriggers.Count; i++) {
                conditionResponseTriggers[i].CallSyncConditionHeadings(parentObject);
                SerializedProperty serializedActionData = serializedActionTrigger
                    .FindPropertyRelative(nameof(_conditionResponseTriggers))
                    .GetArrayElementAtIndex(i);
                (conditionResponseTriggers[i] as ISyncUnityEventHeadings).SyncUnityEventHeadings(serializedActionData);
            }
            
//            for (int i = 0; i < eventActions.Count; i++) {
//                SerializedProperty serializedActionData = serializedActionTrigger
//                    .FindPropertyRelative(nameof(_eventActions))
//                    .GetArrayElementAtIndex(i);
//                (eventActions[i] as ISyncUnityEventHeadings).SyncUnityEventHeadings(serializedActionData);
//            }
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
            sortedActionData.AddRange(eventActions);
            sortedActionData.AddRange(simpleEventTriggerActions);
            sortedActionData.AddRange(conditionResponseTriggers);
            
            sortedActionData.Sort((x, y) => x.priority.CompareTo(y.priority) );
            
            actionData.AddRange(sortedActionData);
        }

        public void OnBeforeSerialize()
        {
            boolActions.Sort((x, y) => x.priority.CompareTo(y.priority));
            eventActions.Sort((x, y) => x.priority.CompareTo(y.priority));
            simpleEventTriggerActions.Sort((x, y) => x.priority.CompareTo(y.priority));
            conditionResponseTriggers.Sort((x, y) => x.priority.CompareTo(y.priority));
        }
    }
}