using System;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace AltSalt.Maestro
{
    [CreateAssetMenu(menuName = "Maestro/Settings/Debug Preferences")]
    public class DebugPreferences : ScriptableObject
    {
        [SerializeField]
        private float _timelineDebugTime;
        
        public float timelineDebugTime
        {
            get => _timelineDebugTime;
            set => _timelineDebugTime = value;
        }
        
        [SerializeField]
        private SimpleEventTrigger _onEditorGraphStart = new SimpleEventTrigger();

        public SimpleEventTrigger onEditorGraphStart => _onEditorGraphStart;
        
        [SerializeField]
        private bool _dynamicLayoutActive = true;
        
        public bool dynamicLayoutActive
        {
            get => _dynamicLayoutActive;
            set => _dynamicLayoutActive = value;
        }
        
        [SerializeField]
        private bool _modifyTextActive = true;

        public bool modifyTextActive
        {
            get => _modifyTextActive;
            set => _modifyTextActive = value;
        }

        [SerializeField]
        private bool _modifyLayoutActive = true;

        public bool modifyLayoutActive
        {
            get => _modifyLayoutActive;
            set => _modifyLayoutActive = value;
        }
        
        [SerializeField]
        private bool _saveDataActive = true;
        
        public bool saveDataActive
        {
            get => _saveDataActive;
            set => _saveDataActive = value;
        }
        
        [SerializeField]
        private bool _useAddressables = false;

        public bool useAddressables
        {
            get => _useAddressables;
            set => _useAddressables = value;
        }
        
        [SerializeField]
        private bool _bookmarkingEnabled = false;

        public bool bookmarkingEnabled
        {
            get => _bookmarkingEnabled;
            set => _bookmarkingEnabled = value;
        }
        
        [ShowInInspector]
        private static bool _logEventCallersAndListeners = false;
        
        public static bool logEventCallersAndListeners
        {
            get => _logEventCallersAndListeners;
            set => _logEventCallersAndListeners = value;
        }
        
        [ShowInInspector]
        private static bool _logResponsiveElementActions = false;

        public static bool logResponsiveElementActions
        {
            get => _logResponsiveElementActions;
            set => _logResponsiveElementActions = value;
        }
        
        [ShowInInspector]
        private static bool _logConditionResponses = false;

        public static bool logConditionResponses
        {
            get => _logConditionResponses;
            set => _logConditionResponses = value;
        }

        private bool _playStartedFromInitializer;

        public bool playStartedFromInitializer
        {
            get => _playStartedFromInitializer;
            set => _playStartedFromInitializer = value;
        }

        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void SetDefaults()
        {
            dynamicLayoutActive = true;
            modifyTextActive = true;
            modifyLayoutActive = true;
            saveDataActive = true;
            useAddressables = false;
            bookmarkingEnabled = false;
            logEventCallersAndListeners = false;
            logResponsiveElementActions = false;
            logConditionResponses = false;
        }

        private void OnEnable()
        {
            playStartedFromInitializer = false;
        }

#if UNITY_EDITOR
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void RefreshDependencies()
        {
            FieldInfo[] fields = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

            for (int i = 0; i < fields.Length; i++) {
                var fieldValue = fields[i].GetValue(this);
                if (fieldValue.GetType().IsSubclassOf(typeof(ReferenceBase)) == true) {
                    
                    (fieldValue as ReferenceBase).isSystemReference = true;
                    
                    FieldInfo variableField = Utils.GetVariableFieldFromReference(fields[i], this, out var referenceValue);
                    
                    var variableValue = variableField.GetValue(referenceValue) as ScriptableObject;
                    if (variableValue == null) {
                        
                        string variableName = fields[i].Name.Replace("_", "").Capitalize();
                        var searchVariable = Utils.GetScriptableObject(variableName);
                        
                        if (searchVariable == null) {
                            Type type = variableField.FieldType;
                            variableField.SetValue(referenceValue, CreateDebugPreference(type, variableName));
                        }
                        else {
                            variableField.SetValue(referenceValue, searchVariable);
                        }
                    }
                }
            }
            
            EditorUtility.SetDirty(this);
        }
        
        private static dynamic CreateDebugPreference(Type assetType, string name)
        {
            return Utils.ForceCreateScriptableObjectAsset(assetType, name, $"{Utils.settingsPath}/DebugEvents");
        }
#endif

    }
}