﻿using System;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;

namespace AltSalt.Maestro
{

    [Serializable]
    [CreateAssetMenu(menuName = "AltSalt/Events/Event Payload")]
    public class EventPayload : ScriptableObject
    {
#if UNITY_EDITOR
        [SerializeField]
        [Multiline]
        [Header("Event Payload")]
        string description;
#endif

        [SerializeField]
        public StringDictionary stringDictionary = new StringDictionary();

        [SerializeField]
        public FloatDictionary floatDictionary = new FloatDictionary();
        
        [SerializeField]
        private IntDictionary _intDictionary = new IntDictionary();

        public IntDictionary intDictionary
        {
            get => _intDictionary;
            private set => _intDictionary = value;
        }

        [SerializeField]
        public BoolDictionary boolDictionary = new BoolDictionary();

        [SerializeField]
        public ScriptableObjectDictionary scriptableObjectDictionary = new ScriptableObjectDictionary();

        [SerializeField]
        public ObjectDictionary objectDictionary = new ObjectDictionary();

        static readonly string arrayExceptionMessage = "Discrepancy between number of keys and values";

        private static EventPayload Init()
        {
            EventPayload payloadInstance = ScriptableObject.CreateInstance(typeof(EventPayload)) as EventPayload;
            return payloadInstance;
        }

        public static EventPayload CreateInstance()
        {
            EventPayload payloadInstance = Init();
            return payloadInstance;
        }

        public static EventPayload CreateInstance(object value)
        {
            EventPayload payloadInstance = Init();

            if(value is string) {
                payloadInstance.stringDictionary[DataType.stringType] = (string)value;

            } else if (value is float) {
                payloadInstance.floatDictionary[DataType.floatType] = (float)value;
            
            } else if (value is int) {
                payloadInstance.intDictionary[DataType.intType] = (int)value;
                
            } else if (value is bool) {
                payloadInstance.boolDictionary[DataType.boolType] = (bool)value;

            } else if (value is ScriptableObject) {
                payloadInstance.scriptableObjectDictionary[DataType.scriptableObjectType] = (ScriptableObject)value;

            } else {
                payloadInstance.objectDictionary[DataType.systemObjectType] = value;
            }
                
            return payloadInstance;
        }

        public static EventPayload CreateInstance(object key, object value)
        {
            EventPayload payloadInstance = Init();

            if (value is string) {
                payloadInstance.objectDictionary[key] = (string)value;

            } else if (value is float) {
                payloadInstance.floatDictionary[key] = (float)value;
                
            } else if (value is int) {
                payloadInstance.intDictionary[key] = (int)value;

            } else if (value is bool) {
                payloadInstance.boolDictionary[key] = (bool)value;

            } else if (value is ScriptableObject) {
                payloadInstance.scriptableObjectDictionary[key] = (ScriptableObject)value;

            } else {
                payloadInstance.objectDictionary[key] = value;
            }

            return payloadInstance;
        }

        public static EventPayload CreateInstance(object[] keys, object[] values)
        {
            EventPayload payloadInstance = Init();
            if (keys.Length != values.Length) {
                throw new Exception(arrayExceptionMessage);
            }

            if (values.GetType() == typeof(string[])) {
                for (int i = 0; i < keys.Length; i++) {
                    payloadInstance.stringDictionary[keys[i]] = (string)values[i];
                }

            } else if (values.GetType() == typeof(float[])) {
                for (int i = 0; i < keys.Length; i++) {
                    payloadInstance.floatDictionary[keys[i]] = (float)values[i];
                }
                
            } else if (values.GetType() == typeof(int[])) {
                for (int i = 0; i < keys.Length; i++) {
                    payloadInstance.intDictionary[keys[i]] = (int)values[i];
                }

            } else if (values.GetType() == typeof(bool[])) {
                for (int i = 0; i < keys.Length; i++) {
                    payloadInstance.boolDictionary[keys[i]] = (bool)values[i];
                }

            } else if (values.GetType() == typeof(ScriptableObject[])) {
                for (int i = 0; i < keys.Length; i++) {
                    payloadInstance.boolDictionary[keys[i]] = (ScriptableObject)values[i];
                }

            } else {
                for (int i = 0; i < keys.Length; i++) {
                    payloadInstance.objectDictionary[keys[i]] = values[i];
                }
            }

            return payloadInstance;
        }

