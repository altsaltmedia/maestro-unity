using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro
{
    [Serializable]
    [ExecuteInEditMode]
    // Meant to be used within ComplexEventPackagerBehaviour or ComplexEventTimelineTrigger.
    // If implementing a custom trigger from within a script, use the basic ComplexEventTrigger instead.
    public class ComplexEventConfigurableTrigger : ComplexEventReference, IPersistentEventTrigger
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

        [SerializeField]
        [ShowIf(nameof(ShowStringKeys))]
        [ValidateInput(nameof(CheckStringKeys), "Keys and Values must be of equal length")]
        [BoxGroup("String Packager")]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(AlwaysAddDefaultValue = true)]
        private List<CustomKeyReference> _stringKeyReferences = new List<CustomKeyReference>();
        
        public List<CustomKeyReference> stringKeys
        {
            get => _stringKeyReferences;
            set => _stringKeyReferences = value;
        }

        [SerializeField]
        [ShowIf(nameof(hasString))]
        [ValidateInput(nameof(CheckStringValues), "Keys and Values must be of equal length")]
        [BoxGroup("String Packager")]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(AlwaysAddDefaultValue = true)]
        private List<StringReference> _stringValueReferences = new List<StringReference>();

        public List<StringReference> stringValues
        {
            get => _stringValueReferences;
            set => _stringValueReferences = value;
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
        
        
        [SerializeField]
        [ShowIf(nameof(ShowFloatKeys))]
        [ValidateInput(nameof(CheckFloatKeys), "Keys and Values must be of equal length")]
        [BoxGroup("Float Packager")]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(AlwaysAddDefaultValue = true)]
        private List<CustomKeyReference> _floatKeyReferences = new List<CustomKeyReference>();

        public List<CustomKeyReference> floatKeys
        {
            get => _floatKeyReferences;
            set => _floatKeyReferences = value;
        }

        [SerializeField]
        [ShowIf(nameof(hasFloat))]
        [ValidateInput(nameof(CheckFloatValues), "Keys and Values must be of equal length")]
        [BoxGroup("Float Packager")]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(AlwaysAddDefaultValue = true)]
        List<FloatReference> _floatValueReferences = new List<FloatReference>();

        public List<FloatReference> floatValues
        {
            get => _floatValueReferences;
            set => _floatValueReferences = value;
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

        [SerializeField]
        [ShowIf(nameof(ShowBoolKeys))]
        [ValidateInput(nameof(CheckBoolKeys), "Keys and Values must be of equal length")]
        [BoxGroup("Bool Packager")]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(AlwaysAddDefaultValue = true)]
        private List<CustomKeyReference> _boolKeyReferences = new List<CustomKeyReference>();

        public List<CustomKeyReference> boolKeys
        {
            get => _boolKeyReferences;
            set => _boolKeyReferences = value;
        }

        [SerializeField]
        [ShowIf(nameof(hasBool))]
        [ValidateInput(nameof(CheckBoolValues), "Keys and Values must be of equal length")]
        [BoxGroup("Bool Packager")]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(AlwaysAddDefaultValue = true)]
        private List<BoolReference> _boolValueReferences = new List<BoolReference>();

        public List<BoolReference> boolValues
        {
            get => _boolValueReferences;
            set => _boolValueReferences = value;
        }

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

        [SerializeField]
        [ShowIf(nameof(ShowScriptableObjectKeys))]
        [ValidateInput(nameof(CheckScriptableObjectKeys), "Keys and Values must be of equal length")]
        [BoxGroup("Scriptable Object Packager")]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(AlwaysAddDefaultValue = true)]
        private List<CustomKeyReference> _scriptableObjectKeyReferences = new List<CustomKeyReference>();

        public List<CustomKeyReference> scriptableObjectKeys
        {
            get => _scriptableObjectKeyReferences;
            set => _scriptableObjectKeyReferences = value;
        }
        
        [SerializeField]
        [ShowIf(nameof(hasScriptableObject))]
        [ValidateInput(nameof(CheckScriptableObjectValues), "Keys and Values must be of equal length")]
        [BoxGroup("Scriptable Object Packager")]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(AlwaysAddDefaultValue = true)]
        private List<ScriptableObjectReference> _scriptableObjectValueReferences = new List<ScriptableObjectReference>();

        public List<ScriptableObjectReference> scriptableObjectValues
        {
            get => _scriptableObjectValueReferences;
            set => _scriptableObjectValueReferences = value;
        }

