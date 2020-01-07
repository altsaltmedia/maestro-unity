using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro
{
    [Serializable]
    [ExecuteInEditMode]
    // Meant to be used within ComplexEventTriggerPackagerBehaviour or ComplexEventTimelineTrigger.
    // If implementing a custom trigger from within a script, use the basic ComplexEventTrigger instead.
    public class ComplexEventTriggerPackager : EventTriggerBase
    {
        [Required]
        [SerializeField]
        ComplexEvent complexEvent;

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
        List<CustomKey> stringKeys = new List<CustomKey>();

        [SerializeField]
        [ShowIf("hasString")]
        [ValidateInput("CheckStringValues", "Keys and Values must be of equal length")]
        [BoxGroup("String Packager")]
        List<string> stringValues = new List<string>();

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
        List<CustomKey> floatKeys = new List<CustomKey>();

        [SerializeField]
        [ShowIf("hasFloat")]
        [ValidateInput("CheckFloatValues", "Keys and Values must be of equal length")]
        [BoxGroup("Float Packager")]
        List<float> floatValues = new List<float>();

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
        List<CustomKey> boolKeys = new List<CustomKey>();

        [SerializeField]
        [ShowIf("hasBool")]
        [ValueDropdown("boolValueList")]
        [ValidateInput("CheckBoolValues", "Keys and Values must be of equal length")]
        [BoxGroup("Bool Packager")]
        List<bool> boolValues = new List<bool>();

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
        List<CustomKey> scriptableObjectKeys = new List<CustomKey>();

        [SerializeField]
        [ShowIf("hasScriptableObject")]
        [ValidateInput("CheckScriptableObjectValues", "Keys and Values must be of equal length")]
        [BoxGroup("Scriptable Object Packager")]
        List<ScriptableObject> scriptableObjectValues = new List<ScriptableObject>();

        public void RaiseEvent(GameObject caller)
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
            complexEvent.StoreCaller(caller);
            complexEvent.Raise(eventPayload);
        }

        public void RaiseEvent(GameObject caller, string sourceScene, string sourceName)
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
            complexEvent.StoreCaller(caller, sourceScene, sourceName);
            complexEvent.Raise(eventPayload);
        }

        EventPayload GetStringValues(EventPayload eventPayload)
        {
            if (stringValues.Count == 0) {
                LogWarning();
                throw new Exception("Event payload could not be created");
            } else if (stringValues.Count == 1) {
                if (customStringKey == true && stringKeys.Count >= 1) {
                    eventPayload.Set(stringKeys[0], stringValues[0]);
                } else {
                    eventPayload.Set(DataType.stringType, stringValues[0]);
                }
            } else {
                for (int i = 0; i < stringValues.Count; i++) {
                    eventPayload.Set(stringKeys[i], stringValues[i]);
                }
            }
            return eventPayload;
        }

        EventPayload GetFloatValues(EventPayload eventPayload)
        {
            if (floatValues.Count == 0) {
                LogWarning();
                throw new Exception("Event payload could not be created");
            } else if (floatValues.Count == 1) {
                if (customFloatKey == true && floatKeys.Count >= 1) {
                    eventPayload.Set(floatKeys[0], floatValues[0]);
                } else {
                    eventPayload.Set(DataType.floatType, floatValues[0]);
                }
            } else {
                for (int i = 0; i < floatValues.Count; i++) {
                    eventPayload.Set(floatKeys[i], floatValues[i]);
                }
            }
            return eventPayload;
        }

        EventPayload GetBoolValues(EventPayload eventPayload)
        {
            if (boolValues.Count == 0) {
                LogWarning();
                throw new Exception("Event payload could not be created");
            } else if (boolValues.Count == 1) {
                if (customBoolKey == true && boolKeys.Count >= 1) {
                    eventPayload.Set(boolKeys[0], boolValues[0]);
                } else {
                    eventPayload.Set(DataType.boolType, boolValues[0]);
                }
            } else {
                for (int i = 0; i < boolValues.Count; i++) {
                    eventPayload.Set(boolKeys[i], boolValues[i]);
                }
            }
            return eventPayload;
        }

        EventPayload GetScriptableObjectValues(EventPayload eventPayload)
        {
            if (scriptableObjectValues.Count == 0) {
                LogWarning();
                throw new Exception("Event payload could not be created");
            } else if (scriptableObjectValues.Count == 1) {
                if (customScriptableObjectKey == true && scriptableObjectKeys.Count >= 1) {
                    eventPayload.Set(scriptableObjectKeys[0], scriptableObjectValues[0]);
                } else {
                    eventPayload.Set(DataType.scriptableObjectType, scriptableObjectValues[0]);
                }
            } else {
                for (int i = 0; i < scriptableObjectValues.Count; i++) {
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
            if (hasString && stringValues != null && stringValues.Count > 1) {
                return true;
            }

            if(customStringKey == true) {
                return true;
            }

            return false;
        }

        bool ShowFloatKeys()
        {
            if (hasFloat && floatValues != null && floatValues.Count > 1) {
                return true;
            }

            if(customFloatKey) {
                return true;
            }

            return false;
        }

        bool ShowBoolKeys()
        {
            if (hasBool && boolValues != null && boolValues.Count > 1) {
                return true;
            }

            if(customBoolKey) {
                return true;
            }

            return false;
        }

        bool ShowScriptableObjectKeys()
        {
            if (hasScriptableObject && scriptableObjectValues != null && scriptableObjectValues.Count > 1) {
                return true;
            }

            if(customScriptableObjectKey) {
                return true;
            }

            return false;
        }

        bool ShowCustomStringToggle()
        {
            if(hasString && stringValues.Count <= 1) {
                return true;
            } else {
                return false;
            }
        }

        bool ShowCustomFloatToggle()
        {
            if (hasFloat && floatValues.Count <= 1) {
                return true;
            } else {
                return false;
            }
        }

        bool ShowCustomBoolToggle()
        {
            if (hasBool && boolValues.Count <= 1) {
                return true;
            } else {
                return false;
            }
        }

        bool ShowCustomScriptableObjectToggle()
        {
            if (hasScriptableObject && scriptableObjectValues.Count <= 1) {
                return true;
            } else {
                return false;
            }
        }

        bool CheckStringKeys(List<CustomKey> attribute)
        {
            if(attribute.Count != stringValues.Count) {
                return false;
            } else {
                return true;
            }
        }

        bool CheckFloatKeys(List<CustomKey> attribute)
        {
            if (attribute.Count != floatValues.Count) {
                return false;
            } else {
                return true;
            }
        }

        bool CheckBoolKeys(List<CustomKey> attribute)
        {
            if (attribute.Count != boolValues.Count) {
                return false;
            } else {
                return true;
            }
        }

        bool CheckScriptableObjectKeys(List<CustomKey> attribute)
        {
            if (attribute.Count != scriptableObjectValues.Count) {
                return false;
            } else {
                return true;
            }
        }

        bool CheckStringValues(List<string> attribute)
        {
            if(attribute.Count <= 1 && customStringKey == false) {
                return true;
            }
            if (attribute.Count != stringKeys.Count) {
                return false;
            } else {
                return true;
            }
        }

        bool CheckFloatValues(List<float> attribute)
        {
            if (attribute.Count <= 1 && customFloatKey == false) {
                return true;
            }
            if (attribute.Count != floatKeys.Count) {
                return false;
            } else {
                return true;
            }
        }

        bool CheckBoolValues(List<bool> attribute)
        {
            if (attribute.Count <= 1 && customBoolKey == false) {
                return true;
            }
            if (attribute.Count != boolKeys.Count) {
                return false;
            } else {
                return true;
            }
        }

        bool CheckScriptableObjectValues(List<ScriptableObject> attribute)
        {
            if (attribute.Count <= 1 && customScriptableObjectKey == false) {
                return true;
            }
            if (attribute.Count != scriptableObjectKeys.Count) {
                return false;
            } else {
                return true;
            }
        }
#endif
    }
}