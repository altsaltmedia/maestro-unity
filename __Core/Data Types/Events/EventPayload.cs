﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AltSalt
{
    public enum EventPayloadType { stringPayload, floatPayload, boolPayload }

    [Serializable]
    public class EventPayload
    {
        private Dictionary<string, string> stringDictionary = new Dictionary<string, string>();
        private Dictionary<string, float> floatDictionary = new Dictionary<string, float>();
        private Dictionary<string, bool> boolDictionary = new Dictionary<string, bool>();

        public EventPayload() {}

        public EventPayload(string value)
        {
            stringDictionary.Add(EventPayloadType.stringPayload.ToString(), value);
        }

        public EventPayload(float value)
        {
            floatDictionary.Add(EventPayloadType.floatPayload.ToString(), value);
        }

        public EventPayload(bool value)
        {
            boolDictionary.Add(EventPayloadType.boolPayload.ToString(), value);
        }

        /*
         * @Set string
         */
        public void Set (string key, string value)
        {
            if (stringDictionary.ContainsKey(key)) {
                stringDictionary[key] = value;
            }
            else {
                stringDictionary.Add(key, value);
            }
		}

        /*
         * @Set float
         */
        public void Set(string key, float value)
        {
            if (stringDictionary.ContainsKey(key)) {
                floatDictionary[key] = value;
            }
            else {
                floatDictionary.Add(key, value);
            }
        }

        /*
         * @Set bool
         */
        public void Set(string key, bool value)
        {
            if (stringDictionary.ContainsKey(key)) {
                boolDictionary[key] = value;
            }
            else {
                boolDictionary.Add(key, value);
            }
        }

        public string GetStringValue (string key)
        {
            string result = null;
            if (stringDictionary.ContainsKey(key)) {
                return stringDictionary[key];
            }
            return result;
        }

        public float GetFloatValue(string key)
        {
            float result = float.NaN;
            if (floatDictionary.ContainsKey(key)) {
                result = floatDictionary[key];
            }
            return result;
        }

        public bool GetBoolValue(string key)
        {
            if (boolDictionary.ContainsKey(key)) {
                return boolDictionary[key];
            } else {
                return false;
            }
        }

    }
	
}