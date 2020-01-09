using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using System.Reflection;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AltSalt.Maestro.Logic.ConditionResponse
{
    [Serializable]
    [ExecuteInEditMode]
    public class ConditionResponseTrigger : ConditionResponseTriggerBase, IClearHiddenValues
    {
        [Serializable]
        private enum ConditionResponseTypes { Always, Bool, Float, Int, TextFamily, Layout }

        [SerializeField]
        [OnValueChanged("DisplayClearDialogue")]
        [FormerlySerializedAs("triggerType")]
        private ConditionResponseTypes _triggerType;

        private ConditionResponseTypes triggerType => _triggerType;

        [SerializeField]
        [ShowIf("triggerType", ConditionResponseTypes.Always)]
        [Title("Always Event List")]
        [ValidateInput("IsPopulated")]
        [FormerlySerializedAs("alwaysEvents")]
        private List<AlwaysConditionResponse> _alwaysEvents = new List<AlwaysConditionResponse>();

        private List<AlwaysConditionResponse> alwaysEvents => _alwaysEvents;

        [SerializeField]
        [ShowIf("triggerType", ConditionResponseTypes.Bool)]
        [Title("Bool Event List")]
        [ValidateInput("IsPopulated")]
        [FormerlySerializedAs("boolEvents")]
        private List<BoolConditionResponse> _boolEvents = new List<BoolConditionResponse>();

        private List<BoolConditionResponse> boolEvents => _boolEvents;

        [SerializeField]
        [ShowIf("triggerType", ConditionResponseTypes.Float)]
        [Title("Float Event List")]
        [ValidateInput("IsPopulated")]
        [FormerlySerializedAs("floatEvents")]
        private List<FloatConditionResponse> _floatEvents = new List<FloatConditionResponse>();

        private List<FloatConditionResponse> floatEvents => _floatEvents;

        [SerializeField]
        [ShowIf("triggerType", ConditionResponseTypes.Int)]
        [Title("Int Event List")]
        [ValidateInput("IsPopulated")]
        [FormerlySerializedAs("intEvents")]
        private List<IntConditionResponse> _intEvents = new List<IntConditionResponse>();

        private List<IntConditionResponse> intEvents => _intEvents;

        [SerializeField]
        [ShowIf("triggerType", ConditionResponseTypes.TextFamily)]
        [Title("Text Family Event List")]
        [ValidateInput("IsPopulated")]
        [FormerlySerializedAs("textFamilyEvents")]
        private List<TextFamilyConditionResponse> _textFamilyEvents = new List<TextFamilyConditionResponse>();

        private List<TextFamilyConditionResponse> textFamilyEvents => _textFamilyEvents;

        [SerializeField]
        [ShowIf("triggerType", ConditionResponseTypes.Layout)]
        [Title("Layout Event List")]
        [ValidateInput("IsPopulated")]
        [FormerlySerializedAs("layoutEvents")]
        private List<LayoutConditionResponse> _layoutEvents = new List<LayoutConditionResponse>();

        private List<LayoutConditionResponse> layoutEvents => _layoutEvents;

        [PropertySpace]

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
                        if (alwaysEvents[i].CheckCondition() == true) {
                            alwaysEvents[i].TriggerResponse(caller, triggerOnStart);
                            return;
                        }
                    }
                    break;

                case ConditionResponseTypes.Bool:
                    for (int i = 0; i < boolEvents.Count; i++) {
                        if (boolEvents[i].CheckCondition() == true) {
                            boolEvents[i].TriggerResponse(caller, triggerOnStart);
                            return;
                        }
                    }
                    break;

                case ConditionResponseTypes.Float:
                    for (int i = 0; i < floatEvents.Count; i++) {
                        if (floatEvents[i].CheckCondition() == true) {
                            floatEvents[i].TriggerResponse(caller, triggerOnStart);
                            return;
                        }
                    }
                    break;

                case ConditionResponseTypes.Int:
                    for (int i = 0; i < intEvents.Count; i++) {
                        if (intEvents[i].CheckCondition() == true) {
                            intEvents[i].TriggerResponse(caller, triggerOnStart);
                            return;
                        }
                    }
                    break;

                case ConditionResponseTypes.TextFamily:
                    for (int i = 0; i < textFamilyEvents.Count; i++) {
                        if (textFamilyEvents[i].CheckCondition() == true) {
                            textFamilyEvents[i].TriggerResponse(caller, triggerOnStart);
                            return;
                        }
                    }
                    break;

                case ConditionResponseTypes.Layout:
                    for (int i = 0; i < layoutEvents.Count; i++) {
                        if (layoutEvents[i].CheckCondition() == true) {
                            layoutEvents[i].TriggerResponse(caller, triggerOnStart);
                            return;
                        }
                    }
                    break;
            }
        }

        public void CallSyncValues(UnityEngine.Object callingObject)
        {
            switch (triggerType) {

                case ConditionResponseTypes.Always:
                    for (int i = 0; i < alwaysEvents.Count; i++) {
                        alwaysEvents[i].SyncValues(callingObject);
                    }
                    break;

                case ConditionResponseTypes.Bool:
                    for (int i = 0; i < boolEvents.Count; i++) {
                        boolEvents[i].SyncValues(callingObject);
                    }
                    break;

                case ConditionResponseTypes.Float:
                    for (int i = 0; i < floatEvents.Count; i++) {
                        floatEvents[i].SyncValues(callingObject);
                    }
                    break;

                case ConditionResponseTypes.Int:
                    for (int i = 0; i < intEvents.Count; i++) {
                        intEvents[i].SyncValues(callingObject);
                    }
                    break;

                case ConditionResponseTypes.TextFamily:
                    for (int i = 0; i < textFamilyEvents.Count; i++) {
                        textFamilyEvents[i].SyncValues(callingObject);
                    }
                    break;

                case ConditionResponseTypes.Layout:
                    for (int i = 0; i < layoutEvents.Count; i++) {
                        layoutEvents[i].SyncValues(callingObject);
                    }
                    break;
            }
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
                        if (Utils.IsPopulated(attribute[i].GetResponse()) == false) {
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

                        if (Utils.IsPopulated(attribute[i].GetResponse()) == false) {
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

                        if (Utils.IsPopulated(attribute[i].GetResponse()) == false) {
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

                        if (Utils.IsPopulated(attribute[i].GetResponse()) == false) {
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

                        if (Utils.IsPopulated(attribute[i].GetResponse()) == false) {
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

                        if (Utils.IsPopulated(attribute[i].GetResponse()) == false) {
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