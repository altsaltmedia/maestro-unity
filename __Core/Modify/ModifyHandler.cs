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

#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class ModifyHandler : MonoBehaviour
    {
        [Required]
        [SerializeField]
        ModifySettings modifySettings;

        [Required]
        [SerializeField]
        EventPayloadKey languageKey;

        [Required]
        [SerializeField]
        EventPayloadKey layoutKey;

        [SerializeField]
        [ValidateInput("IsPopulated")]
        List<LocalizationCorpus> localizationCorpora = new List<LocalizationCorpus>();

        [Required]
        public ComplexEvent languageUpdate;

#if UNITY_EDITOR
        public void Reset()
        {
            if (modifySettings == null) {
                modifySettings = Utils.GetModifySettings();
            }

            if(languageKey == null) {
                languageKey = Utils.GetScriptableObject("LanguageKey") as EventPayloadKey;
            }

            if (layoutKey == null) {
                languageKey = Utils.GetScriptableObject("LayoutKey") as EventPayloadKey;
            }
        }
#endif

        public void TriggerModify(EventPayload eventPayload)
        {
            modifySettings.activeLanguage = eventPayload.GetScriptableObjectValue(EventPayloadType.scriptableObjectPayload) as Language;
            TriggerLanguageUpdate();
            if(modifySettings.activeLanguage.layouts.Count == 0) {
                modifySettings.activeLayout = modifySettings.defaultLayout;
            } else {
                bool triggerLayoutChange = true;
                for(int i=0; i<modifySettings.activeLanguage.layouts.Count; i++) {
                    if(modifySettings.activeLayout == modifySettings.activeLanguage.layouts[i]) {
                        triggerLayoutChange = false;
                    }
                }
                if(triggerLayoutChange == true) {
                    modifySettings.activeLayout = modifySettings.activeLanguage.layouts[0];
                }
            }
        }

        [HorizontalGroup("Split", 0.5f)]
        [InfoBox("Gets text according to active language, corpus, and key.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void TriggerLanguageUpdate()
        {
            for (int i = 0; i < localizationCorpora.Count; i++) {
                languageUpdate.Raise(localizationCorpora[i]);
            }
        }
        private static bool IsPopulated(List<LocalizationCorpus> attribute) {
            return Utils.IsPopulated(attribute);
        }

    }
}