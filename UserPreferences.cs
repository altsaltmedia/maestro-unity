using System;
using System.Reflection;
using Sirenix.OdinInspector;
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
        private BoolReference _userAutoplayEnabled;

        public BoolReference userAutoplayEnabled
        {
            get
            {
                UpdateDependencies();
                return _userAutoplayEnabled;
            }
        }
        
        [SerializeField, Required]
        private BoolReference _userMomentumEnabled;

        public BoolReference userMomentumEnabled
        {
            get
            {
                UpdateDependencies();
                return _userMomentumEnabled;
            }
        }

        public UserPreferences(CustomKey userKey)
        {
#if UNITY_EDITOR
            FieldInfo[] fields = typeof(InputGroup).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

            for (int i = 0; i < fields.Length; i++) {
                
                var varReference = fields[i].GetValue(this);
                string name = fields[i].Name.Replace("_", "").Capitalize();
                
                var variableField = varReference.GetType().GetField("_variable", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                variableField.SetValue(varReference, CreateProfileDependency(variableField.FieldType, $"{userKey.name}-{name}", userKey.name));
                
            }
#endif
        }
        
#if UNITY_EDITOR
        private static dynamic CreateProfileDependency(Type assetType, string name, string userName)
        {
            return Utils.CreateScriptableObjectAsset(assetType, name, $"{Utils.settingsPath}/InputSettings/{userName}");
        }
#endif
    }
}