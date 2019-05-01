/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using TMPro;

namespace AltSalt
{

#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class LocalizedText : MonoBehaviour
    {
        [Required]
        public AppSettings appSettings;

        [Required]
        public string key;

        [Required]
        [SerializeField]
        public LocalizationCorpus localizationCorpus;

        TMP_Text textComponent;

        void Start()
        {
            if(appSettings.localizationActive == true) {
                PopulateWithText();
            }
        }

        void GetTextComponent()
        {
            if (textComponent == null) {
                textComponent = GetComponent<TMP_Text>();
            }
        }

        [HorizontalGroup("Split", 0.5f)]
        [InfoBox("Gets text according to active language, corpus, and key.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void PopulateWithText()
        {
            GetTextComponent();
            textComponent.SetText(localizationCorpus.GetText(appSettings.activeLanguage, key), true);
        }

        public void RefreshText(EventPayload eventPayload)
        {
            if(localizationCorpus == null) {
                Debug.Log("No localization corpus found!", this);
                return;
            }
            LocalizationCorpus targetCorpus = eventPayload.GetScriptableObjectValue(EventPayloadType.scriptableObjectPayload.ToString()) as LocalizationCorpus;
            if (targetCorpus == localizationCorpus) {
                PopulateWithText();
            }
        }

#if UNITY_EDITOR
        void Reset()
        {
            //Uncomment these lines to fix repopulate AppSettings on responsive objects
            //if the values are lost for some reason
            if (appSettings == null) {
                appSettings = Utils.GetAppSettings();
            }
        }
#endif
    }
}