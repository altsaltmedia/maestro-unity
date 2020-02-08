using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AltSalt.Maestro.Logic
{
    [Serializable]
    [ExecuteInEditMode]
    public class ConditionResponseActionData : ActionData, IClearHiddenValues, ISyncUnityEventHeadings
    {
        public static bool debugMode => ActionTrigger.debugMode;
        
        public static bool manualOverride => ActionTrigger.manualOverride;
        
        protected override string title => nameof(ConditionResponseActionData);

        [Serializable]
        private enum ConditionResponseTypes { Bool, Float, Int, TextFamily, Layout }
        
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

        [SerializeField]
#if UNITY_EDITOR        
        [OnValueChanged(nameof(DisplayClearDialogue))]
#endif        
        [FormerlySerializedAs("triggerType")]
        [HideIf(nameof(eventExecutionType), EventExecutionType.CheckAllConditionsValid)]
        private ConditionResponseTypes _triggerType;

        private ConditionResponseTypes triggerType => _triggerType;
        
        [ValueDropdown(nameof(boolValueList))]
        [SerializeField]
        [FormerlySerializedAs("triggerOnStart")]
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
        
        [SerializeField]
        private EventExecutionType _eventExecutionType;

        public EventExecutionType eventExecutionType
        {
            get => _eventExecutionType;
            set => _eventExecutionType = value;
        }

        [SerializeField]
#if UNITY_EDITOR        
        [ShowIf(nameof(ShowBoolEvents))]
        [Title("Bool Event List")]
        [ValidateInput(nameof(IsPopulated))]
        [FormerlySerializedAs("boolEvents")]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(AlwaysAddDefaultValue = true, CustomAddFunction = nameof(AddBoolConditionResponse))]
#endif
        private List<BoolConditionResponse> _boolEvents = new List<BoolConditionResponse>();

        private List<BoolConditionResponse> boolEvents => _boolEvents;

        [SerializeField]
#if UNITY_EDITOR        
        [ShowIf(nameof(ShowFloatEvents))]
        [Title("Float Event List")]
        [ValidateInput(nameof(IsPopulated))]
        [FormerlySerializedAs("floatEvents")]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(AlwaysAddDefaultValue = true, CustomAddFunction = nameof(AddFloatConditionResponse))]
#endif
        private List<FloatConditionResponse> _floatEvents = new List<FloatConditionResponse>();

        private List<FloatConditionResponse> floatEvents => _floatEvents;

        [SerializeField]
#if UNITY_EDITOR        
        [ShowIf(nameof(ShowIntEvents))]
        [Title("Int Event List")]
        [ValidateInput(nameof(IsPopulated))]
        [FormerlySerializedAs("intEvents")]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(AlwaysAddDefaultValue = true, CustomAddFunction = nameof(AddIntConditionResponse))]
#endif
        private List<IntConditionResponse> _intEvents = new List<IntConditionResponse>();

        private List<IntConditionResponse> intEvents => _intEvents;

        [SerializeField]
#if UNITY_EDITOR        
        [ShowIf(nameof(ShowTextFamilyEvents))]
        [Title("Text Family Event List")]
        [ValidateInput(nameof(IsPopulated))]
        [FormerlySerializedAs("textFamilyEvents")]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(AlwaysAddDefaultValue = true, CustomAddFunction = nameof(AddTextFamilyConditionResponse))]
#endif
        private List<TextFamilyConditionResponse> _textFamilyEvents = new List<TextFamilyConditionResponse>();

        private List<TextFamilyConditionResponse> textFamilyEvents => _textFamilyEvents;

        [SerializeField]
#if UNITY_EDITOR        
        [ShowIf(nameof(ShowLayoutEvents))]
        [Title("Layout Event List")]
        [ValidateInput(nameof(IsPopulated))]
        [FormerlySerializedAs("layoutEvents")]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(AlwaysAddDefaultValue = true, CustomAddFunction = nameof(AddLayoutConditionResponse))]
#endif
        private List<LayoutConditionResponse> _layoutEvents = new List<LayoutConditionResponse>();

        private List<LayoutConditionResponse> layoutEvents => _layoutEvents;

        [SerializeField]
