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
    public class TextLoader : MonoBehaviour
    {
        [Required]
        public AppSettings appSettings;

        [Required]
        public ModifySettings modifySettings;

        [Required]
        public string key;

        [Required]
        [SerializeField]
        public TextCollectionBank textCollectionBank;

        TMP_Text textComponent;

        void Start()
        {
            if(appSettings.modifyTextActive == true && textCollectionBank != null) {
                PopulateWithText();
            }
        }

        void GetTextComponent()
        {
            if (textComponent == null) {
                textComponent = GetComponent<TMP_Text>();
            }
        }

        [InfoBox("Gets text according to active text family, text collection bank, and key.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void PopulateWithText()
        {
            GetTextComponent();
            if(textCollectionBank != null) {
                textComponent.SetText(textCollectionBank.GetText(modifySettings.activeTextFamily, key), true);
            }
        }

        public void RefreshText(EventPayload eventPayload)
        {
            if(textCollectionBank == null) {
                return;
            }
            TextCollectionBank targetTextBank = eventPayload.GetScriptableObjectValue(DataType.scriptableObjectType) as TextCollectionBank;

            // If the textUpdate event was raised without a text bank, then update the text.
            // Otherwise, check to see if this text's bank matches the one slated for update.
            if(targetTextBank == null) {
                PopulateWithText();
            } else {
                if (targetTextBank == textCollectionBank) {
                    PopulateWithText();
                }
            }
        }

#if UNITY_EDITOR
        void Reset()
        {
            if (appSettings == null) {
                appSettings = Utils.GetAppSettings();
            }
        }
#endif
    }
}