#if UNITY_EDITOR        
        public void PopulateReferences(UnityEngine.Object parentObject, string serializedPropertyPath)
        {
            FieldInfo[] fields = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            for (int i = 0; i < fields.Length; i++) {

                if (IsReferenceList(fields[i], out var referenceList) == false) {
                    continue;
                }
                
                string referenceListPath = serializedPropertyPath;
                referenceListPath += $".{fields[i].Name}";
                
                MethodInfo methodInfo = typeof(ReferenceBase).GetMethod(
                    nameof(PopulateVariable), BindingFlags.Public | BindingFlags.Instance, null, new[] {typeof(UnityEngine.Object), typeof(string)}, null );

                for (int j = 0; j < referenceList.Count; j++) {
                    string referencePath = referenceListPath;
                    referencePath += $".{j.ToString()}";
                    methodInfo.Invoke(referenceList[j],
                        new object[] { parentObject, referencePath });
                }
            }
        }

        private bool IsReferenceList(FieldInfo field, out IList actionList)
        {
            actionList = null;
            
            if (field.IsNotSerialized == true) {
                return false;
            }

            var fieldValue = field.GetValue(this);
            if (fieldValue is IList == false) {
                return false;
            }

            actionList = fieldValue as IList;
            var listType = actionList.GetType().GetGenericArguments()[0];

            if (listType.IsSubclassOf(typeof(ReferenceBase)) == false) {
                return false;
            }

            return true;
        }

        public void ResetReferencesSearchAttempted()
        {
            FieldInfo[] fields = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            for (int i = 0; i < fields.Length; i++) {

                if (IsReferenceList(fields[i], out var referenceList) == false) {
                    continue;
                }

                MethodInfo methodInfo = typeof(ReferenceBase).GetMethod(
                    nameof(ResetSearchAttempted), BindingFlags.Public | BindingFlags.Instance);

                for (int j = 0; j < referenceList.Count; j++) {
                    methodInfo.Invoke(referenceList[j],
                        new object[] {} );
                }
            }
        }
#endif        

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
            (GetVariable() as ComplexEvent).StoreCaller(caller);
            (GetVariable() as ComplexEvent).Raise(complexPayload);
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
            (GetVariable() as ComplexEvent).StoreCaller(caller, sourceName);
            (GetVariable() as ComplexEvent).Raise(complexPayload);
        }

        private ComplexPayload GetStringValues(ComplexPayload complexPayload)
        {
            if (stringValues.Count == 0) {
                LogWarning();
                throw new Exception("Event payload could not be created");
            }

            if (stringValues.Count == 1) {
                if (customStringKey == true && stringKeys.Count >= 1) {
                    complexPayload.Set(stringKeys[0].GetVariable(), stringValues[0].GetValue());
                } else {
                    complexPayload.Set(DataType.stringType, stringValues[0].GetValue());
                }
            } else {
                for (int i = 0; i < stringValues.Count; i++) {
                    complexPayload.Set(stringKeys[i].GetVariable(), stringValues[i].GetValue());
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
                    complexPayload.Set(floatKeys[0].GetVariable(), floatValues[0].GetValue());
                } else {
                    complexPayload.Set(DataType.floatType, floatValues[0].GetValue());
                }
            } else {
                for (int i = 0; i < floatValues.Count; i++) {
                    complexPayload.Set(floatKeys[i].GetVariable(), floatValues[i].GetValue());
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
                    complexPayload.Set(boolKeys[0].GetVariable(), boolValues[0].GetValue());
                } else {
                    complexPayload.Set(DataType.boolType, boolValues[0].GetValue());
                }
            } else {
                for (int i = 0; i < boolValues.Count; i++) {
                    complexPayload.Set(boolKeys[i].GetVariable(), boolValues[i].GetValue());
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
                    complexPayload.Set(scriptableObjectKeys[0].GetVariable(), scriptableObjectValues[0].GetVariable());
                } else {
                    complexPayload.Set(DataType.scriptableObjectType, scriptableObjectValues[0].GetVariable());
                }
            } else {
                for (int i = 0; i < scriptableObjectValues.Count; i++) {
                    complexPayload.Set(scriptableObjectKeys[i].GetVariable(), scriptableObjectValues[i].GetVariable());
                }
            }
            return complexPayload;
        }

        protected virtual void LogWarning()
        {
            Debug.LogWarning("Failed to create event payload");
        }
        
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
        
        private bool CheckStringKeys(List<CustomKeyReference> attribute)
        {
            if(attribute.Count != stringValues.Count) {
                return false;
            } else {
                return true;
            }
        }

        private bool CheckFloatKeys(List<CustomKeyReference> attribute)
        {
            if (attribute.Count != floatValues.Count) {
                return false;
            } else {
                return true;
            }
        }

        private bool CheckBoolKeys(List<CustomKeyReference> attribute)
        {
            if (attribute.Count != boolValues.Count) {
                return false;
            } else {
                return true;
            }
        }

        private bool CheckScriptableObjectKeys(List<CustomKeyReference> attribute)
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
        
        private bool CheckStringValues(List<StringReference> attribute)
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

        private bool CheckFloatValues(List<FloatReference> attribute)
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

        private bool CheckBoolValues(List<BoolReference> attribute)
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

        private bool CheckScriptableObjectValues(List<ScriptableObjectReference> attribute)
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

    }
}