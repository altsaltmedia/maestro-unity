using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using AltSalt.Maestro.Logic.ConditionResponse;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

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

        [SerializeField]
        private Object _parentObject;

        private Object parentObject
        {
            get => _parentObject;
            set => _parentObject = value;
        }

        [SerializeField]
        private string _serializedPropertyPath;

        private string serializedPropertyPath
        {
            get => _serializedPropertyPath;
            set => _serializedPropertyPath = value;
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
        
        [SerializeField]
        [HideInInspector]
        private List<AxisActionData> _axisActions = new List<AxisActionData>();

        private List<AxisActionData> axisActions  => _axisActions;
        
        [SerializeField]
        [HideInInspector]
        private List<ColorActionData> _colorActions = new List<ColorActionData>();

        private List<ColorActionData> colorActions => _colorActions;

        
        [FormerlySerializedAs("_conditionResponseTriggers")]
        [SerializeField]
        [HideInInspector]
        private List<ConditionResponseActionData> _conditionResponseActions = new List<ConditionResponseActionData>();

        private List<ConditionResponseActionData> conditionResponseActions => _conditionResponseActions;
        
        [SerializeField]
        [HideInInspector]
        private List<SimpleEventActionData> _simpleEventActions = new List<SimpleEventActionData>();

        private List<SimpleEventActionData> simpleEventActions => _simpleEventActions;
        
        [SerializeField]
        [HideInInspector]
        private List<ComplexEventActionData> _complexEventActions = new List<ComplexEventActionData>();

        private List<ComplexEventActionData> complexEventActions => _complexEventActions;

        private enum ActionType
        {
            Generic,
            Bool,
            Float,
            Int,
            Axis,
            Color,
            ConditionResponse,
            SimpleEventTrigger,
            ComplexEventPackager
        }

        [SerializeField]
        [PropertySpace]
        private ActionType _actionType;

        private ActionType actionType => _actionType;

        public ActionTrigger Initialize(Object parentObject, string serializedPropertyPath)
        {
            this.parentObject = parentObject;
            this.serializedPropertyPath = serializedPropertyPath;
            return this;
        }

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
                
                case ActionType.Axis:
                {
                    var action = new AxisActionData(actionData.Count + 1);
                    actionData.Add(action);
                    axisActions.Add(action);
                    break;
                }
                
                case ActionType.Color:
                {
                    var action = new ColorActionData(actionData.Count + 1);
                    actionData.Add(action);
                    colorActions.Add(action);
                    break;
                }

                case ActionType.ConditionResponse:
                {
                    string conditionResponsePath = serializedPropertyPath;
                    conditionResponsePath += $".{nameof(_conditionResponseActions)}";
                    conditionResponsePath += $".{conditionResponseActions.Count}";
                    var action = new ConditionResponseActionData(parentObject, conditionResponsePath, actionData.Count + 1);
                    actionData.Add(action);
                    conditionResponseActions.Add(action);
                    break;
                }
                
                case ActionType.SimpleEventTrigger:
                {
                    var action = new SimpleEventActionData(actionData.Count + 1);
                    actionData.Add(action);
                    simpleEventActions.Add(action);
                    break;
                }
                
                case ActionType.ComplexEventPackager:
                {
                    var action = new ComplexEventActionData(actionData.Count + 1);
                    actionData.Add(action);
                    complexEventActions.Add(action);
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
            axisActions.RemoveAll(predicate);
            colorActions.RemoveAll(predicate);
            conditionResponseActions.RemoveAll(predicate);
            simpleEventActions.RemoveAll(predicate);
            complexEventActions.RemoveAll(predicate);
            
            // Update priorities for all items based on their order
            // of appearance in the editor
            for (int i = 0; i < actionData.Count; i++) {
                actionData[i].priority = i + 1;
            }
            
            return actionData;
        }

        public ActionTrigger CallPopulateReferences()
        {
            FieldInfo[] fields = GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            for (int i = 0; i < fields.Length; i++) {
                if (fields[i].IsNotSerialized == true) {
                    continue;
                }

                var fieldValue = fields[i].GetValue(this);
                if (fieldValue is IList == false) {
                    continue;
                }

                var actionList = fieldValue as IList;
                var listType = actionList.GetType().GetGenericArguments()[0];

                if (listType.IsSubclassOf(typeof(ActionData)) == false) {
                    continue;
                }

                string actionListPath = serializedPropertyPath;
                actionListPath += $".{fields[i].Name}";

                MethodInfo methodInfo = listType.GetMethod(nameof(ActionData.PopulateReferences));

                for (int j = 0; j < actionList.Count; j++) {
                    string actionPath = actionListPath;
                    actionPath += $".{j.ToString()}";
                    methodInfo.Invoke(actionList[j], BindingFlags.Public | BindingFlags.Instance, null,
                        new object[] {parentObject, actionPath}, null);
                }
            }

            return this;
        }

        public void OnAfterDeserialize()
        {
            actionData.Clear();
            
            actionData.AddRange(genericActions);
            actionData.AddRange(boolActions);
            actionData.AddRange(floatActions);
            actionData.AddRange(intActions);
            actionData.AddRange(axisActions);
            actionData.AddRange(colorActions);
            actionData.AddRange(conditionResponseActions);
            actionData.AddRange(simpleEventActions);
            actionData.AddRange(complexEventActions);
            
            actionData.Sort((x, y) => x.priority.CompareTo(y.priority) );
        }

        public void OnBeforeSerialize()
        {
            genericActions.Sort((x, y) => x.priority.CompareTo(y.priority));
            boolActions.Sort((x, y) => x.priority.CompareTo(y.priority));
            floatActions.Sort((x, y) => x.priority.CompareTo(y.priority));
            intActions.Sort((x, y) => x.priority.CompareTo(y.priority));
            axisActions.Sort((x, y) => x.priority.CompareTo(y.priority));
            colorActions.Sort((x, y) => x.priority.CompareTo(y.priority));
            conditionResponseActions.Sort((x, y) => x.priority.CompareTo(y.priority));
            simpleEventActions.Sort((x, y) => x.priority.CompareTo(y.priority));
            complexEventActions.Sort((x, y) => x.priority.CompareTo(y.priority));
        }
    }
}