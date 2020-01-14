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
        private bool _triggerOnStart = false;

        public bool triggerOnStart => _triggerOnStart;

        private ValueDropdownList<bool> boolValueList = new ValueDropdownList<bool>(){
            {"YES", true },
            {"NO", false }
        };
        
        [HideLabel]
        [DisplayAsString(false)]
        [ShowInInspector]
        [Title("Description")]
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 15)]
        private string _fullActionDescription;

        private string fullActionDescription
        {
            get => _fullActionDescription;
            set => _fullActionDescription = value;
        }

        [ShowInInspector]
        [OnValueChanged(nameof(UpdateActionLists))]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(HideAddButton = true, Expanded = true)]
        [HideDuplicateReferenceBox]
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
        private List<FloatActionData> _floatActions = new List<FloatActionData>();

        private List<FloatActionData> floatActions => _floatActions;
        
        [SerializeField]
        [HideInInspector]
        private List<IntActionData> _intActions = new List<IntActionData>();

        private List<IntActionData> intActions => _intActions;

        
        [FormerlySerializedAs("_conditionResponseTriggers")]
        [SerializeField]
        [HideInInspector]
        private List<ConditionResponseActionData> _conditionResponseActions = new List<ConditionResponseActionData>();

        private List<ConditionResponseActionData> conditionResponseActions => _conditionResponseActions;
        
        [SerializeField]
        [HideInInspector]
        private List<SimpleEventTriggerActionData> _simpleEventTriggerActions = new List<SimpleEventTriggerActionData>();

        private List<SimpleEventTriggerActionData> simpleEventTriggerActions => _simpleEventTriggerActions;
        
        [SerializeField]
        [HideInInspector]
        private List<ComplexEventPackagerActionData> _complexEventPackagerActions = new List<ComplexEventPackagerActionData>();

        private List<ComplexEventPackagerActionData> complexEventPackagerActions => _complexEventPackagerActions;

        private enum ActionType
        {
            Generic,
            Bool,
            Float,
            Int,
            ConditionResponse,
            SimpleEventTrigger,
            ComplexEventPackager
        }

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
                
                
                case ActionType.Float:
                {
                    var action = new FloatActionData(actionData.Count + 1);
                    actionData.Add(action);
                    floatActions.Add(action);
                    break;
                }
                
                case ActionType.Int:
                {
                    var action = new IntActionData(actionData.Count + 1);
                    actionData.Add(action);
                    intActions.Add(action);
                    break;
                }

                case ActionType.ConditionResponse:
                {
                    var action = new ConditionResponseActionData(actionData.Count + 1);
                    actionData.Add(action);
                    conditionResponseActions.Add(action);
                    break;
                }
                
                case ActionType.SimpleEventTrigger:
                {
                    var action = new SimpleEventTriggerActionData(actionData.Count + 1);
                    actionData.Add(action);
                    simpleEventTriggerActions.Add(action);
                    break;
                }
                
                case ActionType.ComplexEventPackager:
                {
                    var action = new ComplexEventPackagerActionData(actionData.Count + 1);
                    actionData.Add(action);
                    complexEventPackagerActions.Add(action);
                    break;
                }
            }
        }

        [Button(ButtonSizes.Large), GUIColor(0.8f, 0.6f, 1)]
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
                
                if (string.IsNullOrEmpty(actionData[i].actionDescription) == false) {
                    fullActionDescription += actionData[i].actionDescription;
                    if (i < actionData.Count - 1) {
                        fullActionDescription += "\n\n";
                    }
                }
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

        public void SyncFullActionDescription()
        {
            fullActionDescription = "";
            
            for (int i = 0; i < actionData.Count; i++) {
                if (string.IsNullOrEmpty(actionData[i].actionDescription) == false) {
                    fullActionDescription += actionData[i].actionDescription;
                    if (i < actionData.Count - 1) {
                        fullActionDescription += "\n\n";
                    }
                }
            }
        }

        // Called whenever an item as added or removed from our ActionData list
        private List<ActionData> UpdateActionLists()
        {
            // In the cae of removal, we need to sync our serialized lists with
            // the ActionData list. Here, we get the priorities, which double
            // as unique identifiers for this use case
            List<int> actionDataIds = new List<int>();
            actionDataIds.AddRange(actionData.ConvertAll(x => x.priority));
            
            // Define a predicate to remove any item containing a priority
            // that is not defined in our actionDataIds list
            Predicate<ActionData> predicate = FindActionData;
            bool FindActionData(ActionData actionData)
            {
                return actionDataIds.Contains(actionData.priority) == false;
            }

            // Loop through our serialized lists using the predicate
            genericActions.RemoveAll(predicate);
            boolActions.RemoveAll(predicate);
            floatActions.RemoveAll(predicate);
            intActions.RemoveAll(predicate);
            conditionResponseActions.RemoveAll(predicate);
            simpleEventTriggerActions.RemoveAll(predicate);
            complexEventPackagerActions.RemoveAll(predicate);
            
            // Update priorities for all items based on their order
            // of appearance in the editor
            for (int i = 0; i < actionData.Count; i++) {
                actionData[i].priority = i + 1;
            }
            
            return actionData;
        }

        public void OnAfterDeserialize()
        {
            actionData.Clear();
            
            actionData.AddRange(genericActions);
            actionData.AddRange(boolActions);
            actionData.AddRange(floatActions);
            actionData.AddRange(intActions);
            actionData.AddRange(conditionResponseActions);
            actionData.AddRange(simpleEventTriggerActions);
            actionData.AddRange(complexEventPackagerActions);
            
            actionData.Sort((x, y) => x.priority.CompareTo(y.priority) );
        }

        public void OnBeforeSerialize()
        {
            genericActions.Sort((x, y) => x.priority.CompareTo(y.priority));
            boolActions.Sort((x, y) => x.priority.CompareTo(y.priority));
            floatActions.Sort((x, y) => x.priority.CompareTo(y.priority));
            intActions.Sort((x, y) => x.priority.CompareTo(y.priority));
            conditionResponseActions.Sort((x, y) => x.priority.CompareTo(y.priority));
            simpleEventTriggerActions.Sort((x, y) => x.priority.CompareTo(y.priority));
            complexEventPackagerActions.Sort((x, y) => x.priority.CompareTo(y.priority));
        }
    }
}