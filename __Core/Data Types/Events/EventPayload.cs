using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;

namespace AltSalt
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
        public BoolDictionary boolDictionary = new BoolDictionary();

        [SerializeField]
        public ScriptableObjectDictionary scriptableObjectDictionary = new ScriptableObjectDictionary();

        static readonly string arrayExceptionMessage = "Discrepancy between number of keys and values";

        public static EventPayload Init()
        {
            EventPayload payloadInstance = ScriptableObject.CreateInstance(typeof(EventPayload)) as EventPayload;
            return payloadInstance;
        }

        public static EventPayload CreateInstance()
        {
            EventPayload payloadInstance = Init();
            return payloadInstance;
        }

        public static new EventPayload CreateInstance(string value)
        {
            EventPayload payloadInstance = Init();
            payloadInstance.stringDictionary[EventPayloadType.stringPayload] = value;
            return payloadInstance;
        }

        public static EventPayload CreateInstance(float value)
        {
            EventPayload payloadInstance = Init();
            payloadInstance.floatDictionary[EventPayloadType.floatPayload] = value;
            return payloadInstance;
        }

        public static EventPayload CreateInstance(bool value)
        {
            EventPayload payloadInstance = Init();
            payloadInstance.boolDictionary[EventPayloadType.boolPayload] = value;
            return payloadInstance;
        }

        public static EventPayload CreateInstance(ScriptableObject value)
        {
            EventPayload payloadInstance = Init();
            payloadInstance.scriptableObjectDictionary[EventPayloadType.scriptableObjectPayload] = value;
            return payloadInstance;
        }

        public static EventPayload CreateInstance(object key, string value)
        {
            EventPayload payloadInstance = Init();
            payloadInstance.stringDictionary[key] = value;
            return payloadInstance;
        }

        public static EventPayload CreateInstance(object key, float value)
        {
            EventPayload payloadInstance = Init();
            payloadInstance.floatDictionary[key] = value;
            return payloadInstance;
        }

        public static EventPayload CreateInstance(object key, bool value)
        {
            EventPayload payloadInstance = Init();
            payloadInstance.boolDictionary[key] = value;
            return payloadInstance;
        }

        public static EventPayload CreateInstance(object key, ScriptableObject value)
        {
            EventPayload payloadInstance = Init();
            payloadInstance.scriptableObjectDictionary[key] = value;
            return payloadInstance;
        }

        public static EventPayload CreateInstance(object[] keys, string[] values)
        {
            EventPayload payloadInstance = Init();
            if(keys.Length != values.Length) {
                throw new Exception(arrayExceptionMessage);
            }
            for (int i = 0; i < keys.Length; i++) {
                payloadInstance.stringDictionary[keys[i]] = values[i];
            }
            return payloadInstance;
        }

        public static EventPayload CreateInstance(object[] keys, float[] values)
        {
            EventPayload payloadInstance = Init();
            if (keys.Length != values.Length) {
                throw new Exception(arrayExceptionMessage);
            }
            for (int i = 0; i < keys.Length; i++) {
                payloadInstance.floatDictionary[keys[i]] = values[i];
            }
            return payloadInstance;
        }

        public static EventPayload CreateInstance(object[] keys, bool[] values)
        {
            EventPayload payloadInstance = Init();
            if (keys.Length != values.Length) {
                throw new Exception(arrayExceptionMessage);
            }
            for (int i = 0; i < keys.Length; i++) {
                payloadInstance.boolDictionary[keys[i]] = values[i];
            }
            return payloadInstance;
        }

        public static EventPayload CreateInstance(object[] keys, ScriptableObject[] values)
        {
            EventPayload payloadInstance = Init();
            if (keys.Length != values.Length) {
                throw new Exception(arrayExceptionMessage);
            }
            for (int i = 0; i < keys.Length; i++) {
                payloadInstance.scriptableObjectDictionary[keys[i]] = values[i];
            }
            return payloadInstance;
        }

        public void Set(string value)
        {
            stringDictionary[EventPayloadType.stringPayload] = value;
        }

        public void Set(float value)
        {
            floatDictionary[EventPayloadType.floatPayload] = value;
        }

        public void Set(bool value)
        {
            boolDictionary[EventPayloadType.boolPayload] = value;
        }

        public void Set(ScriptableObject value)
        {
            scriptableObjectDictionary[EventPayloadType.scriptableObjectPayload] = value;
        }

        public void Set (object key, string value)
        {
            stringDictionary[key] = value;
		}

        public void Set(object key, float value)
        {
            floatDictionary[key] = value;
        }

        public void Set(object key, bool value)
        {
            boolDictionary[key] = value;
        }

        public void Set(object key, ScriptableObject value)
        {
            scriptableObjectDictionary[key] = value;
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

        public float GetFloatValue(object key)
        {
            if (floatDictionary.ContainsKey(key)) {
                return floatDictionary[key];
            } else {
                Debug.Log("Key for float value not found in EventPayload");
                return -1f;
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

        public ScriptableObject GetScriptableObjectValue(object key)
        {
            if (scriptableObjectDictionary.ContainsKey(key)) {
                return scriptableObjectDictionary[key];
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
        public class BoolDictionary : SerializableDictionaryBase<object, bool> { }

        [Serializable]
        public class ScriptableObjectDictionary : SerializableDictionaryBase<object, ScriptableObject> { }
    }

}