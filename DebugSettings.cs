using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace AltSalt.Maestro
{
    [CreateAssetMenu(menuName = "AltSalt/Debug Settings")]
    public class DebugSettings : ScriptableObject
    {
        
        [SerializeField, Required]
        private BoolVariable _responsiveLayoutActive;
        
        public BoolVariable responsiveLayoutActive
        {
            get
            {
                UpdateDependencies();
                return _responsiveLayoutActive;
            }
            private set => _responsiveLayoutActive = value;
        }
        
        [SerializeField, Required]
        private BoolVariable _modifyTextActive;

        public BoolVariable modifyTextActive
        {
            get
            {
                UpdateDependencies();
                return _modifyTextActive;
            }
            private set => _modifyTextActive = value;
        }

        [SerializeField, Required]
        private BoolVariable _modifyLayoutActive;

        public BoolVariable modifyLayoutActive
        {
            get
            {
                UpdateDependencies();
                return _modifyLayoutActive;
            }
            private set => _modifyLayoutActive = value;
        }
        
        [SerializeField, Required]
        private BoolVariable _saveDataActive;
        
        public BoolVariable saveDataActive
        {
            get
            {
                UpdateDependencies();
                return _saveDataActive;
            }
            private set => _saveDataActive = value;
        }
        
        [SerializeField, Required]
        private BoolVariable _useAddressables;

        public BoolVariable useAddressables
        {
            get
            {
                UpdateDependencies();
                return _useAddressables;
            }
            private set => _useAddressables = value;
        }
        
        [SerializeField, Required]
        private BoolVariable _logEventCallersAndListeners;
        
        public BoolVariable logEventCallersAndListeners
        {
            get
            {
                UpdateDependencies();
                return _logEventCallersAndListeners;
            }
            private set => _logEventCallersAndListeners = value;
        }
        
        [SerializeField, Required]
        private BoolVariable _logResponsiveElementActions;

        public BoolVariable logResponsiveElementActions
        {
            get
            {
                UpdateDependencies();
                return _logResponsiveElementActions;
            }
            private set => _logResponsiveElementActions = value;
        }
        
        [SerializeField, Required]
        private BoolVariable _logConditionResponses;

        public BoolVariable logConditionResponses
        {
            get
            {
                UpdateDependencies();
                return _logConditionResponses;
            }
            private set => _logConditionResponses = value;
        }
        
        [InfoBox("Finds and / or creates debug settings.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void UpdateDependencies()
        {
#if UNITY_EDITOR
            if (_responsiveLayoutActive == null) {
                var variable = Utils.GetBoolVariable(nameof(responsiveLayoutActive).Capitalize());
                if (variable != null) {
                    _responsiveLayoutActive = variable;
                }
                else {
                    _responsiveLayoutActive = CreateDebugSetting(typeof(BoolVariable), nameof(responsiveLayoutActive).Capitalize()) as BoolVariable;
                    _responsiveLayoutActive.SetValue(true);
                    _responsiveLayoutActive.hasDefault = true;
                    _responsiveLayoutActive.defaultValue = true;
                }
            }
            
            if (_modifyTextActive == null) {
                var variable = Utils.GetBoolVariable(nameof(modifyTextActive).Capitalize());
                if (variable != null) {
                    _modifyTextActive = variable;
                }
                else {
                    _modifyTextActive = CreateDebugSetting(typeof(BoolVariable), nameof(modifyTextActive).Capitalize()) as BoolVariable;
                    _modifyTextActive.SetValue(true);
                    _modifyTextActive.hasDefault = true;
                    _modifyTextActive.defaultValue = true;
                }
            }
            
            if (_modifyLayoutActive == null) {
                var variable = Utils.GetBoolVariable(nameof(modifyLayoutActive).Capitalize());
                if (variable != null) {
                    _modifyLayoutActive = variable;
                }
                else {
                    _modifyLayoutActive = CreateDebugSetting(typeof(BoolVariable), nameof(modifyLayoutActive).Capitalize()) as BoolVariable;
                    _modifyLayoutActive.SetValue(true);
                    _modifyLayoutActive.hasDefault = true;
                    _modifyLayoutActive.defaultValue = true;
                }
            }
            
            if (_saveDataActive == null) {
                var variable = Utils.GetBoolVariable(nameof(saveDataActive).Capitalize());
                if (variable != null) {
                    _saveDataActive = variable;
                }
                else {
                    _saveDataActive = CreateDebugSetting(typeof(BoolVariable), nameof(saveDataActive).Capitalize()) as BoolVariable;
                    _saveDataActive.SetValue(true);
                    _saveDataActive.hasDefault = true;
                    _saveDataActive.defaultValue = true;
                }
            }
            
            if (_useAddressables == null) {
                var variable = Utils.GetBoolVariable(nameof(useAddressables).Capitalize());
                if (variable != null) {
                    _useAddressables = variable;
                }
                else {
                    _useAddressables = CreateDebugSetting(typeof(BoolVariable), nameof(useAddressables).Capitalize()) as BoolVariable;
                    _useAddressables.SetValue(false);
                    _useAddressables.hasDefault = true;
                    _useAddressables.defaultValue = false;
                }
            }
            
            if (_logEventCallersAndListeners == null) {
                var variable = Utils.GetBoolVariable(nameof(logEventCallersAndListeners).Capitalize());
                if (variable != null) {
                    _logEventCallersAndListeners = variable;
                }
                else {
                    _logEventCallersAndListeners = CreateDebugSetting(typeof(BoolVariable), nameof(logEventCallersAndListeners).Capitalize()) as BoolVariable;
                    _logEventCallersAndListeners.SetValue(false);
                    _logEventCallersAndListeners.hasDefault = true;
                    _logEventCallersAndListeners.defaultValue = false;
                }
            }
            
            if (_logResponsiveElementActions == null) {
                var variable = Utils.GetBoolVariable(nameof(logResponsiveElementActions).Capitalize());
                if (variable != null) {
                    _logResponsiveElementActions = variable;
                }
                else {
                    _logResponsiveElementActions = CreateDebugSetting(typeof(BoolVariable), nameof(logResponsiveElementActions).Capitalize()) as BoolVariable;
                    _logResponsiveElementActions.SetValue(false);
                    _logResponsiveElementActions.hasDefault = true;
                    _logResponsiveElementActions.defaultValue = false;
                }
            }
            
            if (_logConditionResponses == null) {
                var variable = Utils.GetBoolVariable(nameof(logConditionResponses).Capitalize());
                if (variable != null) {
                    _logConditionResponses = variable;
                }
                else {
                    _logConditionResponses = CreateDebugSetting(typeof(BoolVariable), nameof(logConditionResponses).Capitalize()) as BoolVariable;
                    _logConditionResponses.SetValue(false);
                    _logConditionResponses.hasDefault = true;
                    _logConditionResponses.defaultValue = false;
                }
            }
#endif
        }
        
        private static UnityEngine.Object CreateDebugSetting(Type assetType, string name)
        {
            return Utils.CreateScriptableObjectAsset(assetType, name, new string[] {"Scenes", "Shared", "AppSettings", "DebugSettings"});
        }
    }
}