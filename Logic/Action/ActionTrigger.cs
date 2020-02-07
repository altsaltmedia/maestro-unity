/***********************************************

Copyright © AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / artemio@altsalt.com
        
**********************************************/

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
        [SerializeField]
        private bool _active = true;

        public bool active
        {
            get => _active;
            set => _active = value;
        }
        
        [ShowInInspector]
        public static bool debugMode = false;
        
        [ShowInInspector]
        [ShowIf(nameof(debugMode))]
        public static bool manualOverride = false;

        [Required]
        [SerializeField]
        [ReadOnly]
        [ShowIf(nameof(debugMode))]
        private AppSettingsReference _appSettings = new AppSettingsReference();

        private AppSettings appSettings => _appSettings.GetVariable() as AppSettings;
        
        [SerializeField]
        [ReadOnly]
        [ShowIf(nameof(debugMode))]
        private Object _parentObject;

        private Object parentObject
        {
            get => _parentObject;
            set => _parentObject = value;
        }

        [SerializeField]
        [ShowIf(nameof(debugMode))]
        [EnableIf(nameof(manualOverride))]
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
        
        [ValueDropdown(nameof(boolValueList))]
        [SerializeField]
        private bool _resetGameStateOnStart = false;

        public bool resetGameStateOnStart => _resetGameStateOnStart;
        
        [ValueDropdown(nameof(boolValueList))]
        [SerializeField]
        private bool _hasDelay = false;

        public bool hasDelay => _hasDelay;

        [SerializeField]
        [ShowIf(nameof(hasDelay))]
        private float _delay;

        public float delay
        {
            get => _delay;
            set => _delay = value;
        }

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
#if UNITY_EDITOR        
        [OnValueChanged(nameof(UpdateActionLists))]
#endif        
        [HideReferenceObjectPicker]
        [ListDrawerSettings(HideAddButton = true, Expanded = true)]
        [HideDuplicateReferenceBox]
        private List<ActionData> _actionData = new List<ActionData>();

        private List<ActionData> actionData => _actionData;
        
        [FormerlySerializedAs("_eventActions")]
        [SerializeField]
        [ShowIf(nameof(debugMode))]
        private List<GenericActionData> _genericActions = new List<GenericActionData>();

        private List<GenericActionData> genericActions => _genericActions;

        [SerializeField]
        [ShowIf(nameof(debugMode))]
        private List<BoolActionData> _boolActions = new List<BoolActionData>();

        private List<BoolActionData> boolActions => _boolActions;

        [SerializeField]
        [ShowIf(nameof(debugMode))]
        private List<FloatActionData> _floatActions = new List<FloatActionData>();

        private List<FloatActionData> floatActions => _floatActions;
        
        [SerializeField]
        [ShowIf(nameof(debugMode))]
        private List<IntActionData> _intActions = new List<IntActionData>();

        private List<IntActionData> intActions => _intActions;
        
        [SerializeField]
        [ShowIf(nameof(debugMode))]
        private List<AxisActionData> _axisActions = new List<AxisActionData>();

        private List<AxisActionData> axisActions  => _axisActions;
        
        [SerializeField]
        [ShowIf(nameof(debugMode))]
        private List<ColorActionData> _colorActions = new List<ColorActionData>();

        private List<ColorActionData> colorActions => _colorActions;

        
        [FormerlySerializedAs("_conditionResponseTriggers")]
        [SerializeField]
        [ShowIf(nameof(debugMode))]
        private List<ConditionResponseActionData> _conditionResponseActions = new List<ConditionResponseActionData>();

        private List<ConditionResponseActionData> conditionResponseActions => _conditionResponseActions;
        
        [SerializeField]
        [ShowIf(nameof(debugMode))]
        private List<SimpleEventActionData> _simpleEventActions = new List<SimpleEventActionData>();

        private List<SimpleEventActionData> simpleEventActions => _simpleEventActions;
        
        [SerializeField]
        [ShowIf(nameof(debugMode))]
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
            ComplexEventTrigger
        }

        [SerializeField]
        [PropertySpace]
        private ActionType _actionType;

        private ActionType actionType => _actionType;
        
        public static bool GetDebugMode()
        {
            return debugMode;
        }

        public ActionTrigger Initialize(Object parentObject, string serializedPropertyPath)
        {
#if UNITY_EDITOR
            this.parentObject = parentObject;
            this.serializedPropertyPath = serializedPropertyPath;
            _appSettings.PopulateVariable(parentObject, $"{serializedPropertyPath}.{nameof(_appSettings)}");
#endif            
            return this;
        }
        
        public void PerformActions(GameObject callingObject)
        {
            for (int i = 0; i < actionData.Count; i++) {
                actionData[i].PerformAction(callingObject);
            }
        }

        public IEnumerator PerformActionsDelayed(GameObject callingObject)
        {
            yield return new WaitForSeconds(delay);
            for (int i = 0; i < actionData.Count; i++) {
                actionData[i].PerformAction(callingObject);
            }
        }
        
        public void ResetGameState(GameObject callingObject)
        {
            AppSettings.ResetGameState(callingObject);
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
        
#if UNITY_EDITOR
        
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
                
                case ActionType.ComplexEventTrigger:
                {
                    var action = new ComplexEventActionData(actionData.Count + 1);
                    actionData.Add(action);
                    complexEventActions.Add(action);
                    break;
                }
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
            // Make sure that we add any pasted items to our serialized lists
            for (int i = 0; i < actionData.Count; i++) {
                switch (actionData[i])
                {
                    case GenericActionData actionData:
                    {
                        if (genericActions.Contains(actionData) == false) {
                            genericActions.Add(actionData);
                        }
                        break;
                    }
                    
                    case BoolActionData actionData:
                    {
                        if (boolActions.Contains(actionData) == false) {
                            boolActions.Add(actionData);
                        }
                        break;
                    }
                    
                    case FloatActionData actionData:
                    {
                        if (floatActions.Contains(actionData) == false) {
                            floatActions.Add(actionData);
                        }
                        break;
                    }
                    
                    case IntActionData actionData:
                    {
                        if (intActions.Contains(actionData) == false) {
                            intActions.Add(actionData);
                        }
                        break;
                    }
                    
                    case AxisActionData actionData:
                    {
                        if (axisActions.Contains(actionData) == false) {
                            axisActions.Add(actionData);
                        }
                        break;
                    }
                    
                    case ColorActionData actionData:
                    {
                        if (colorActions.Contains(actionData) == false) {
                            colorActions.Add(actionData);
                        }
                        break;
                    }
                    
                    case ConditionResponseActionData actionData:
                    {
                        if (conditionResponseActions.Contains(actionData) == false) {
                            conditionResponseActions.Add(actionData);
                        }
                        break;
                    }
                    
                    case SimpleEventActionData actionData:
                    {
                        if (simpleEventActions.Contains(actionData) == false) {
                            simpleEventActions.Add(actionData);
                        }
                        break;
                    }
                    
                    case ComplexEventActionData actionData:
                    {
                        if (complexEventActions.Contains(actionData) == false) {
                            complexEventActions.Add(actionData);
                        }
                        break;
                    }
                }
            }
            
            
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
            
            CallPopulateReferences();

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
#endif
        
    }
}