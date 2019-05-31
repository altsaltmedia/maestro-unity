using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [Serializable]
    [ExecuteInEditMode]
    public class ComplexEventPackager
    {
        [Required]
        [SerializeField]
        ComplexEvent Event;

        [SerializeField]
        [BoxGroup("String Packager")]
        bool hasString;

        [SerializeField]
        [ShowIf("ShowCustomStringToggle")]
        [BoxGroup("String Packager", false)]
        bool customStringKey;

#if UNITY_EDITOR
        [SerializeField]
        [ShowIf("hasString")]
        [Multiline]
        [BoxGroup("String Packager")]
        string stringDescription = "";
#endif

        [SerializeField]
        [ShowIf("ShowStringKeys")]
        [ValidateInput("CheckStringKeys", "Keys and Values must be of equal length")]
        [BoxGroup("String Packager")]
        EventPayloadKey[] stringKeys;

        [SerializeField]
        [ShowIf("hasString")]
        [ValidateInput("CheckStringValues", "Keys and Values must be of equal length")]
        [BoxGroup("String Packager")]
        string[] stringValues;

        [PropertySpace]

        [SerializeField]
        [BoxGroup("Float Packager", false)]
        bool hasFloat;

        [SerializeField]
        [ShowIf("ShowCustomFloatToggle")]
        [BoxGroup("Float Packager")]
        bool customFloatKey;

#if UNITY_EDITOR
        [SerializeField]
        [ShowIf("hasFloat")]
        [Multiline]
        [BoxGroup("Float Packager")]
        string floatDescription = "";
#endif

        [SerializeField]
        [ShowIf("ShowFloatKeys")]
        [ValidateInput("CheckFloatKeys", "Keys and Values must be of equal length")]
        [BoxGroup("Float Packager")]
        EventPayloadKey[] floatKeys;

        [SerializeField]
        [ShowIf("hasFloat")]
        [ValidateInput("CheckFloatValues", "Keys and Values must be of equal length")]
        [BoxGroup("Float Packager")]
        float[] floatValues;

        [PropertySpace]

        [SerializeField]
        [BoxGroup("Bool Packager", false)]
        bool hasBool;

        [SerializeField]
        [ShowIf("ShowCustomBoolToggle")]
        [BoxGroup("Bool Packager")]
        bool customBoolKey;

#if UNITY_EDITOR
        [SerializeField]
        [ShowIf("hasBool")]
        [Multiline]
        [BoxGroup("Bool Packager")]
        string boolDescription = "";
#endif

        [SerializeField]
        [ShowIf("ShowBoolKeys")]
        [ValidateInput("CheckBoolKeys", "Keys and Values must be of equal length")]
        [BoxGroup("Bool Packager")]
        EventPayloadKey[] boolKeys;

        [SerializeField]
        [ShowIf("hasBool")]
        [ValueDropdown("boolValueList")]
        [ValidateInput("CheckBoolValues", "Keys and Values must be of equal length")]
        [BoxGroup("Bool Packager")]
        bool[] boolValues;

        private ValueDropdownList<bool> boolValueList = new ValueDropdownList<bool>(){
                {"FALSE", false },
                {"TRUE", true }
            };

        [PropertySpace]

        [SerializeField]
        [BoxGroup("Scriptable Object Packager", false)]
        bool hasScriptableObject;

        [SerializeField]
        [ShowIf("ShowCustomScriptableObjectToggle")]
        [BoxGroup("Scriptable Object Packager")]
        bool customScriptableObjectKey;

#if UNITY_EDITOR
        [SerializeField]
        [ShowIf("hasScriptableObject")]
        [Multiline]
        [BoxGroup("Scriptable Object Packager")]
        string scriptableObjectDescription = "";
#endif

        [SerializeField]
        [ShowIf("ShowScriptableObjectKeys")]
        [ValidateInput("CheckScriptableObjectKeys", "Keys and Values must be of equal length")]
        [BoxGroup("Scriptable Object Packager")]
        EventPayloadKey[] scriptableObjectKeys;

        [SerializeField]
        [ShowIf("hasScriptableObject")]
        [ValidateInput("CheckScriptableObjectValues", "Keys and Values must be of equal length")]
        [BoxGroup("Scriptable Object Packager")]
        ScriptableObject[] scriptableObjectValues;

        public void RaiseComplexEvent()
        {
            EventPayload eventPayload = EventPayload.CreateInstance();
            if (hasString) {
                eventPayload = GetStringValues(eventPayload);
            }
            if (hasFloat) {
                eventPayload = GetFloatValues(eventPayload);
            }
            if (hasBool) {
                eventPayload = GetBoolValues(eventPayload);
            }
            if (hasScriptableObject) {
                eventPayload = GetScriptableObjectValues(eventPayload);
            }
            Event.Raise(eventPayload);
        }

        EventPayload GetStringValues(EventPayload eventPayload)
        {
            if (stringValues.Length == 0) {
                LogWarning();
                throw new Exception("Event payload could not be created");
            }
            else if (stringValues.Length == 1) {
                if(customStringKey == true && stringKeys.Length >= 1) {
                    eventPayload.Set(stringKeys[0], stringValues[0]);
                } else {
                    eventPayload.Set(DataType.stringType, stringValues[0]);
                }
            }
            else {
                for (int i = 0; i < stringValues.Length; i++) {
                    eventPayload.Set(stringKeys[i], stringValues[i]);
                }
            }
            return eventPayload;
        }

        EventPayload GetFloatValues(EventPayload eventPayload)
        {
            if (floatValues.Length == 0) {
                LogWarning();
                throw new Exception("Event payload could not be created");
            }
            else if (floatValues.Length == 1) {
                if (customFloatKey == true && floatKeys.Length >= 1) {
                    eventPayload.Set(floatKeys[0], floatValues[0]);
                } else {
                    eventPayload.Set(DataType.floatType, floatValues[0]);
                }
            }
            else {
                for (int i = 0; i < floatValues.Length; i++) {
                    eventPayload.Set(floatKeys[i], floatValues[i]);
                }
            }
            return eventPayload;
        }

        EventPayload GetBoolValues(EventPayload eventPayload)
        {
            if (boolValues.Length == 0) {
                LogWarning();
                throw new Exception("Event payload could not be created");
            }
            else if (boolValues.Length == 1) {
                if (customBoolKey == true && boolKeys.Length >= 1) {
                    eventPayload.Set(boolKeys[0], boolValues[0]);
                } else {
                    eventPayload.Set(DataType.boolType, boolValues[0]);
                }
            }
            else {
                for (int i = 0; i < boolValues.Length; i++) {
                    eventPayload.Set(boolKeys[i], boolValues[i]);
                }
            }
            return eventPayload;
        }

        EventPayload GetScriptableObjectValues(EventPayload eventPayload)
        {
            if (scriptableObjectValues.Length == 0) {
                LogWarning();
                throw new Exception("Event payload could not be created");
            }
            else if (scriptableObjectValues.Length == 1) {
                if (customScriptableObjectKey == true && scriptableObjectKeys.Length >= 1) {
                    eventPayload.Set(scriptableObjectKeys[0], scriptableObjectValues[0]);
                } else {
                    eventPayload.Set(DataType.scriptableObjectType, scriptableObjectValues[0]);
                }
            }
            else {
                for (int i = 0; i < scriptableObjectValues.Length; i++) {
                    eventPayload.Set(scriptableObjectKeys[i], scriptableObjectValues[i]);
                }
            }
            return eventPayload;
        }

        protected virtual void LogWarning()
        {
            Debug.LogWarning("Failed to create event payload");
        }

#if UNITY_EDITOR
        bool ShowStringKeys()
        {
            if (hasString && stringValues != null && stringValues.Length > 1) {
                return true;
            }

            if(customStringKey == true) {
                return true;
            }

            return false;
        }

        bool ShowFloatKeys()
        {
            if (hasFloat && floatValues != null && floatValues.Length > 1) {
                return true;
            }

            if(customFloatKey) {
                return true;
            }

            return false;
        }

        bool ShowBoolKeys()
        {
            if (hasBool && boolValues != null && boolValues.Length > 1) {
                return true;
            }

            if(customBoolKey) {
                return true;
            }

            return false;
        }

        bool ShowScriptableObjectKeys()
        {
            if (hasScriptableObject && scriptableObjectValues != null && scriptableObjectValues.Length > 1) {
                return true;
            }

            if(customScriptableObjectKey) {
                return true;
            }

            return false;
        }

        bool ShowCustomStringToggle()
        {
            if(hasString && stringValues.Length <= 1) {
                return true;
            } else {
                return false;
            }
        }

        bool ShowCustomFloatToggle()
        {
            if (hasFloat && floatValues.Length <= 1) {
                return true;
            } else {
                return false;
            }
        }

        bool ShowCustomBoolToggle()
        {
            if (hasBool && boolValues.Length <= 1) {
                return true;
            } else {
                return false;
            }
        }

        bool ShowCustomScriptableObjectToggle()
        {
            if (hasScriptableObject && scriptableObjectValues.Length <= 1) {
                return true;
            } else {
                return false;
            }
        }

        bool CheckStringKeys(EventPayloadKey[] attribute)
        {
            if(attribute.Length != stringValues.Length) {
                return false;
            } else {
                return true;
            }
        }

        bool CheckFloatKeys(EventPayloadKey[] attribute)
        {
            if (attribute.Length != floatValues.Length) {
                return false;
            } else {
                return true;
            }
        }

        bool CheckBoolKeys(EventPayloadKey[] attribute)
        {
            if (attribute.Length != boolValues.Length) {
                return false;
            } else {
                return true;
            }
        }

        bool CheckScriptableObjectKeys(EventPayloadKey[] attribute)
        {
            if (attribute.Length != scriptableObjectValues.Length) {
                return false;
            } else {
                return true;
            }
        }

        bool CheckStringValues(string[] attribute)
        {
            if(attribute.Length <= 1 && customStringKey == false) {
                return true;
            }
            if (attribute.Length != stringKeys.Length) {
                return false;
            } else {
                return true;
            }
        }

        bool CheckFloatValues(float[] attribute)
        {
            if (attribute.Length <= 1 && customFloatKey == false) {
                return true;
            }
            if (attribute.Length != floatKeys.Length) {
                return false;
            } else {
                return true;
            }
        }

        bool CheckBoolValues(bool[] attribute)
        {
            if (attribute.Length <= 1 && customBoolKey == false) {
                return true;
            }
            if (attribute.Length != boolKeys.Length) {
                return false;
            } else {
                return true;
            }
        }

        bool CheckScriptableObjectValues(ScriptableObject[] attribute)
        {
            if (attribute.Length <= 1 && customScriptableObjectKey == false) {
                return true;
            }
            if (attribute.Length != scriptableObjectKeys.Length) {
                return false;
            } else {
                return true;
            }
        }
#endif
    }
}