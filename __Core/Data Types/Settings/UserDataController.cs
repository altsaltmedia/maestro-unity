using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [ExecuteInEditMode]
    public class UserDataController : MonoBehaviour
    {
        [SerializeField]
        List<UserDatum> userData = new List<UserDatum>();

        public void CallWriteToStoredData(EventPayload eventPayload)
        {
            ScriptableObject receivedObject = eventPayload.GetScriptableObjectValue(DataType.scriptableObjectType);
            bool valueFound = false;
            for (int i = 0; i < userData.Count; i++) {
                if (receivedObject == userData[i].scriptableObject) {
                    valueFound = true;
                    WriteToStoredData(userData[i]);
                }
            }

            if(valueFound == false) {
                Debug.LogError("Data unable to be written - " + receivedObject.name + " variable not stored in " + this.name, this);
            }
        }

        [InfoBox("Save target scriptable object value to Player Prefs.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void CallWriteToStoredData(ScriptableObject receivedObject)
        {
            bool valueFound = false;
            for (int i = 0; i < userData.Count; i++) {
                if (receivedObject == userData[i].scriptableObject) {
                    valueFound = true;
                    WriteToStoredData(userData[i]);
                }
            }

            if (valueFound == false) {
                Debug.LogError("Data unable to be written - " + receivedObject.name + " variable not stored in " + this.name, this);
            }
        }

        void WriteToStoredData(UserDatum userDatum)
        {
            switch (userDatum.dataType) {

                case DataType.boolType:
                    BoolVariable boolVariable = userDatum.scriptableObject as BoolVariable;
                    int boolIntValue = boolVariable.Value == true ? 1 : 0;
                    PlayerPrefs.SetInt(boolVariable.name, boolIntValue);
#if UNITY_EDITOR
                    string boolString = boolIntValue == 1 ? "true" : "false";
                    LogSavedMessage(boolVariable.name, boolString);
#endif
                    break;

                case DataType.floatType:
                    FloatVariable floatVariable = userDatum.scriptableObject as FloatVariable;
                    PlayerPrefs.SetFloat(floatVariable.name, floatVariable.Value);
#if UNITY_EDITOR
                    LogSavedMessage(floatVariable.name, floatVariable.Value.ToString("F6"));
#endif
                    break;


                case DataType.stringType:
                    StringVariable stringVariable = userDatum.scriptableObject as StringVariable;
                    PlayerPrefs.SetString(stringVariable.name, stringVariable.Value);
#if UNITY_EDITOR
                    LogSavedMessage(stringVariable.name, stringVariable.Value);
#endif
                    break;

                case DataType.intType:
                    IntVariable intVariable = userDatum.scriptableObject as IntVariable;
                    PlayerPrefs.SetInt(intVariable.name, intVariable.Value);
#if UNITY_EDITOR
                    LogSavedMessage(intVariable.name, intVariable.Value.ToString("F6"));
#endif
                    break;
            }
        }

        public void CallReadFromStoredData()
        {
            for (int i = 0; i < userData.Count; i++) {
                ReadFromStoredData(userData[i]);
            }
        }

        public void CallReadFromStoredData(ScriptableObject sourceObject)
        {
            bool valueFound = false;
            for (int i = 0; i < userData.Count; i++) {
                if (sourceObject == userData[i].scriptableObject) {
                    valueFound = true;
                    ReadFromStoredData(userData[i]);
                }
            }

            if (valueFound == false) {
                Debug.LogError("Data unable to be read - " + sourceObject.name + " variable not stored in " + this.name, this);
            }
        }

        void ReadFromStoredData(UserDatum userDatum)
        {
            switch (userDatum.dataType) {

                case DataType.boolType:
                    BoolVariable boolVariable = userDatum.scriptableObject as BoolVariable;
                    int boolIntValue = PlayerPrefs.GetInt(boolVariable.name);
                    if(boolIntValue == 1) {
                        boolVariable.SetValue(true);
                    } else if (boolIntValue == 0) {
                        boolVariable.SetValue(false);
                    }
                    break;

                case DataType.floatType:
                    FloatVariable floatVariable = userDatum.scriptableObject as FloatVariable;
                    float floatValue = PlayerPrefs.GetFloat(floatVariable.name);
                    floatVariable.SetValue(floatValue);
                    break;


                case DataType.stringType:
                    StringVariable stringVariable = userDatum.scriptableObject as StringVariable;
                    string stringValue = PlayerPrefs.GetString(stringVariable.name);
                    stringVariable.SetValue(stringValue);
                    break;

                case DataType.intType:
                    IntVariable intVariable = userDatum.scriptableObject as IntVariable;
                    int intValue = PlayerPrefs.GetInt(intVariable.name);
                    intVariable.SetValue(intValue);
                    break;
            }
        }

#if UNITY_EDITOR
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

        void LogFromStoredData(UserDatum userDatum)
        {
            switch (userDatum.dataType) {

                case DataType.boolType:
                    BoolVariable boolVariable = userDatum.scriptableObject as BoolVariable;
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

                case DataType.floatType:
                    FloatVariable floatVariable = userDatum.scriptableObject as FloatVariable;
                    float floatValue = PlayerPrefs.GetFloat(floatVariable.name);
                    Debug.Log("Value of " + floatVariable.name + " variable in stored data is " + floatValue);
                    break;

                case DataType.stringType:
                    StringVariable stringVariable = userDatum.scriptableObject as StringVariable;
                    string stringValue = PlayerPrefs.GetString(stringVariable.name);
                    Debug.Log("Value of " + stringVariable.name + " variable in stored data is " + stringValue);
                    break;

                case DataType.intType:
                    IntVariable intVariable = userDatum.scriptableObject as IntVariable;
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
#endif

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
            public ScriptableObject scriptableObject;

            [SerializeField]
            public DataType dataType;

        }
    }

}