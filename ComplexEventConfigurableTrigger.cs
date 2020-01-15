using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro
{
    [Serializable]
    [ExecuteInEditMode]
    // Meant to be used within ComplexEventPackagerBehaviour or ComplexEventTimelineTrigger.
    // If implementing a custom trigger from within a script, use the basic ComplexEventTrigger instead.
    public class ComplexEventConfigurableTrigger : ComplexEventReference
    {
        [SerializeField]
        [BoxGroup("String Packager")]
        [FormerlySerializedAs("hasString")]
        private bool _hasString;

        public bool hasString
        {
            get => _hasString;
            set => _hasString = value;
        }

        [SerializeField]
        [ShowIf(nameof(ShowCustomStringToggle))]
        [BoxGroup("String Packager", false)]
        [FormerlySerializedAs("customStringKey")]
        private bool _customStringKey;

        public bool customStringKey
        {
            get => _customStringKey;
            set => _customStringKey = value;
        }

#if UNITY_EDITOR
        [SerializeField]
        [ShowIf(nameof(hasString))]
        [Multiline]
        [BoxGroup("String Packager")]
        [FormerlySerializedAs("stringDescription")]
        private string _stringDescription = "";

        private string stringDescription
        {
            get => _stringDescription;
            set => _stringDescription = value;
        }
#endif

        [SerializeField]
        [ShowIf(nameof(ShowStringKeys))]
        [ValidateInput(nameof(CheckStringKeys), "Keys and Values must be of equal length")]
        [BoxGroup("String Packager")]
        [FormerlySerializedAs("stringKeys")]
        private List<CustomKey> _stringKeys = new List<CustomKey>();

        public List<CustomKey> stringKeys
        {
            get => _stringKeys;
            set => _stringKeys = value;
        }

        [SerializeField]
        [ShowIf(nameof(hasString))]
        [ValidateInput(nameof(CheckStringValues), "Keys and Values must be of equal length")]
        [BoxGroup("String Packager")]
        [FormerlySerializedAs("stringValues")]
        private List<string> _stringValues = new List<string>();

        public List<string> stringValues
        {
            get => _stringValues;
            set => _stringValues = value;
        }

        [PropertySpace]

        [SerializeField]
        [BoxGroup("Float Packager", false)]
        [FormerlySerializedAs("hasFloat")]
        private bool _hasFloat;

        public bool hasFloat
        {
            get => _hasFloat;
            set => _hasFloat = value;
        }

        [SerializeField]
        [ShowIf(nameof(ShowCustomFloatToggle))]
        [BoxGroup("Float Packager")]
        [FormerlySerializedAs("customFloatKey")]
        private bool _customFloatKey;

        public bool customFloatKey
        {
            get => _customFloatKey;
            set => _customFloatKey = value;
        }

#if UNITY_EDITOR
        [SerializeField]
        [ShowIf(nameof(hasFloat))]
        [Multiline]
        [BoxGroup("Float Packager")]
        [FormerlySerializedAs("floatDescription")]
        private string _floatDescription = "";

        public string floatDescription
        {
            get => _floatDescription;
            set => _floatDescription = value;
        }
#endif

        [SerializeField]
        [ShowIf(nameof(ShowFloatKeys))]
        [ValidateInput(nameof(CheckFloatKeys), "Keys and Values must be of equal length")]
        [BoxGroup("Float Packager")]
        [FormerlySerializedAs("floatKeys")]
        private List<CustomKey> _floatKeys = new List<CustomKey>();

        public List<CustomKey> floatKeys
        {
            get => _floatKeys;
            set => _floatKeys = value;
        }

        [SerializeField]
        [ShowIf(nameof(hasFloat))]
        [ValidateInput(nameof(CheckFloatValues), "Keys and Values must be of equal length")]
        [BoxGroup("Float Packager")]
        [FormerlySerializedAs("floatValues")]
        List<float> _floatValues = new List<float>();

        public List<float> floatValues
        {
            get => _floatValues;
            set => _floatValues = value;
        }

        [PropertySpace]

        [SerializeField]
        [BoxGroup("Bool Packager", false)]
        [FormerlySerializedAs("hasBool")]
        private bool _hasBool;

        public bool hasBool
        {
            get => _hasBool;
            set => _hasBool = value;
        }

        [SerializeField]
        [ShowIf(nameof(ShowCustomBoolToggle))]
        [BoxGroup("Bool Packager")]
        [FormerlySerializedAs("customBoolKey")]
        private bool _customBoolKey;

        public bool customBoolKey
        {
            get => _customBoolKey;
            set => _customBoolKey = value;
        }

#if UNITY_EDITOR
        [SerializeField]
        [ShowIf(nameof(hasBool))]
        [Multiline]
        [BoxGroup("Bool Packager")]
        [FormerlySerializedAs("boolDescription")]
        private string _boolDescription = "";

        public string boolDescription
        {
            get => _boolDescription;
            set => _boolDescription = value;
        }
#endif

        [SerializeField]
        [ShowIf(nameof(ShowBoolKeys))]
        [ValidateInput(nameof(CheckBoolKeys), "Keys and Values must be of equal length")]
        [BoxGroup("Bool Packager")]
        [FormerlySerializedAs("boolKeys")]
        private List<CustomKey> _boolKeys = new List<CustomKey>();

        public List<CustomKey> boolKeys
        {
            get => _boolKeys;
            set => _boolKeys = value;
        }

        [SerializeField]
        [ShowIf(nameof(hasBool))]
        [ValueDropdown(nameof(boolValueList))]
        [ValidateInput(nameof(CheckBoolValues), "Keys and Values must be of equal length")]
        [BoxGroup("Bool Packager")]
        [FormerlySerializedAs(nameof(boolValues))]
        private List<bool> _boolValues = new List<bool>();

        public List<bool> boolValues
        {
            get => _boolValues;
            set => _boolValues = value;
        }

        private ValueDropdownList<bool> boolValueList = new ValueDropdownList<bool>(){
                {"FALSE", false },
                {"TRUE", true }
            };

        [PropertySpace]

        [SerializeField]
        [BoxGroup("Scriptable Object Packager", false)]
        [FormerlySerializedAs("hasScriptableObject")]
        private bool _hasScriptableObject;

        public bool hasScriptableObject
        {
            get => _hasScriptableObject;
            set => _hasScriptableObject = value;
        }

        [SerializeField]
        [ShowIf(nameof(ShowCustomScriptableObjectToggle))]
        [BoxGroup("Scriptable Object Packager")]
        [FormerlySerializedAs("customScriptableObjectKey")]
        private bool _customScriptableObjectKey;

        public bool customScriptableObjectKey
        {
            get => _customScriptableObjectKey;
            set => _customScriptableObjectKey = value;
        }

#if UNITY_EDITOR
        [SerializeField]
        [ShowIf(nameof(hasScriptableObject))]
        [Multiline]
        [BoxGroup("Scriptable Object Packager")]
        [FormerlySerializedAs("scriptableObjectDescription")]
        private string _scriptableObjectDescription = "";

        public string scriptableObjectDescription
        {
            get => _scriptableObjectDescription;
            set => _scriptableObjectDescription = value;
        }
#endif

        [SerializeField]
        [ShowIf(nameof(ShowScriptableObjectKeys))]
        [ValidateInput(nameof(CheckScriptableObjectKeys), "Keys and Values must be of equal length")]
        [BoxGroup("Scriptable Object Packager")]
        [FormerlySerializedAs("scriptableObjectKeys")]
        private List<CustomKey> _scriptableObjectKeys = new List<CustomKey>();

        public List<CustomKey> scriptableObjectKeys
        {
            get => _scriptableObjectKeys;
            set => _scriptableObjectKeys = value;
        }

        [SerializeField]
        [ShowIf(nameof(hasScriptableObject))]
        [ValidateInput(nameof(CheckScriptableObjectValues), "Keys and Values must be of equal length")]
        [BoxGroup("Scriptable Object Packager")]
        [FormerlySerializedAs("scriptableObjectValues")]
        private List<ScriptableObject> _scriptableObjectValues = new List<ScriptableObject>();

        public List<ScriptableObject> scriptableObjectValues
        {
            get => _scriptableObjectValues;
            set => _scriptableObjectValues = value;
        }

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
            GetVariable(caller).StoreCaller(caller);
            GetVariable(caller).Raise(complexPayload);
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
            GetVariable(caller).StoreCaller(caller, sourceName);
            GetVariable(caller).Raise(complexPayload);
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