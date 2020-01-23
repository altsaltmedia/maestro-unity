using System;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace AltSalt.Maestro
{
    [Serializable]
    public class UserPreferences
    {
        [SerializeField]
        [FoldoutGroup("Configuration")]
        private FloatReference _ySensitivity = new FloatReference();

        public FloatReference ySensitivity
        {
            get => _ySensitivity;
            set => _ySensitivity = value;
        }
        
        [SerializeField]
        [FoldoutGroup("Configuration")]
        private BoolReference _invertYInput = new BoolReference();

        public BoolReference invertYInput
        {
            get => _invertYInput;
            set => _invertYInput = value;
        }
        
        [SerializeField]
        [FoldoutGroup("Configuration")]
        private FloatReference _xSensitivity = new FloatReference();

        public FloatReference xSensitivity
        {
            get => _xSensitivity;
            set => _xSensitivity = value;
        }
        
        [SerializeField]
        [FoldoutGroup("Configuration")]
        private BoolReference _invertXInput = new BoolReference();

        public BoolReference invertXInput
        {
            get => _invertXInput;
            set => _invertXInput = value;
        }

        [SerializeField, Required]
        private BoolReference _userAutoplayEnabled = new BoolReference();

        public BoolReference userAutoplayEnabled
        {
            get => _userAutoplayEnabled;
            set => _userAutoplayEnabled = value;
        }

        [SerializeField, Required]
        private BoolReference _userMomentumEnabled = new BoolReference();

        public BoolReference userMomentumEnabled
        {
            get => _userMomentumEnabled;
            set => _userMomentumEnabled = value;
        }

        public UserPreferences(UserData userData, UserDataKey userKey)
        {
#if UNITY_EDITOR
            RefreshDependencies(userKey);
            SetDefaults(userData, userKey);
#endif
        }
        
        public void RefreshDependencies(UserDataKey userKey)
        {
            FieldInfo[] referenceFields = typeof(UserPreferences).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

            for (int i = 0; i < referenceFields.Length; i++) {
                
                string name = referenceFields[i].Name.Replace("_", "").Capitalize();
                var variableField = Utils.GetVariableFieldFromReference(referenceFields[i], this, out var referenceValue);
                
                var variableValue = variableField.GetValue(referenceValue) as ScriptableObject;

                if (variableValue == null) {
                    variableField.SetValue(referenceValue,
                        CreateUserPreference(variableField.FieldType, $"{userKey.name}-{name}", userKey.name));
                }
            }
        }

        public UserPreferences SetDefaults(UserData userData, UserDataKey userKey)
        {
            FieldInfo[] fields = typeof(UserPreferences).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

            for (int i = 0; i < fields.Length; i++) {
                var referenceFieldValue = fields[i].GetValue(this) as ReferenceBase;
                referenceFieldValue.isSystemReference = true;
            }

            (ySensitivity.GetVariable() as FloatVariable).defaultValue = 0.0027f;
            (invertYInput.GetVariable() as BoolVariable).defaultValue = false;
            (xSensitivity.GetVariable() as FloatVariable).defaultValue = 0.0054f;
            (invertXInput.GetVariable() as BoolVariable).defaultValue = true;
            (userAutoplayEnabled.GetVariable() as BoolVariable).defaultValue = true;
            (userMomentumEnabled.GetVariable() as BoolVariable).defaultValue = true;
            
            for (int i = 0; i < fields.Length; i++) {
                
                var variableField = Utils.GetVariableFieldFromReference(fields[i], this, out var referenceValue);
                var variableValue = variableField.GetValue(referenceValue);
                
                if (variableValue is ModifiableEditorVariable modifiableEditorVariable) {
                    modifiableEditorVariable.StoreCaller(userData, "setting default from refresh dependencies",
                        "app settings");
                    modifiableEditorVariable.hasDefault = true;
                    modifiableEditorVariable.SetToDefaultValue();
                }
            }

            return this;
        }
        
#if UNITY_EDITOR
        private static dynamic CreateUserPreference(Type assetType, string name, string userName)
        {
            return Utils.CreateScriptableObjectAsset(assetType, name, $"{Utils.settingsPath}/UsetData/{userName}");
        }
#endif
    }
}