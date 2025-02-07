﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro.Logic
{
    [ExecuteInEditMode]
    public class UserDataController : MonoBehaviour
    {
        [SerializeField]
        [Required]
        [ReadOnly]
        private AppSettingsReference _appSettings = new AppSettingsReference();

        private AppSettings appSettings => _appSettings.GetVariable() as AppSettings;

        [FormerlySerializedAs("userData"),SerializeField]
        private List<UserDatum> _userData = new List<UserDatum>();

        private List<UserDatum> userData => _userData;

        private void Awake()
        {
#if UNITY_EDITOR
            _appSettings.PopulateVariable(this, nameof(_appSettings));
            
            string userDataListPath = nameof(_userData);
            for (int i = 0; i < userData.Count; i++) {
                string userPataPath = $"{userDataListPath}.{i}";
                userData[i].PopulateReference(this, userPataPath);
            }
#endif
            CallReadFromStoredData();
        }

        public void CallWriteToStoredData(ComplexPayload complexPayload)
        {
            if(appSettings.saveDataActive == false) {
                return;
            }

            foreach (KeyValuePair<object, ScriptableObject> payloadObject in complexPayload.scriptableObjectDictionary) {
                ScriptableObject receivedObject = payloadObject.Value;
                bool valueFound = false;
                for (int j = 0; j < _userData.Count; j++) {
                    if (receivedObject == _userData[j].scriptableObject) {
                        valueFound = true;
                        WriteToStoredData(_userData[j]);
                    }
                }
                if(valueFound == false) {
                    Debug.LogError("Data unable to be written - " + receivedObject.name + " variable not stored in " + this.name, this);
                }
            }
        }

        [InfoBox("Save target scriptable object value to Player Prefs.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void CallWriteToStoredData(ScriptableObject receivedObject)
        {
            bool valueFound = false;
            for (int i = 0; i < _userData.Count; i++) {
                if (receivedObject == _userData[i].scriptableObject) {
                    valueFound = true;
                    WriteToStoredData(_userData[i]);
                }
            }

            if (valueFound == false) {
                Debug.LogError("Data unable to be written - " + receivedObject.name + " variable not stored in " + this.name, this);
            }
        }

        private void WriteToStoredData(UserDatum userDatum)
        {
            switch (userDatum.scriptableObject) {

                case BoolVariable boolVariable:
                    int boolIntValue = boolVariable.value == true ? 1 : 0;
                    PlayerPrefs.SetInt(boolVariable.name, boolIntValue);
#if UNITY_EDITOR
                    string boolString = boolIntValue == 1 ? "true" : "false";
                    LogSavedMessage(boolVariable.name, boolString);
#endif
                    break;

                case FloatVariable floatVariable:
                    PlayerPrefs.SetFloat(floatVariable.name, floatVariable.value);
#if UNITY_EDITOR
                    LogSavedMessage(floatVariable.name, floatVariable.value.ToString("F6"));
#endif
                    break;


                case StringVariable stringVariable:
                    PlayerPrefs.SetString(stringVariable.name, stringVariable.value);
#if UNITY_EDITOR
                    LogSavedMessage(stringVariable.name, stringVariable.value);
#endif
                    break;

                case IntVariable intVariable:
                    PlayerPrefs.SetInt(intVariable.name, intVariable.value);
#if UNITY_EDITOR
                    LogSavedMessage(intVariable.name, intVariable.value.ToString("F6"));
#endif
                    break;
                
                default:
                    Debug.LogError("Type not recognized; unable to store data", this);
                    break;
            }
        }

        public void CallReadFromStoredData()
        {
            if(appSettings.saveDataActive == false) {
                return;
            }
            
            for (int i = 0; i < _userData.Count; i++) {
                ReadFromStoredData(_userData[i]);
            }
        }

        public void CallReadFromStoredData(ScriptableObject sourceObject)
        {
            if(appSettings.saveDataActive == false) {
                return;
            }
            
            bool valueFound = false;
            for (int i = 0; i < _userData.Count; i++) {
                if (sourceObject == _userData[i].scriptableObject) {
                    valueFound = true;
                    ReadFromStoredData(_userData[i]);
                }
            }

            if (valueFound == false) {
                Debug.LogError("Data unable to be read - " + sourceObject.name + " variable not stored in " + this.name, this);
            }
        }

        private void ReadFromStoredData(UserDatum userDatum)
        {
            
            switch (userDatum.scriptableObject) {

                case BoolVariable boolVariable:
                    int boolIntValue = PlayerPrefs.GetInt(boolVariable.name);
                    boolVariable.StoreCaller(this.gameObject);
                    if(boolIntValue == 1) {
                        boolVariable.SetValue(true);
                    } else if (boolIntValue == 0) {
                        boolVariable.SetValue(false);
                    }
                    break;

                case FloatVariable floatVariable:
                    float floatValue = PlayerPrefs.GetFloat(floatVariable.name);
                    floatVariable.StoreCaller(this.gameObject);
                    floatVariable.SetValue(floatValue);
                    break;


                case StringVariable stringVariable:
                    string stringValue = PlayerPrefs.GetString(stringVariable.name);
                    stringVariable.StoreCaller(this.gameObject);
                    stringVariable.SetValue(stringValue);
                    break;

                case IntVariable intVariable:
                    int intValue = PlayerPrefs.GetInt(intVariable.name);
                    intVariable.StoreCaller(this.gameObject);
                    intVariable.SetValue(intValue);
                    break;
            }
        }
        
        [InfoBox("Print values of user data stored in Player Prefs.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void PrintPlayerPrefs()
        {
            for (int i = 0; i < userData.Count; i++) {
                LogFromStoredData(userData[i]);
            }
        }

        [InfoBox("Delete values of user data from this object stored in Player Prefs.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void DeleteStoredPlayerPrefs()
        {
            for (int i = 0; i < userData.Count; i++) {
                PlayerPrefs.DeleteKey(userData[i].scriptableObject.name);
                Debug.Log("Deleted " + userData[i].scriptableObject.name + " value from Player Prefs.");
            }
        }

        private void LogFromStoredData(UserDatum userDatum)
        {
            switch (userDatum.scriptableObject) {

                case BoolVariable boolVariable:
                    int boolIntValue = PlayerPrefs.GetInt(boolVariable.name);
                    string boolString = "";
                    if (boolIntValue == 1) {
                        boolString = "TRUE";
                    } else if (boolIntValue == 0) {
                        boolString = "FALSE";
                    } else {
                        boolString = "UNDEFINED";
                    }
                    Debug.Log("Value of " + boolVariable.name + " variable in stored data is " + boolString);
                    break;

                case FloatVariable floatVariable:
                    float floatValue = PlayerPrefs.GetFloat(floatVariable.name);
                    Debug.Log("Value of " + floatVariable.name + " variable in stored data is " + floatValue);
                    break;

                case StringVariable stringVariable:
                    string stringValue = PlayerPrefs.GetString(stringVariable.name);
                    Debug.Log("Value of " + stringVariable.name + " variable in stored data is " + stringValue);
                    break;

                case IntVariable intVariable:
                    int intValue = PlayerPrefs.GetInt(intVariable.name);
                    Debug.Log("Value of " + intVariable.name + " variable in stored data is " + intValue);
                    break;
            }
        }

        [InfoBox("Delete target scriptable object value from Player Prefs.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void DeleteFromStoredData(ScriptableObject scriptableObject)
        {
            PlayerPrefs.DeleteKey(scriptableObject.name);
            Debug.Log("Deleted " + scriptableObject.name + " value from Player Prefs.");
        }

        void LogSavedMessage(string attributeName, string value)
        {
            Debug.Log("Saved " + attributeName + " value of " + value + " to Player Prefs.");
        }

        [InfoBox("Delete all stored data in Player Prefs. Use with caution.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void DeleteAllPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }

        [Serializable]
        public class UserDatum
        {
            [SerializeField]
            private ScriptableObjectReference _scriptableObject = new ScriptableObjectReference();

            public ScriptableObject scriptableObject
            {
                get => _scriptableObject.GetVariable();
                set => _scriptableObject.SetVariable(value);
            }
            
#if UNITY_EDITOR
            public void PopulateReference(Object parentObject, string serializedPropertyPath)
            {
                _scriptableObject.PopulateVariable(parentObject, $"{serializedPropertyPath}.{nameof(_scriptableObject)}");
            }
#endif            
        }
    }

}