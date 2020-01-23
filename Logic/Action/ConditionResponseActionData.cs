using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Sirenix.OdinInspector;
using AltSalt.Maestro.Logic.Action;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AltSalt.Maestro.Logic.ConditionResponse
{
    [Serializable]
    [ExecuteInEditMode]
    public class ConditionResponseActionData : ActionData, IClearHiddenValues, ISyncUnityEventHeadings
    {
        protected override string title => nameof(ConditionResponseActionData);

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

        [Serializable]
        private enum ConditionResponseTypes { Always, Bool, Float, Int, TextFamily, Layout }

        [SerializeField]
        [OnValueChanged(nameof(DisplayClearDialogue))]
        [FormerlySerializedAs("triggerType")]
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
        [ShowIf(nameof(triggerType), ConditionResponseTypes.Always)]
        [Title("Always Event List")]
        [ValidateInput(nameof(IsPopulated))]
        [FormerlySerializedAs("alwaysEvents")]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(AlwaysAddDefaultValue = true)]
        private List<AlwaysConditionResponse> _alwaysEvents = new List<AlwaysConditionResponse>();

        private List<AlwaysConditionResponse> alwaysEvents => _alwaysEvents;

        [SerializeField]
        [ShowIf(nameof(triggerType), ConditionResponseTypes.Bool)]
        [Title("Bool Event List")]
        [ValidateInput(nameof(IsPopulated))]
        [FormerlySerializedAs("boolEvents")]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(AlwaysAddDefaultValue = true, CustomAddFunction = nameof(AddBoolConditionResponse))]
        private List<BoolConditionResponse> _boolEvents = new List<BoolConditionResponse>();

        private List<BoolConditionResponse> boolEvents => _boolEvents;

        [SerializeField]
        [ShowIf(nameof(triggerType), ConditionResponseTypes.Float)]
        [Title("Float Event List")]
        [ValidateInput(nameof(IsPopulated))]
        [FormerlySerializedAs("floatEvents")]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(AlwaysAddDefaultValue = true, CustomAddFunction = nameof(AddFloatConditionResponse))]
        private List<FloatConditionResponse> _floatEvents = new List<FloatConditionResponse>();

        private List<FloatConditionResponse> floatEvents => _floatEvents;

        [SerializeField]
        [ShowIf(nameof(triggerType), ConditionResponseTypes.Int)]
        [Title("Int Event List")]
        [ValidateInput(nameof(IsPopulated))]
        [FormerlySerializedAs("intEvents")]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(AlwaysAddDefaultValue = true, CustomAddFunction = nameof(AddIntConditionResponse))]
        private List<IntConditionResponse> _intEvents = new List<IntConditionResponse>();

        private List<IntConditionResponse> intEvents => _intEvents;

        [SerializeField]
        [ShowIf(nameof(triggerType), ConditionResponseTypes.TextFamily)]
        [Title("Text Family Event List")]
        [ValidateInput(nameof(IsPopulated))]
        [FormerlySerializedAs("textFamilyEvents")]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(AlwaysAddDefaultValue = true, CustomAddFunction = nameof(AddTextFamilyConditionResponse))]
        private List<TextFamilyConditionResponse> _textFamilyEvents = new List<TextFamilyConditionResponse>();

        private List<TextFamilyConditionResponse> textFamilyEvents => _textFamilyEvents;

        [SerializeField]
        [ShowIf(nameof(triggerType), ConditionResponseTypes.Layout)]
        [Title("Layout Event List")]
        [ValidateInput(nameof(IsPopulated))]
        [FormerlySerializedAs("layoutEvents")]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(AlwaysAddDefaultValue = true, CustomAddFunction = nameof(AddLayoutConditionResponse))]
        private List<LayoutConditionResponse> _layoutEvents = new List<LayoutConditionResponse>();

        private List<LayoutConditionResponse> layoutEvents => _layoutEvents;

        [PropertySpace]

        public ConditionResponseActionData(Object parentObject, string serializedPropertyPath, int priority) : base(priority)
        {
            this.parentObject = parentObject;
            this.serializedPropertyPath = serializedPropertyPath;
        }

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
            switch (triggerType) {
                
                case ConditionResponseTypes.Always :
                    actionDescription = GetHeaderConditionDescription(alwaysEvents.ConvertAll(x => (ConditionResponseBase)x));
                    break;
                
                case ConditionResponseTypes.Bool :
                    actionDescription = GetHeaderConditionDescription(boolEvents.ConvertAll(x => (ConditionResponseBase)x));
                    break;
                
                case ConditionResponseTypes.Float :
                    actionDescription = GetHeaderConditionDescription(floatEvents.ConvertAll(x => (ConditionResponseBase)x));
                    break;
                
                case ConditionResponseTypes.Int :
                    actionDescription = GetHeaderConditionDescription(intEvents.ConvertAll(x => (ConditionResponseBase)x));
                    break;
                
                case ConditionResponseTypes.TextFamily :
                    actionDescription = GetHeaderConditionDescription(textFamilyEvents.ConvertAll(x => (ConditionResponseBase)x));
                    break;
                
                case ConditionResponseTypes.Layout :
                    actionDescription = GetHeaderConditionDescription(layoutEvents.ConvertAll(x => (ConditionResponseBase)x));
                    break;
                
            }
        }

        private static string GetHeaderConditionDescription(List<ConditionResponseBase> conditionResponseList)
        {
            string actionDescription = "";
            
            for (int i = 0; i < conditionResponseList.Count; i++) {
                actionDescription += conditionResponseList[i].conditionEventTitle + "\n    ";
                actionDescription += conditionResponseList[i].eventDescription.Replace("\n", "\n" + "    ");;
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

                if (listType.IsSubclassOf(typeof(ConditionResponseBase)) == false) {
                    continue;
                }

                string conditionResponseListPath = serializedPropertyPath;
                conditionResponseListPath += $".{fields[i].Name}";

                MethodInfo methodInfo = typeof(ConditionResponseBase).GetMethod(
                    nameof(ConditionResponseBase.PopulateReferences), BindingFlags.Public | BindingFlags.Instance);

                if (methodInfo == null) {
                    continue;
                }
                
                for (int j = 0; j < actionList.Count; j++) {
                    string actionPath = conditionResponseListPath;
                    actionPath += $".{j.ToString()}";
                    methodInfo.Invoke(actionList[j],
                        new object[] {});
                }
            }

            return this;
        }

        public override void PerformAction(GameObject callingObject)
        {
            if (eventExecutionType == EventExecutionType.ExecuteAll) {
                TriggerAllResponses(callingObject, triggerOnStart);
            }
            else {
                TriggerUntilFirstSuccess(callingObject, triggerOnStart);
            }
        }

        public void TriggerAllResponses(GameObject caller, bool triggerOnStart)
        {
            switch (triggerType) {

                case ConditionResponseTypes.Always:
                    for (int i = 0; i < alwaysEvents.Count; i++) {
                        alwaysEvents[i].TriggerResponse(caller, triggerOnStart);
                    }
                    break;

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

                case ConditionResponseTypes.Always:
                    for (int i = 0; i < alwaysEvents.Count; i++) {
                        if (alwaysEvents[i].CheckCondition(caller) == true) {
                            alwaysEvents[i].TriggerResponse(caller, triggerOnStart);
                            return;
                        }
                    }
                    break;

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

        public void CallSyncConditionHeadings(UnityEngine.Object parentObject)
        {
            switch (triggerType) {

                case ConditionResponseTypes.Always:
                    for (int i = 0; i < alwaysEvents.Count; i++) {
                        alwaysEvents[i].prefix = GetPrefix(eventExecutionType, i, alwaysEvents.Count);
                        alwaysEvents[i].SyncConditionHeading(parentObject);
                    }
                    break;

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
            switch (triggerType) {

                case ConditionResponseTypes.Always:
                    for (int i = 0; i < alwaysEvents.Count; i++) {
                        SerializedProperty serializedConditionResponse = unityEventParentProperty
                            .FindPropertyRelative(nameof(_alwaysEvents)).GetArrayElementAtIndex(i);
                        alwaysEvents[i].SyncUnityEventHeading(serializedConditionResponse);
                    }
                    break;

                case ConditionResponseTypes.Bool:
                    for (int i = 0; i < boolEvents.Count; i++) {
                        SerializedProperty serializedConditionResponse = unityEventParentProperty
                            .FindPropertyRelative(nameof(_boolEvents)).GetArrayElementAtIndex(i);
                        boolEvents[i].SyncUnityEventHeading(serializedConditionResponse);
                    }
                    break;

                case ConditionResponseTypes.Float:
                    for (int i = 0; i < floatEvents.Count; i++) {
                        SerializedProperty serializedConditionResponse = unityEventParentProperty
                            .FindPropertyRelative(nameof(_floatEvents)).GetArrayElementAtIndex(i);
                        floatEvents[i].SyncUnityEventHeading(serializedConditionResponse);
                    }
                    break;

                case ConditionResponseTypes.Int:
                    for (int i = 0; i < intEvents.Count; i++) {
                        SerializedProperty serializedConditionResponse = unityEventParentProperty
                            .FindPropertyRelative(nameof(_intEvents)).GetArrayElementAtIndex(i);
                        intEvents[i].SyncUnityEventHeading(serializedConditionResponse);
                    }
                    break;

                case ConditionResponseTypes.TextFamily:
                    for (int i = 0; i < textFamilyEvents.Count; i++) {
                        SerializedProperty serializedConditionResponse = unityEventParentProperty
                            .FindPropertyRelative(nameof(_textFamilyEvents)).GetArrayElementAtIndex(i);
                        textFamilyEvents[i].SyncUnityEventHeading(serializedConditionResponse);
                    }
                    break;

                case ConditionResponseTypes.Layout:
                    for (int i = 0; i < layoutEvents.Count; i++) {
                        SerializedProperty serializedConditionResponse = unityEventParentProperty
                            .FindPropertyRelative(nameof(_layoutEvents)).GetArrayElementAtIndex(i);
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
            
            if (currentIndex > 0 && currentIndex < listCount - 1) {
                return "ELSE IF";
            }

            return "ELSE";
        }

#if UNITY_EDITOR
        void DisplayClearDialogue()
        {
            if (EditorUtility.DisplayDialog("Clear unused values?", "You just changed the condition response type. Would you like to erase unused, hidden values?", "Yes", "No")) {
                ClearHiddenValues();
            }
        }

        [HorizontalGroup("Split", 1f)]
        [InfoBox("Clear any values that are not currently being used by the trigger")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void ClearHiddenValues()
        {
            if(triggerType != ConditionResponseTypes.Always) {
                alwaysEvents.Clear();
            }

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

        private bool IsPopulated(List<AlwaysConditionResponse> attribute)
        {
            if(triggerType == ConditionResponseTypes.Always) {
                if(attribute.Count > 0) {
                    bool isValid = true;
                    for(int i=0; i<attribute.Count; i++) {
                        if (Utils.IsPopulated(attribute[i].response) == false) {
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

        private bool IsPopulated(List<BoolConditionResponse> attribute)
        {
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

                        if (Utils.IsPopulated(attribute[i].response) == false) {
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

                        if (Utils.IsPopulated(attribute[i].response) == false) {
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

                        if (Utils.IsPopulated(attribute[i].response) == false) {
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

                        if (Utils.IsPopulated(attribute[i].response) == false) {
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

                        if (Utils.IsPopulated(attribute[i].response) == false) {
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

#endif

    }
}