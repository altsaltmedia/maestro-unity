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
        AppSettings appSettings;

        [SerializeField]
        [ValidateInput("IsPopulated")]
        List<LocalizationCorpus> localizationCorpora = new List<LocalizationCorpus>();

        [Required]
        public ComplexEvent languageUpdate;

        public void Reset()
        {
            if (appSettings == null) {
                appSettings = Utils.GetAppSettings();
            }
        }

        public void TriggerModify(EventPayload eventPayload)
        {
            appSettings.activeLanguage = eventPayload.GetScriptableObjectValue(EventPayloadType.scriptableObjectPayload.ToString()) as Language;
            TriggerLanguageUpdate();
        }

#if UNITY_EDITOR
        [HorizontalGroup("Split", 0.5f)]
        [InfoBox("Gets text according to active language, corpus, and key.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void TriggerLanguageUpdate()
        {
            for (int i = 0; i < localizationCorpora.Count; i++) {
                languageUpdate.Raise(localizationCorpora[i]);
            }
        }
#endif
        private static bool IsPopulated(List<LocalizationCorpus> attribute) {
            return Utils.IsPopulated(attribute);
        }

    }
}