        public void Set(object value)
        {
            if (value is string) {

                stringDictionary[DataType.stringType] = (string)value;

            } else if (value is float) {

                floatDictionary[DataType.floatType] = (float)value;
                
            } else if (value is int) {

                intDictionary[DataType.intType] = (int)value;

            } else if (value is bool) {

                boolDictionary[DataType.boolType] = (bool)value;

            } else if (value is ScriptableObject) {

                scriptableObjectDictionary[DataType.scriptableObjectType] = (ScriptableObject)value;

            } else {

                objectDictionary[DataType.systemObjectType] = value;
            }
        }

        public void Set(object key, object value)
        {
            if (value is string) {

                stringDictionary[key] = (string)value;

            } else if (value is float) {

                floatDictionary[key] = (float)value;
                
            } else if (value is int) {

                intDictionary[key] = (int)value;

            } else if (value is bool) {

                boolDictionary[key] = (bool)value;

            } else if (value is ScriptableObject) {

                scriptableObjectDictionary[key] = (ScriptableObject)value;

            } else {

                objectDictionary[key] = value;
            }
        }

        public string GetStringValue()
        {
            if (stringDictionary.ContainsKey(DataType.stringType)) {
                return stringDictionary[DataType.stringType];
            } else {
                Debug.Log("Key for string value not found in EventPayload");
                return null;
            }
        }
        
        public string GetStringValue(object key)
        {
            if (stringDictionary.ContainsKey(key)) {
                return stringDictionary[key];
            } else {
                Debug.Log("Key for string value not found in EventPayload");
                return null;
            }
        }

        public float GetFloatValue()
        {
            if (floatDictionary.ContainsKey(DataType.floatType)) {
                return floatDictionary[DataType.floatType];
            } else {
                Debug.Log("Key for float value not found in EventPayload");
                return float.NaN;
            }
        }
        
        public float GetFloatValue(object key)
        {
            if (floatDictionary.ContainsKey(key)) {
                return floatDictionary[key];
            } else {
//                Debug.Log("Key for float value not found in EventPayload");
                return float.NaN;
            }
        }
        
        
        public int GetIntValue()
        {
            if (intDictionary.ContainsKey(DataType.intType)) {
                return intDictionary[DataType.intType];
            } else {
                Debug.Log("Key for int value not found in EventPayload");
                return -1;
            }
        }
        
        public int GetIntValue(object key)
        {
            if (intDictionary.ContainsKey(key)) {
                return intDictionary[key];
            } else {
                Debug.Log("Key for int value not found in EventPayload");
                return -1;
            }
        }


        public bool GetBoolValue(object key)
        {
            if (boolDictionary.ContainsKey(key)) {
                return boolDictionary[key];
            } else {
                return false;
            }
        }

        public ScriptableObject GetScriptableObjectValue()
        {
            if (scriptableObjectDictionary.ContainsKey(DataType.scriptableObjectType)) {
                return scriptableObjectDictionary[DataType.scriptableObjectType];
            } else {
//                Debug.Log("Key for scriptable object value not found in EventPayload");
                return null;
            }
        }
        
        public ScriptableObject GetScriptableObjectValue(object key)
        {
            if (scriptableObjectDictionary.ContainsKey(key)) {
                return scriptableObjectDictionary[key];
            } else {
//                Debug.Log("Key for scriptable object value not found in EventPayload");
                return null;
            }
        }

        public object GetObjectValue(object key)
        {
            if (objectDictionary.ContainsKey(key)) {
                return objectDictionary[key];
            } else {
                //                Debug.Log("Key for scriptable object value not found in EventPayload");
                return null;
            }
        }

        [Serializable]
        public class StringDictionary : SerializableDictionaryBase<object, string> { }

        [Serializable]
        public class FloatDictionary : SerializableDictionaryBase<object, float> { }
        
        [Serializable]
        public class IntDictionary : SerializableDictionaryBase<object, int> { }

        [Serializable]
        public class BoolDictionary : SerializableDictionaryBase<object, bool> { }

        [Serializable]
        public class ScriptableObjectDictionary : SerializableDictionaryBase<object, ScriptableObject> { }

        [Serializable]
        public class ObjectDictionary : SerializableDictionaryBase<object, object> { }
    }

}