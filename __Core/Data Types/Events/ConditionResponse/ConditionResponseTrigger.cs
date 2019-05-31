using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using System.Reflection;

namespace AltSalt
{
    [Serializable]
    [ExecuteInEditMode]
    public class ConditionResponseTrigger
    {
        [Serializable]
        enum ConditionResponseTypes { Always, Bool, Float, Int, TextFamily, Layout }

        [SerializeField]
        ConditionResponseTypes triggerType;

        [SerializeField]
        [ShowIf("triggerType", ConditionResponseTypes.Always)]
        [Title("Always Event List")]
        List<AlwaysConditionResponse> alwaysEvents = new List<AlwaysConditionResponse>();

        [SerializeField]
        [ShowIf("triggerType", ConditionResponseTypes.Bool)]
        [Title("Bool Event List")]
        List<BoolConditionResponse> boolEvents = new List<BoolConditionResponse>();

        [SerializeField]
        [ShowIf("triggerType", ConditionResponseTypes.Float)]
        [Title("Float Event List")]
        List<FloatConditionResponse> floatEvents = new List<FloatConditionResponse>();

        [SerializeField]
        [ShowIf("triggerType", ConditionResponseTypes.TextFamily)]
        [Title("Text Family Event List")]
        List<TextFamilyConditionResponse> textFamilyEvents = new List<TextFamilyConditionResponse>();

        [SerializeField]
        [ShowIf("triggerType", ConditionResponseTypes.Layout)]
        [Title("Layout Event List")]
        List<LayoutConditionResponse> layoutEvents = new List<LayoutConditionResponse>();

        [PropertySpace]

        [HorizontalGroup("Split", 1f)]
        [InfoBox("Trigger the list of events")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void TriggerAllResponses()
        {
            switch (triggerType) {

                case ConditionResponseTypes.Always:
                    for (int i = 0; i < alwaysEvents.Count; i++) {
                        alwaysEvents[i].TriggerResponse();
                    }
                    break;

                case ConditionResponseTypes.Bool:
                    for (int i = 0; i < boolEvents.Count; i++) {
                        boolEvents[i].TriggerResponse();
                    }
                    break;

                case ConditionResponseTypes.Float:
                    for (int i = 0; i < floatEvents.Count; i++) {
                        floatEvents[i].TriggerResponse();
                    }
                    break;

                case ConditionResponseTypes.TextFamily:
                    for (int i = 0; i < textFamilyEvents.Count; i++) {
                        textFamilyEvents[i].TriggerResponse();
                    }
                    break;

                case ConditionResponseTypes.Layout:
                    for (int i = 0; i < layoutEvents.Count; i++) {
                        layoutEvents[i].TriggerResponse();
                    }
                    break;
            }
        }

        [HorizontalGroup("Split", 1f)]
        [InfoBox("Trigger the list of events")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void TriggerUntilFirstSuccess()
        {
            switch (triggerType) {

                case ConditionResponseTypes.Always:
                    for (int i = 0; i < alwaysEvents.Count; i++) {
                        if(alwaysEvents[i].CheckCondition() == true) {
                            alwaysEvents[i].TriggerResponse();
                            return;
                        }
                    }
                    break;

                case ConditionResponseTypes.Bool:
                    for (int i = 0; i < boolEvents.Count; i++) {
                        if(boolEvents[i].CheckCondition() == true) {
                            boolEvents[i].TriggerResponse();
                            return;
                        }
                    }
                    break;

                case ConditionResponseTypes.Float:
                    for (int i = 0; i < floatEvents.Count; i++) {
                        if(floatEvents[i].CheckCondition() == true) {
                            floatEvents[i].TriggerResponse();
                            return;
                        }
                    }
                    break;

                case ConditionResponseTypes.TextFamily:
                    for (int i = 0; i < textFamilyEvents.Count; i++) {
                        if (textFamilyEvents[i].CheckCondition() == true) {
                            textFamilyEvents[i].TriggerResponse();
                            return;
                        }
                    }
                    break;

                case ConditionResponseTypes.Layout:
                    for (int i = 0; i < layoutEvents.Count; i++) {
                        if(layoutEvents[i].CheckCondition() == true) {
                            layoutEvents[i].TriggerResponse();
                            return;
                        }
                    }
                    break;
            }
        }

#if UNITY_EDITOR
        public void CallSyncValues()
        {
            switch (triggerType) {

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
#endif

    }
}