#if UNITY_EDITOR        
        [PropertyOrder(9)]
        [ValidateInput(nameof(IsPopulated))]
        [HideReferenceObjectPicker]
        [ShowIf(nameof(eventExecutionType), EventExecutionType.CheckAllConditionsValid)]
#endif        
        protected GameObjectGenericAction _action = new GameObjectGenericAction();

        public GameObjectGenericAction action
        {
            get => _action;
            set => _action = value;
        }

#if UNITY_EDITOR

        private string _genericActionDescription;

        private string genericActionDescription
        {
            get => _genericActionDescription;
            set => _genericActionDescription = value;
        }
        
        private List<UnityEventData> _cachedEventData = new List<UnityEventData>();

        private List<UnityEventData> cachedEventData
        {
            get
            {
                if (_cachedEventData == null) {
                    _cachedEventData = new List<UnityEventData>();
                }
                return _cachedEventData;
            }
            set => _cachedEventData = value;
        }

        [PropertySpace]
#endif

        public ConditionResponseActionData(Object parentObject, string serializedPropertyPath, int priority) : base(priority)
        {
#if UNITY_EDITOR
            this.parentObject = parentObject;
            this.serializedPropertyPath = serializedPropertyPath;
#endif            
        }

        public override void PerformAction(GameObject callingObject)
        {
            if (eventExecutionType == EventExecutionType.ExecuteAll) {
                TriggerAllResponses(callingObject, triggerOnStart);
            }
            else if(eventExecutionType == EventExecutionType.CancelAfterFirstSuccess) {
                TriggerUntilFirstSuccess(callingObject, triggerOnStart);
            }
            else {
                if (AllConditionsValid(callingObject, triggerOnStart) == true) {
                    action.Invoke(callingObject);
                }
            }
        }

        public void TriggerAllResponses(GameObject caller, bool triggerOnStart)
        {
            switch (triggerType) {

                case ConditionResponseTypes.Bool:
                    for (int i = 0; i < boolEvents.Count; i++) {
                        boolEvents[i].TriggerResponse(caller, triggerOnStart);
                    }
                    break;

                case ConditionResponseTypes.Float:
                    for (int i = 0; i < floatEvents.Count; i++) {
                        floatEvents[i].TriggerResponse(caller, triggerOnStart);
                    }
                    break;

                case ConditionResponseTypes.Int:
                    for (int i = 0; i < intEvents.Count; i++) {
                        intEvents[i].TriggerResponse(caller, triggerOnStart);
                    }
                    break;

                case ConditionResponseTypes.TextFamily:
                    for (int i = 0; i < textFamilyEvents.Count; i++) {
                        textFamilyEvents[i].TriggerResponse(caller, triggerOnStart);
                    }
                    break;

                case ConditionResponseTypes.Layout:
                    for (int i = 0; i < layoutEvents.Count; i++) {
                        layoutEvents[i].TriggerResponse(caller, triggerOnStart);
                    }
                    break;
            }
        }

        public void TriggerUntilFirstSuccess(GameObject caller, bool triggerOnStart)
        {
            switch (triggerType) {

                case ConditionResponseTypes.Bool:
                    for (int i = 0; i < boolEvents.Count; i++) {
                        if (boolEvents[i].CheckCondition(caller) == true) {
                            boolEvents[i].TriggerResponse(caller, triggerOnStart);
                            return;
                        }
                    }
                    break;

                case ConditionResponseTypes.Float:
                    for (int i = 0; i < floatEvents.Count; i++) {
                        if (floatEvents[i].CheckCondition(caller) == true) {
                            floatEvents[i].TriggerResponse(caller, triggerOnStart);
                            return;
                        }
                    }
                    break;

                case ConditionResponseTypes.Int:
                    for (int i = 0; i < intEvents.Count; i++) {
                        if (intEvents[i].CheckCondition(caller) == true) {
                            intEvents[i].TriggerResponse(caller, triggerOnStart);
                            return;
                        }
                    }
                    break;

                case ConditionResponseTypes.TextFamily:
                    for (int i = 0; i < textFamilyEvents.Count; i++) {
                        if (textFamilyEvents[i].CheckCondition(caller) == true) {
                            textFamilyEvents[i].TriggerResponse(caller, triggerOnStart);
                            return;
                        }
                    }
                    break;

                case ConditionResponseTypes.Layout:
                    for (int i = 0; i < layoutEvents.Count; i++) {
                        if (layoutEvents[i].CheckCondition(caller) == true) {
                            layoutEvents[i].TriggerResponse(caller, triggerOnStart);
                            return;
                        }
                    }
                    break;
            }
        }
        
        public bool AllConditionsValid(GameObject caller, bool triggerOnStart)
        {

            for (int i = 0; i < boolEvents.Count; i++) {
                if (boolEvents[i].CheckCondition(caller) == false) {
                    boolEvents[i].TriggerResponse(caller, triggerOnStart);
                    return false;
                }
            }


            for (int i = 0; i < floatEvents.Count; i++) {
                if (floatEvents[i].CheckCondition(caller) == false) {
                    floatEvents[i].TriggerResponse(caller, triggerOnStart);
                    return false;
                }
            }


            for (int i = 0; i < intEvents.Count; i++) {
                if (intEvents[i].CheckCondition(caller) == false) {
                    intEvents[i].TriggerResponse(caller, triggerOnStart);
                    return false;
                }
            }


            for (int i = 0; i < textFamilyEvents.Count; i++) {
                if (textFamilyEvents[i].CheckCondition(caller) == false) {
                    textFamilyEvents[i].TriggerResponse(caller, triggerOnStart);
                    return false;
                }
            }


            for (int i = 0; i < layoutEvents.Count; i++) {
                if (layoutEvents[i].CheckCondition(caller) == false) {
                    layoutEvents[i].TriggerResponse(caller, triggerOnStart);
                    return false;
                }
            }

            return true;
        }
        
        private bool IsPopulated(List<BoolConditionResponse> attribute)
        {
            if (eventExecutionType == EventExecutionType.CheckAllConditionsValid) {
                return true;
            }
            
            if (triggerType == ConditionResponseTypes.Bool) {
                if (attribute.Count > 0) {
                    bool isValid = true;
                    for (int i = 0; i < attribute.Count; i++) {
                        if (attribute[i].GetReference() == null || Utils.IsPopulated(attribute[i].GetReference()) == false) {
                            isValid = false;
                            break;
                        }

                        if (attribute[i].GetCondition() == null || Utils.IsPopulated(attribute[i].GetCondition()) == false) {
                            isValid = false;
                            break;
                        }

                        if (Utils.IsPopulated(attribute[i].action) == false) {
                            isValid = false;
                            break;
                        }
                    }
                    return isValid;
                }
                return false;
            }

            return true;
        }

        private bool IsPopulated(List<FloatConditionResponse> attribute)
        {
            if (eventExecutionType == EventExecutionType.CheckAllConditionsValid) {
                return true;
            }
            
            if (triggerType == ConditionResponseTypes.Float) {
                if (attribute.Count > 0) {
                    bool isValid = true;
                    for (int i = 0; i < attribute.Count; i++) {
                        if (attribute[i].GetReference() == null || Utils.IsPopulated(attribute[i].GetReference()) == false) {
                            isValid = false;
                            break;
                        }

                        if (attribute[i].GetCondition() == null || Utils.IsPopulated(attribute[i].GetCondition()) == false) {
                            isValid = false;
                            break;
                        }

                        if (Utils.IsPopulated(attribute[i].action) == false) {
                            isValid = false;
                            break;
                        }
                    }
                    return isValid;
                }
                return false;
            }

            return true;
        }

        private bool IsPopulated(List<IntConditionResponse> attribute)
        {
            if (eventExecutionType == EventExecutionType.CheckAllConditionsValid) {
                return true;
            }
            
            if (triggerType == ConditionResponseTypes.Int) {
                if (attribute.Count > 0) {
                    bool isValid = true;
                    for (int i = 0; i < attribute.Count; i++) {
                        if (attribute[i].GetReference() == null || Utils.IsPopulated(attribute[i].GetReference()) == false) {
                            isValid = false;
                            break;
                        }

                        if (attribute[i].GetCondition() == null || Utils.IsPopulated(attribute[i].GetCondition()) == false) {
                            isValid = false;
                            break;
                        }

                        if (Utils.IsPopulated(attribute[i].action) == false) {
                            isValid = false;
                            break;
                        }
                    }
                    return isValid;
                }
                return false;
            }

            return true;
        }

        private bool IsPopulated(List<TextFamilyConditionResponse> attribute)
        {
            if (eventExecutionType == EventExecutionType.CheckAllConditionsValid) {
                return true;
            }
            
            if (triggerType == ConditionResponseTypes.TextFamily) {
                if (attribute.Count > 0) {
                    bool isValid = true;
                    for (int i = 0; i < attribute.Count; i++) {
                        if (attribute[i].GetReference() == null) {
                            isValid = false;
                            break;
                        }
                        
                        if (attribute[i].GetCondition() == null || Utils.IsPopulated(attribute[i].GetCondition()) == false) {
                            isValid = false;
                            break;
                        }

                        if (Utils.IsPopulated(attribute[i].action) == false) {
                            isValid = false;
                            break;
                        }
                    }
                    return isValid;
                }
                return false;
            }

            return true;
        }

        private bool IsPopulated(List<LayoutConditionResponse> attribute)
        {
            if (eventExecutionType == EventExecutionType.CheckAllConditionsValid) {
                return true;
            }
            
            if (triggerType == ConditionResponseTypes.Layout) {
                if (attribute.Count > 0) {
                    bool isValid = true;
                    for (int i = 0; i < attribute.Count; i++) {
                        if (attribute[i].GetReference() == null) {
                            isValid = false;
                            break;
                        }
                        
                        if (attribute[i].GetCondition() == null || Utils.IsPopulated(attribute[i].GetCondition()) == false) {
                            isValid = false;
                            break;
                        }

                        if (Utils.IsPopulated(attribute[i].action) == false) {
                            isValid = false;
                            break;
                        }
                    }
                    return isValid;
                }
                return false;
            }
            return true;
        }

        private bool ShowBoolEvents()
        {
            if (triggerType == ConditionResponseTypes.Bool ||
                eventExecutionType == EventExecutionType.CheckAllConditionsValid) {
                return true;
            }

            return false;
        }
        
        private bool ShowFloatEvents()
        {
            if (triggerType == ConditionResponseTypes.Float ||
                eventExecutionType == EventExecutionType.CheckAllConditionsValid) {
                return true;
            }

            return false;
        }
        
        private bool ShowIntEvents()
        {
            if (triggerType == ConditionResponseTypes.Int ||
                eventExecutionType == EventExecutionType.CheckAllConditionsValid) {
                return true;
            }

            return false;
        }
        
        private bool ShowTextFamilyEvents()
        {
            if (triggerType == ConditionResponseTypes.TextFamily ||
                eventExecutionType == EventExecutionType.CheckAllConditionsValid) {
                return true;
            }

            return false;
        }
        
        private bool ShowLayoutEvents()
        {
            if (triggerType == ConditionResponseTypes.Layout ||
                eventExecutionType == EventExecutionType.CheckAllConditionsValid) {
                return true;
            }

            return false;
        }
        
        protected static bool IsPopulated(GameObjectGenericAction attribute)
        {
            return Utils.IsPopulated(attribute);
        }

        
