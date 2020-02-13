/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AltSalt.Maestro
{

    [ExecuteInEditMode]
    public class TextLoader : MonoBehaviour
    {
        [SerializeField]
        [Required]
        [ReadOnly]
        private AppSettingsReference _appSettings = new AppSettingsReference();

        private AppSettings appSettings => _appSettings.GetVariable() as AppSettings;

        [Required]
        [SerializeField]
        public string key;

        [FormerlySerializedAs("textCollectionBank"),Required]
        [SerializeField]
#if UNITY_EDITOR
        [Header("$"+nameof(GetActiveTextFamilyName))]
#endif
        private TextCollectionBank _textCollectionBank;

        public TextCollectionBank textCollectionBank => _textCollectionBank;

        private TMP_Text _textComponent;

        private TMP_Text textComponent {
            get
            {
                if (_textComponent == null) {
                    _textComponent = GetComponent<TMP_Text>();
                }

                return _textComponent;
            }
            set => _textComponent = value;
        }

        private void Awake()
        {
#if UNITY_EDITOR
            _appSettings.PopulateVariable(this, nameof(_appSettings));
#endif
            textComponent = GetComponent<TMP_Text>();
        }

#if UNITY_EDITOR
        private string GetActiveTextFamilyName()
        {
            if (textCollectionBank != null) {
                return "Active Text Family: " + textCollectionBank.GetActiveTextFamilyName();
            }

            return "Please populate a text collection bank";
        }
#endif
        
        private void Start()
        {
            if(appSettings.modifyTextActive == true && textCollectionBank != null) {
                PopulateWithText();
            }
        }

        [InfoBox("Gets text according to active text family, text collection bank, and key.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void PopulateWithText()
        {
            if(textCollectionBank != null) {
#if UNITY_EDITOR
                Undo.RecordObject(textComponent, "populate text");
#endif
                if (textCollectionBank.GetText(key, out string text)) {
                    textComponent.SetText(text, true);    
                }
                else {
                    Debug.Log(text, this);
                }
            }
        }

        public void RefreshText(ComplexPayload complexPayload)
        {
            if(textCollectionBank == null) {
                return;
            }
            TextCollectionBank targetTextBank = complexPayload.GetScriptableObjectValue(DataType.scriptableObjectType) as TextCollectionBank;

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

    }
}