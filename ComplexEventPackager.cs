using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro
{
    [Serializable]
    [ExecuteInEditMode]
    // Meant to be used within ComplexEventPackagerBehaviour or ComplexEventTimelineTrigger.
    // If implementing a custom trigger from within a script, use the basic ComplexEventTrigger instead.
    public class ComplexEventPackager : EventTriggerBase
    {
        [Required]
        [SerializeField]
        ComplexEvent complexEvent;

        [SerializeField]
        [BoxGroup("String Packager")]
        bool hasString;

        [SerializeField]
        [ShowIf(nameof(ShowCustomStringToggle))]
        [BoxGroup("String Packager", false)]
        bool customStringKey;

#if UNITY_EDITOR
        [SerializeField]
        [ShowIf(nameof(hasString))]
        [Multiline]
        [BoxGroup("String Packager")]
        string stringDescription = "";
#endif

        [SerializeField]
        [ShowIf(nameof(ShowStringKeys))]
        [ValidateInput(nameof(CheckStringKeys), "Keys and Values must be of equal length")]
        [BoxGroup("String Packager")]
        List<CustomKey> stringKeys = new List<CustomKey>();

        [SerializeField]
        [ShowIf(nameof(hasString))]
        [ValidateInput(nameof(CheckStringValues), "Keys and Values must be of equal length")]
        [BoxGroup("String Packager")]
        List<string> stringValues = new List<string>();

        [PropertySpace]

        [SerializeField]
        [BoxGroup("Float Packager", false)]
        bool hasFloat;

        [SerializeField]
        [ShowIf(nameof(ShowCustomFloatToggle))]
        [BoxGroup("Float Packager")]
        bool customFloatKey;

#if UNITY_EDITOR
        [SerializeField]
        [ShowIf(nameof(hasFloat))]
        [Multiline]
        [BoxGroup("Float Packager")]
        string floatDescription = "";
#endif

        [SerializeField]
        [ShowIf(nameof(ShowFloatKeys))]
        [ValidateInput(nameof(CheckFloatKeys), "Keys and Values must be of equal length")]
        [BoxGroup("Float Packager")]
        List<CustomKey> floatKeys = new List<CustomKey>();

        [SerializeField]
        [ShowIf(nameof(hasFloat))]
        [ValidateInput(nameof(CheckFloatValues), "Keys and Values must be of equal length")]
        [BoxGroup("Float Packager")]
        List<float> floatValues = new List<float>();

        [PropertySpace]

        [SerializeField]
        [BoxGroup("Bool Packager", false)]
        bool hasBool;

        [SerializeField]
        [ShowIf(nameof(ShowCustomBoolToggle))]
        [BoxGroup("Bool Packager")]
        bool customBoolKey;

#if UNITY_EDITOR
        [SerializeField]
        [ShowIf(nameof(hasBool))]
        [Multiline]
        [BoxGroup("Bool Packager")]
        string boolDescription = "";
#endif

        [SerializeField]
        [ShowIf(nameof(ShowBoolKeys))]
        [ValidateInput(nameof(CheckBoolKeys), "Keys and Values must be of equal length")]
        [BoxGroup("Bool Packager")]
        List<CustomKey> boolKeys = new List<CustomKey>();

        [SerializeField]
        [ShowIf(nameof(hasBool))]
        [ValueDropdown(nameof(boolValueList))]
        [ValidateInput(nameof(CheckBoolValues), "Keys and Values must be of equal length")]
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
        [ShowIf(nameof(ShowCustomScriptableObjectToggle))]
        [BoxGroup("Scriptable Object Packager")]
        bool customScriptableObjectKey;

#if UNITY_EDITOR
        [SerializeField]
        [ShowIf(nameof(hasScriptableObject))]
        [Multiline]
        [BoxGroup("Scriptable Object Packager")]
        string scriptableObjectDescription = "";
#endif

        [SerializeField]
        [ShowIf(nameof(ShowScriptableObjectKeys))]
        [ValidateInput(nameof(CheckScriptableObjectKeys), "Keys and Values must be of equal length")]
        [BoxGroup("Scriptable Object Packager")]
        List<CustomKey> scriptableObjectKeys = new List<CustomKey>();

        [SerializeField]
        [ShowIf(nameof(hasScriptableObject))]
        [ValidateInput(nameof(CheckScriptableObjectValues), "Keys and Values must be of equal length")]
        [BoxGroup("Scriptable Object Packager")]
        List<ScriptableObject> scriptableObjectValues = new List<ScriptableObject>();

        public void RaiseEvent(GameObject caller)
        {
            ComplexPayload complexPayload = ComplexPayload.CreateInstance();
            if (hasString) {
                complexPayload = GetStringValues(complexPayload);
            }
            if (hasFloat) {
                complexPayload = GetFloatValues(complexPayload);
            }
            if (hasBool) {
                complexPayload = GetBoolValues(complexPayload);
            }
            if (hasScriptableObject) {
                complexPayload = GetScriptableObjectValues(complexPayload);
            }
            complexEvent.StoreCaller(caller);
            complexEvent.Raise(complexPayload);
        }

        public void RaiseEvent(GameObject caller, string sourceName)
        {
            ComplexPayload complexPayload = ComplexPayload.CreateInstance();
            if (hasString) {
                complexPayload = GetStringValues(complexPayload);
            }
            if (hasFloat) {
                complexPayload = GetFloatValues(complexPayload);
            }
            if (hasBool) {
                complexPayload = GetBoolValues(complexPayload);
            }
            if (hasScriptableObject) {
                complexPayload = GetScriptableObjectValues(complexPayload);
            }
            complexEvent.StoreCaller(caller, sourceName);
            complexEvent.Raise(complexPayload);
        }

        private ComplexPayload GetStringValues(ComplexPayload complexPayload)
        {
            if (stringValues.Count == 0) {
                LogWarning();
                throw new Exception("Event payload could not be created");
            }

            if (stringValues.Count == 1) {
                if (customStringKey == true && stringKeys.Count >= 1) {
                    complexPayload.Set(stringKeys[0], stringValues[0]);
                } else {
                    complexPayload.Set(DataType.stringType, stringValues[0]);
                }
            } else {
                for (int i = 0; i < stringValues.Count; i++) {
                    complexPayload.Set(stringKeys[i], stringValues[i]);
                }
            }
            return complexPayload;
        }

        private ComplexPayload GetFloatValues(ComplexPayload complexPayload)
        {
            if (floatValues.Count == 0) {
                LogWarning();
                throw new Exception("Event payload could not be created");
            }

            if (floatValues.Count == 1) {
                if (customFloatKey == true && floatKeys.Count >= 1) {
                    complexPayload.Set(floatKeys[0], floatValues[0]);
                } else {
                    complexPayload.Set(DataType.floatType, floatValues[0]);
                }
            } else {
                for (int i = 0; i < floatValues.Count; i++) {
                    complexPayload.Set(floatKeys[i], floatValues[i]);
                }
            }
            return complexPayload;
        }

        private ComplexPayload GetBoolValues(ComplexPayload complexPayload)
        {
            if (boolValues.Count == 0) {
                LogWarning();
                throw new Exception("Event payload could not be created");
            }

            if (boolValues.Count == 1) {
                if (customBoolKey == true && boolKeys.Count >= 1) {
                    complexPayload.Set(boolKeys[0], boolValues[0]);
                } else {
                    complexPayload.Set(DataType.boolType, boolValues[0]);
                }
            } else {
                for (int i = 0; i < boolValues.Count; i++) {
                    complexPayload.Set(boolKeys[i], boolValues[i]);
                }
            }
            return complexPayload;
        }

        private ComplexPayload GetScriptableObjectValues(ComplexPayload complexPayload)
        {
            if (scriptableObjectValues.Count == 0) {
                LogWarning();
                throw new Exception("Event payload could not be created");
            }

            if (scriptableObjectValues.Count == 1) {
                if (customScriptableObjectKey == true && scriptableObjectKeys.Count >= 1) {
                    complexPayload.Set(scriptableObjectKeys[0], scriptableObjectValues[0]);
                } else {
                    complexPayload.Set(DataType.scriptableObjectType, scriptableObjectValues[0]);
                }
            } else {
                for (int i = 0; i < scriptableObjectValues.Count; i++) {
                    complexPayload.Set(scriptableObjectKeys[i], scriptableObjectValues[i]);
                }
            }
            return complexPayload;
        }

        protected virtual void LogWarning()
        {
            Debug.LogWarning("Failed to create event payload");
        }

#if UNITY_EDITOR
        private bool ShowStringKeys()
        {
            if (hasString && stringValues != null && stringValues.Count > 1) {
                return true;
            }

            if(customStringKey == true) {
                return true;
            }

            return false;
        }

        private bool ShowFloatKeys()
        {
            if (hasFloat && floatValues != null && floatValues.Count > 1) {
                return true;
            }

            if(customFloatKey) {
                return true;
            }

            return false;
        }

        private bool ShowBoolKeys()
        {
            if (hasBool && boolValues != null && boolValues.Count > 1) {
                return true;
            }

            if(customBoolKey) {
                return true;
            }

            return false;
        }

        private bool ShowScriptableObjectKeys()
        {
            if (hasScriptableObject && scriptableObjectValues != null && scriptableObjectValues.Count > 1) {
                return true;
            }

            if(customScriptableObjectKey) {
                return true;
            }

            return false;
        }

        private bool ShowCustomStringToggle()
        {
            if(hasString && stringValues.Count <= 1) {
                return true;
            } else {
                return false;
            }
        }

        private bool ShowCustomFloatToggle()
        {
            if (hasFloat && floatValues.Count <= 1) {
                return true;
            } else {
                return false;
            }
        }

        private bool ShowCustomBoolToggle()
        {
            if (hasBool && boolValues.Count <= 1) {
                return true;
            } else {
                return false;
            }
        }

        private bool ShowCustomScriptableObjectToggle()
        {
            if (hasScriptableObject && scriptableObjectValues.Count <= 1) {
                return true;
            } else {
                return false;
            }
        }

        private bool CheckStringKeys(List<CustomKey> attribute)
        {
            if(attribute.Count != stringValues.Count) {
                return false;
            } else {
                return true;
            }
        }

        private bool CheckFloatKeys(List<CustomKey> attribute)
        {
            if (attribute.Count != floatValues.Count) {
                return false;
            } else {
                return true;
            }
        }

        private bool CheckBoolKeys(List<CustomKey> attribute)
        {
            if (attribute.Count != boolValues.Count) {
                return false;
            } else {
                return true;
            }
        }

        private bool CheckScriptableObjectKeys(List<CustomKey> attribute)
        {
            if (attribute.Count != scriptableObjectValues.Count) {
                return false;
            } else {
                return true;
            }
        }

        private bool CheckStringValues(List<string> attribute)
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

        private bool CheckFloatValues(List<float> attribute)
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

        private bool CheckBoolValues(List<bool> attribute)
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

        private bool CheckScriptableObjectValues(List<ScriptableObject> attribute)
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