#if UNITY_EDITOR
        private BoolConditionResponse AddBoolConditionResponse()
        {
            string conditionResponsePath = serializedPropertyPath;
            conditionResponsePath += $".{nameof(_boolEvents)}";
            conditionResponsePath += $".{_boolEvents.Count}";
            var conditionResponse = new BoolConditionResponse(parentObject, conditionResponsePath);
            return conditionResponse;
        }
        
        private FloatConditionResponse AddFloatConditionResponse()
        {
            string conditionResponsePath = serializedPropertyPath;
            conditionResponsePath += $".{nameof(_floatEvents)}";
            conditionResponsePath += $".{_floatEvents.Count}";
            var conditionResponse = new FloatConditionResponse(parentObject, conditionResponsePath);
            return conditionResponse;
        }

        private IntConditionResponse AddIntConditionResponse()
        {
            string conditionResponsePath = serializedPropertyPath;
            conditionResponsePath += $".{nameof(_intEvents)}";
            conditionResponsePath += $".{_intEvents.Count}";
            var conditionResponse = new IntConditionResponse(parentObject, conditionResponsePath);
            return conditionResponse;
        }
        
        private TextFamilyConditionResponse AddTextFamilyConditionResponse()
        {
            string conditionResponsePath = serializedPropertyPath;
            conditionResponsePath += $".{nameof(_textFamilyEvents)}";
            conditionResponsePath += $".{_textFamilyEvents.Count}";
            var conditionResponse = new TextFamilyConditionResponse(parentObject, conditionResponsePath);
            return conditionResponse;
        }

        private LayoutConditionResponse AddLayoutConditionResponse()
        {
            string conditionResponsePath = serializedPropertyPath;
            conditionResponsePath += $".{nameof(_layoutEvents)}";
            conditionResponsePath += $".{_layoutEvents.Count}";
            var conditionResponse = new LayoutConditionResponse(parentObject, conditionResponsePath);
            return conditionResponse;
        }
        
        public override void SyncEditorActionHeadings()
        {
            if (eventExecutionType == EventExecutionType.CheckAllConditionsValid) {

                actionDescription = "";
                actionDescription += GetHeaderConditionDescription(boolEvents.ConvertAll(x => (ConditionResponse)x));
                actionDescription += GetHeaderConditionDescription(floatEvents.ConvertAll(x => (ConditionResponse)x));
                actionDescription += GetHeaderConditionDescription(intEvents.ConvertAll(x => (ConditionResponse)x));
                actionDescription += GetHeaderConditionDescription(textFamilyEvents.ConvertAll(x => (ConditionResponse)x));
                actionDescription += GetHeaderConditionDescription(layoutEvents.ConvertAll(x => (ConditionResponse)x));
                RefreshGenericActionDescription();
                actionDescription += "    ";
                actionDescription += genericActionDescription.Replace("\n", "\n    ");
                
                return;
            }
            
            switch (triggerType) {

                case ConditionResponseTypes.Bool :
                    actionDescription = GetHeaderConditionDescriptionWithActions(boolEvents.ConvertAll(x => (ConditionResponse)x));
                    break;
                
                case ConditionResponseTypes.Float :
                    actionDescription = GetHeaderConditionDescriptionWithActions(floatEvents.ConvertAll(x => (ConditionResponse)x));
                    break;
                
                case ConditionResponseTypes.Int :
                    actionDescription = GetHeaderConditionDescriptionWithActions(intEvents.ConvertAll(x => (ConditionResponse)x));
                    break;
                
                case ConditionResponseTypes.TextFamily :
                    actionDescription = GetHeaderConditionDescriptionWithActions(textFamilyEvents.ConvertAll(x => (ConditionResponse)x));
                    break;
                
                case ConditionResponseTypes.Layout :
                    actionDescription = GetHeaderConditionDescriptionWithActions(layoutEvents.ConvertAll(x => (ConditionResponse)x));
                    break;
                
            }
        }

        private static string GetHeaderConditionDescription(List<ConditionResponse> conditionResponseList)
        {
            string actionDescription = "";
            
            for (int i = 0; i < conditionResponseList.Count; i++) {
                actionDescription += conditionResponseList[i].conditionEventTitle + "\n";
            }

            return actionDescription;
        }

        private void RefreshGenericActionDescription()
        {
            var serializedObject = new SerializedObject(parentObject);
            SerializedProperty eventList =
                Utils.FindReferenceProperty(serializedObject, serializedPropertyPath.Split('.'), nameof(_action));
            string[] parameterNames = UnityEventUtils.GetUnityEventParameters(eventList);
            if (UnityEventUtils.UnityEventValuesChanged(action, parameterNames, cachedEventData, out var eventData)) {
                if (eventData.Count > 0) {
                    cachedEventData = eventData;
                    genericActionDescription = UnityEventUtils.ParseUnityEventDescription(eventData);
                }
            }

            if (action.GetPersistentEventCount() < 1) {
                genericActionDescription = "No actions defined";
            }
        }
        
        private static string GetHeaderConditionDescriptionWithActions(List<ConditionResponse> conditionResponseList)
        {
            string actionDescription = "";
            
            for (int i = 0; i < conditionResponseList.Count; i++) {
                actionDescription += conditionResponseList[i].conditionEventTitle + "\n    ";
                actionDescription += conditionResponseList[i].eventDescription.Replace("\n", "\n    ");;
                if (i < conditionResponseList.Count - 1) {
                    actionDescription += "\n\n";
                }
            }

            if (string.IsNullOrEmpty(actionDescription)) {
                return "Please populate the condition response.";
            }
            
            return actionDescription;
        }
        
        
        public override ActionData PopulateReferences(Object parentObject, string serializedPropertyPath)
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

                if (listType.IsSubclassOf(typeof(ConditionResponse)) == false) {
                    continue;
                }

                string conditionResponseListPath = serializedPropertyPath;
                conditionResponseListPath += $".{fields[i].Name}";

                MethodInfo methodInfo = typeof(ConditionResponse).GetMethod(
                    nameof(ConditionResponse.PopulateReferences), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                for (int j = 0; j < actionList.Count; j++) {
                    string actionPath = conditionResponseListPath;
                    actionPath += $".{j.ToString()}";
                    methodInfo.Invoke(actionList[j],
                        new object[] {});
                }
            }

            return this;
        }
        
         public void CallSyncConditionHeadings(UnityEngine.Object parentObject)
        {
            if (eventExecutionType == EventExecutionType.CheckAllConditionsValid) {
                int currentIndex = 0;
                for (int i = 0; i < boolEvents.Count; i++) {
                    boolEvents[i].prefix = GetPrefix(eventExecutionType, currentIndex, boolEvents.Count);
                    boolEvents[i].SyncConditionHeading(parentObject);
                    currentIndex++;
                }
                for (int i = 0; i < floatEvents.Count; i++) {
                    floatEvents[i].prefix = GetPrefix(eventExecutionType, currentIndex, floatEvents.Count);
                    floatEvents[i].SyncConditionHeading(parentObject);
                    currentIndex++;
                }
                for (int i = 0; i < intEvents.Count; i++) {
                    intEvents[i].prefix = GetPrefix(eventExecutionType, currentIndex, intEvents.Count);
                    intEvents[i].SyncConditionHeading(parentObject);
                    currentIndex++;
                }
                for (int i = 0; i < textFamilyEvents.Count; i++) {
                    textFamilyEvents[i].prefix = GetPrefix(eventExecutionType, currentIndex, textFamilyEvents.Count);
                    textFamilyEvents[i].SyncConditionHeading(parentObject);
                    currentIndex++;
                }
                for (int i = 0; i < layoutEvents.Count; i++) {
                    layoutEvents[i].prefix = GetPrefix(eventExecutionType, currentIndex, layoutEvents.Count);
                    layoutEvents[i].SyncConditionHeading(parentObject);
                    currentIndex++;
                }
                
                return;
            }
            
            switch (triggerType) {

                case ConditionResponseTypes.Bool:
                    for (int i = 0; i < boolEvents.Count; i++) {
                        boolEvents[i].prefix = GetPrefix(eventExecutionType, i, boolEvents.Count);
                        boolEvents[i].SyncConditionHeading(parentObject);
                    }
                    break;

                case ConditionResponseTypes.Float:
                    for (int i = 0; i < floatEvents.Count; i++) {
                        floatEvents[i].prefix = GetPrefix(eventExecutionType, i, floatEvents.Count);
                        floatEvents[i].SyncConditionHeading(parentObject);
                    }
                    break;

                case ConditionResponseTypes.Int:
                    for (int i = 0; i < intEvents.Count; i++) {
                        intEvents[i].prefix = GetPrefix(eventExecutionType, i, intEvents.Count);
                        intEvents[i].SyncConditionHeading(parentObject);
                    }
                    break;

                case ConditionResponseTypes.TextFamily:
                    for (int i = 0; i < textFamilyEvents.Count; i++) {
                        textFamilyEvents[i].prefix = GetPrefix(eventExecutionType, i, textFamilyEvents.Count);
                        textFamilyEvents[i].SyncConditionHeading(parentObject);
                    }
                    break;

                case ConditionResponseTypes.Layout:
                    for (int i = 0; i < layoutEvents.Count; i++) {
                        layoutEvents[i].prefix = GetPrefix(eventExecutionType, i, layoutEvents.Count);
                        layoutEvents[i].SyncConditionHeading(parentObject);
                    }
                    break;

            }
        }
        
        public void SyncUnityEventHeadings(SerializedProperty unityEventParentProperty)
        {
            if (eventExecutionType == EventExecutionType.CheckAllConditionsValid) {
                
                for (int i = 0; i < boolEvents.Count; i++) {
                    boolEvents[i].hideGenericAction = true;
                }
                for (int i = 0; i < floatEvents.Count; i++) {
                    floatEvents[i].hideGenericAction = true;
                }
                for (int i = 0; i < intEvents.Count; i++) {
                    intEvents[i].hideGenericAction = true;
                }
                for (int i = 0; i < textFamilyEvents.Count; i++) {
                    textFamilyEvents[i].hideGenericAction = true;
                }
                for (int i = 0; i < layoutEvents.Count; i++) {
                    layoutEvents[i].hideGenericAction = true;
                }

                return;
            }
            
            switch (triggerType) {

                case ConditionResponseTypes.Bool:
                    for (int i = 0; i < boolEvents.Count; i++) {
                        SerializedProperty serializedConditionResponse = unityEventParentProperty
                            .FindPropertyRelative(nameof(_boolEvents)).GetArrayElementAtIndex(i);
                        boolEvents[i].hideGenericAction = false;
                        boolEvents[i].SyncUnityEventHeading(serializedConditionResponse);
                    }
                    break;

                case ConditionResponseTypes.Float:
                    for (int i = 0; i < floatEvents.Count; i++) {
                        SerializedProperty serializedConditionResponse = unityEventParentProperty
                            .FindPropertyRelative(nameof(_floatEvents)).GetArrayElementAtIndex(i);
                        floatEvents[i].hideGenericAction = false;
                        floatEvents[i].SyncUnityEventHeading(serializedConditionResponse);
                    }
                    break;

                case ConditionResponseTypes.Int:
                    for (int i = 0; i < intEvents.Count; i++) {
                        SerializedProperty serializedConditionResponse = unityEventParentProperty
                            .FindPropertyRelative(nameof(_intEvents)).GetArrayElementAtIndex(i);
                        intEvents[i].hideGenericAction = false;
                        intEvents[i].SyncUnityEventHeading(serializedConditionResponse);
                    }
                    break;

                case ConditionResponseTypes.TextFamily:
                    for (int i = 0; i < textFamilyEvents.Count; i++) {
                        SerializedProperty serializedConditionResponse = unityEventParentProperty
                            .FindPropertyRelative(nameof(_textFamilyEvents)).GetArrayElementAtIndex(i);
                        textFamilyEvents[i].hideGenericAction = false;
                        textFamilyEvents[i].SyncUnityEventHeading(serializedConditionResponse);
                    }
                    break;

                case ConditionResponseTypes.Layout:
                    for (int i = 0; i < layoutEvents.Count; i++) {
                        SerializedProperty serializedConditionResponse = unityEventParentProperty
                            .FindPropertyRelative(nameof(_layoutEvents)).GetArrayElementAtIndex(i);
                        layoutEvents[i].hideGenericAction = false;
                        layoutEvents[i].SyncUnityEventHeading(serializedConditionResponse);
                    }
                    break;
            }
        }

        private string GetPrefix(EventExecutionType eventExecutionType, int currentIndex, int listCount)
        {
            if (eventExecutionType == EventExecutionType.ExecuteAll) {
                return "IF";
            }

            if (currentIndex == 0) {
                return "IF";
            }

            if (eventExecutionType == EventExecutionType.CancelAfterFirstSuccess) {
                
                if (currentIndex > 0 && currentIndex < listCount - 1) {
                    return "ELSE IF";
                }
                
                return "ELSE";
            }

            return "AND";
        }
        
        void DisplayClearDialogue()
        {
            if (EditorUtility.DisplayDialog("Clear unused values?", "You just changed the condition response type. Would you like to erase unused, hidden values?", "Yes", "No")) {
                ClearHiddenValues();
            }
        }
        
        [InfoBox("Clear any values that are not currently being used by the trigger")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void ClearHiddenValues()
        {
            if (triggerType != ConditionResponseTypes.Bool) {
                boolEvents.Clear();
            }

            if (triggerType != ConditionResponseTypes.Float) {
                floatEvents.Clear();
            }

            if (triggerType != ConditionResponseTypes.Int) {
                intEvents.Clear();
            }

            if (triggerType != ConditionResponseTypes.Layout) {
                layoutEvents.Clear();
            }

            if (triggerType != ConditionResponseTypes.TextFamily) {
                textFamilyEvents.Clear();
            }
        }

#endif

    }
}