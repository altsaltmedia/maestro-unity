using System;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;

namespace AltSalt.Maestro
{

    [Serializable]
    [CreateAssetMenu(menuName = "AltSalt/Events/Event Payload")]
    public class ComplexPayload : ScriptableObject
    {
#if UNITY_EDITOR
        [SerializeField]
        [Multiline]
        [Header("Event Payload")]
        string description;
#endif

        [SerializeField]
        private StringDictionary _stringDictionary = new StringDictionary();

        private StringDictionary stringDictionary => _stringDictionary;

        [SerializeField]
        private FloatDictionary _floatDictionary = new FloatDictionary();

        private FloatDictionary floatDictionary => _floatDictionary;

        [SerializeField]
        private IntDictionary _intDictionary = new IntDictionary();

        private IntDictionary intDictionary => _intDictionary;

        [SerializeField]
        public BoolDictionary boolDictionary = new BoolDictionary();

        [SerializeField]
        private ScriptableObjectDictionary _scriptableObjectDictionary = new ScriptableObjectDictionary();

        private ScriptableObjectDictionary scriptableObjectDictionary => _scriptableObjectDictionary;

        [SerializeField]
        private ObjectDictionary _objectDictionary = new ObjectDictionary();

        private ObjectDictionary objectDictionary => _objectDictionary;

        static readonly string arrayExceptionMessage = "Discrepancy between number of keys and values";

        private static ComplexPayload Init()
        {
            ComplexPayload payloadInstance = ScriptableObject.CreateInstance(typeof(ComplexPayload)) as ComplexPayload;
            return payloadInstance;
        }

        public static ComplexPayload CreateInstance()
        {
            ComplexPayload payloadInstance = Init();
            return payloadInstance;
        }

        public static ComplexPayload CreateInstance(object value)
        {
            ComplexPayload payloadInstance = Init();

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

        public static ComplexPayload CreateInstance(object key, object value)
        {
            ComplexPayload payloadInstance = Init();

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

        public static ComplexPayload CreateInstance(object[] keys, object[] values)
        {
            ComplexPayload payloadInstance = Init();
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
            }

            //Debug.Log("Key for string value not found in EventPayload");
            return null;
        }
        
        public string GetStringValue(object key)
        {
            if (stringDictionary.ContainsKey(key)) {
                return stringDictionary[key];
            }

            //Debug.Log("Key for string value not found in EventPayload");
            return null;
        }

        public float GetFloatValue()
        {
            if (floatDictionary.ContainsKey(DataType.floatType)) {
                return floatDictionary[DataType.floatType];
            }

            //Debug.Log("Key for float value not found in EventPayload");
            return float.NaN;
        }
        
        public float GetFloatValue(object key)
        {
            if (floatDictionary.ContainsKey(key)) {
                return floatDictionary[key];
            }

            //Debug.Log("Key for float value not found in EventPayload");
            return float.NaN;
        }
        
        
        public int GetIntValue()
        {
            if (intDictionary.ContainsKey(DataType.intType)) {
                return intDictionary[DataType.intType];
            }

            //Debug.Log("Key for int value not found in EventPayload");
            return -1;
        }
        
        public int GetIntValue(object key)
        {
            if (intDictionary.ContainsKey(key)) {
                return intDictionary[key];
            }

            //Debug.Log("Key for int value not found in EventPayload");
            return -1;
        }


        public bool GetBoolValue(object key)
        {
            if (boolDictionary.ContainsKey(key)) {
                return boolDictionary[key];
            }

            return false;
        }

        public ScriptableObject GetScriptableObjectValue()
        {
            if (scriptableObjectDictionary.ContainsKey(DataType.scriptableObjectType)) {
                return scriptableObjectDictionary[DataType.scriptableObjectType];
            }

            // Debug.Log("Key for scriptable object value not found in EventPayload");
            return null;
        }
        
        public ScriptableObject GetScriptableObjectValue(object key)
        {
            if (scriptableObjectDictionary.ContainsKey(key)) {
                return scriptableObjectDictionary[key];
            }

            //Debug.Log("Key for scriptable object value not found in EventPayload");
            return null;
        }

        public object GetObjectValue()
        {
            if (objectDictionary.ContainsKey(DataType.systemObjectType)) {
                return objectDictionary[DataType.systemObjectType];
            }

            //Debug.Log("Key for object value not found in EventPayload");
            return null;
        }
        
        public object GetObjectValue(object key)
        {
            if (objectDictionary.ContainsKey(key)) {
                return objectDictionary[key];
            }

            //Debug.Log("Key for object value not found in EventPayload");
            return null;
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