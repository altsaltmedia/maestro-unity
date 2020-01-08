using System;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;

namespace AltSalt.Maestro
{
    [CreateAssetMenu(menuName = "AltSalt/App Settings")]
    public class AppSettings : ScriptableObject
    {
        [SerializeField, Required]
        private DebugSettings _debugSettings;

        private DebugSettings debugSettings
        {
            get 
            {
                UpdateDependencies();
                return _debugSettings;
            }
            set => _debugSettings = value;
        }
        
        [SerializeField, Required]
        private ProductionSettings _productionSettings;

        private ProductionSettings productionSettings
        {
            get 
            {
                UpdateDependencies();
                return _productionSettings;
            }
            set => _productionSettings = value;
        }

        // Production settings
        // In production, these values will vary depending on app state
        
        public bool hasBeenOpened => productionSettings.hasBeenOpened;
        
        public bool autoplayActive => productionSettings.autoplayActive;
        
        public bool momentumActive => productionSettings.momentumActive;
        
        public bool musicActive => productionSettings.musicActive;
        
        public bool soundEffectsActive => productionSettings.soundEffectsActive;
        
        public float timescale => productionSettings.timescale;
        
        
        // Debug settings
        
        public bool responsiveLayoutActive => debugSettings.responsiveLayoutActive;
        
        public bool saveDataActive => debugSettings.saveDataActive;
        
        public bool modifyTextActive => debugSettings.modifyTextActive;
        
        public bool modifyLayoutActive => debugSettings.modifyLayoutActive;
        
        public bool useAddressables => debugSettings.useAddressables;

        public bool logEventCallersAndListeners => debugSettings.logEventCallersAndListeners;
        
        public bool logResponsiveElementActions => debugSettings.logResponsiveElementActions;
        
        public bool logConditionResponses => debugSettings.logConditionResponses;

        [InfoBox("Finds and / or creates settings.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void UpdateDependencies()
        {
#if UNITY_EDITOR
            if (_debugSettings == null) {
                var variable = Utils.GetScriptableObject(nameof(DebugSettings)) as DebugSettings;
                if (variable != null) {
                    _debugSettings = variable;
                }
                else {
                    _debugSettings = CreateAppSetting(typeof(DebugSettings), nameof(DebugSettings)) as DebugSettings;
                }
            }
            
            if (_productionSettings == null) {
                var variable = Utils.GetScriptableObject(nameof(ProductionSettings)) as ProductionSettings;
                if (variable != null) {
                    _productionSettings = variable;
                }
                else {
                    _productionSettings = CreateAppSetting(typeof(ProductionSettings), nameof(ProductionSettings)) as ProductionSettings;;
                }
            }
#endif
        }

        private static ScriptableObject CreateAppSetting(Type assetType, string name)
        {
            return Utils.CreateScriptableObjectAsset(assetType, name, Utils.settingsPath);
        }

    }
}