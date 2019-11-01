using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AltSalt
{

    [ExecuteInEditMode]
    public class ModifyHandler : MonoBehaviour
    {
#if UNITY_EDITOR
        string activeLayoutName;
        string activeTextFamilyName;
#endif
        [Required]
        [Title("$activeTextFamilyName")]
        [Title("$activeLayoutName")]
        [SerializeField]
        ModifySettings modifySettings;

        [Required]
        [SerializeField]
        CustomKey textFamilyKey;

        [Required]
        [SerializeField]
        CustomKey layoutKey;

        [Required]
        public ComplexEventTrigger textUpdateTrigger;

        [Required]
        public SimpleEventTrigger layoutUpdateTrigger;

#if UNITY_EDITOR
        void OnGUI()
        {
            activeLayoutName = "Current layout: " + modifySettings.activeLayout.name;
            activeTextFamilyName = "Current text family: " + modifySettings.activeTextFamily.name;
        }

        public void Reset()
        {
            if (modifySettings == null) {
                modifySettings = Utils.GetModifySettings();
            }

            if(textFamilyKey == null) {
                textFamilyKey = Utils.GetScriptableObject("TextFamilyKey") as CustomKey;
            }

            if (layoutKey == null) {
                textFamilyKey = Utils.GetScriptableObject("LayoutKey") as CustomKey;
            }
        }
#endif

        public void TriggerTextModify(EventPayload eventPayload)
        {
            modifySettings.activeTextFamily = eventPayload.GetScriptableObjectValue(DataType.scriptableObjectType) as TextFamily;
            TextCollectionBank textCollectionBank = eventPayload.GetScriptableObjectValue(DataType.scriptableObjectType) as TextCollectionBank;
            TriggerTextUpdate(textCollectionBank);

            if(modifySettings.activeTextFamily.supportedLayouts.Count == 0) {
                modifySettings.activeLayout = modifySettings.defaultLayout;
                layoutUpdateTrigger.RaiseEvent(this.gameObject);
            } else {
                bool triggerLayoutChange = true;
                for(int i=0; i<modifySettings.activeTextFamily.supportedLayouts.Count; i++) {
                    if(modifySettings.activeLayout == modifySettings.activeTextFamily.supportedLayouts[i]) {
                        triggerLayoutChange = false;
                    }
                }
                if(triggerLayoutChange == true) {
                    modifySettings.activeLayout = modifySettings.activeTextFamily.supportedLayouts[0];
                    layoutUpdateTrigger.RaiseEvent(this.gameObject);
                }
            }
        }

        public void TriggerLayoutModify(EventPayload eventPayload)
        {
            modifySettings.activeLayout = eventPayload.GetScriptableObjectValue(DataType.scriptableObjectType) as Layout;
            TriggerLayoutUpdate();

            if (modifySettings.activeLayout.supportedTextFamilies.Count == 0) {

                modifySettings.activeTextFamily = modifySettings.defaultTextFamily;
                textUpdateTrigger.RaiseEvent(this.gameObject);

            } else {
                bool triggerLayoutChange = true;
                for (int i = 0; i < modifySettings.activeTextFamily.supportedLayouts.Count; i++) {
                    if (modifySettings.activeTextFamily == modifySettings.activeLayout.supportedTextFamilies[i]) {
                        triggerLayoutChange = false;
                    }
                }
                if (triggerLayoutChange == true) {
                    modifySettings.activeTextFamily = modifySettings.activeLayout.supportedTextFamilies[0];
                    textUpdateTrigger.RaiseEvent(this.gameObject);
                }
            }
        }

        public void TriggerTextUpdate(TextCollectionBank targetBank)
        {
            if(targetBank != null) {
                textUpdateTrigger.RaiseEvent(this.gameObject, targetBank);
            } else {
                textUpdateTrigger.RaiseEvent(this.gameObject);
            }
        }

        public void TriggerLayoutUpdate()
        {
            layoutUpdateTrigger.RaiseEvent(this.gameObject);
        }

        private static bool IsPopulated(List<TextCollectionBank> attribute) {
            return Utils.IsPopulated(attribute);
        }

    }
}