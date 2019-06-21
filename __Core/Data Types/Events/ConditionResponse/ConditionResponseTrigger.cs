using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AltSalt
{
    [Serializable]
    [ExecuteInEditMode]
    public class ConditionResponseTrigger : ConditionResponseTriggerBase, IClearHiddenValues
    {
        [Serializable]
        enum ConditionResponseTypes { Always, Bool, Float, Int, TextFamily, Layout }

        [SerializeField]
        [OnValueChanged("DisplayClearDialogue")]
        ConditionResponseTypes triggerType;

        [SerializeField]
        [ShowIf("triggerType", ConditionResponseTypes.Always)]
        [Title("Always Event List")]
        [ValidateInput("IsPopulated")]
        List<AlwaysConditionResponse> alwaysEvents = new List<AlwaysConditionResponse>();

        [SerializeField]
        [ShowIf("triggerType", ConditionResponseTypes.Bool)]
        [Title("Bool Event List")]
        [ValidateInput("IsPopulated")]
        List<BoolConditionResponse> boolEvents = new List<BoolConditionResponse>();

        [SerializeField]
        [ShowIf("triggerType", ConditionResponseTypes.Float)]
        [Title("Float Event List")]
        [ValidateInput("IsPopulated")]
        List<FloatConditionResponse> floatEvents = new List<FloatConditionResponse>();

        [SerializeField]
        [ShowIf("triggerType", ConditionResponseTypes.TextFamily)]
        [Title("Text Family Event List")]
        [ValidateInput("IsPopulated")]
        List<TextFamilyConditionResponse> textFamilyEvents = new List<TextFamilyConditionResponse>();

        [SerializeField]
        [ShowIf("triggerType", ConditionResponseTypes.Layout)]
        [Title("Layout Event List")]
        [ValidateInput("IsPopulated")]
        List<LayoutConditionResponse> layoutEvents = new List<LayoutConditionResponse>();

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

        public void CallSyncValues()
        {
            switch (triggerType) {

                case ConditionResponseTypes.Always:
                    for (int i = 0; i < alwaysEvents.Count; i++) {
                        alwaysEvents[i].SyncValues();
                    }
                    break;

                case ConditionResponseTypes.Bool:
                    for (int i = 0; i < boolEvents.Count; i++) {
                        boolEvents[i].SyncValues();
                    }
                    break;

                case ConditionResponseTypes.Float:
                    for (int i = 0; i < floatEvents.Count; i++) {
                        floatEvents[i].SyncValues();
                    }
                    break;

                case ConditionResponseTypes.TextFamily:
                    for (int i = 0; i < textFamilyEvents.Count; i++) {
                        textFamilyEvents[i].SyncValues();
                    }
                    break;

                case ConditionResponseTypes.Layout:
                    for (int i = 0; i < layoutEvents.Count; i++) {
                        layoutEvents[i].SyncValues();
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
                // Int condition response not yet implemented
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

        private bool IsPopulated(List<TextFamilyConditionResponse> attribute)
        {
            if (triggerType == ConditionResponseTypes.TextFamily) {
                if (attribute.Count > 0) {
                    bool isValid = true;
                    for (int i = 0; i < attribute.Count; i++) {
                        if (attribute[i].GetCondition() == null) {
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
                        if (attribute[i].GetCondition() == null) {
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