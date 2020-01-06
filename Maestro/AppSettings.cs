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
        
        public bool hasBeenOpened => productionSettings.hasBeenOpened.value;
        
        public bool autoplayActive => productionSettings.autoplayActive.value;
        
        public bool momentumActive => productionSettings.momentumActive.value;
        
        public bool musicActive => productionSettings.musicActive.value;
        
        public bool soundEffectsActive => productionSettings.soundEffectsActive.value;
        
        public float timescale => productionSettings.timescale.value;
        
        
        // Debug settings
        
        public bool responsiveLayoutActive => debugSettings.responsiveLayoutActive.value;
        
        public bool saveDataActive => debugSettings.saveDataActive.value;
        
        public bool modifyTextActive => debugSettings.modifyTextActive.value;
        
        public bool modifyLayoutActive => debugSettings.modifyLayoutActive.value;
        
        public bool useAddressables => debugSettings.useAddressables.value;

        public bool logEventCallersAndListeners => debugSettings.logEventCallersAndListeners.value;
        
        public bool logResponsiveElementActions => debugSettings.logResponsiveElementActions.value;
        
        public bool logConditionResponses => debugSettings.logConditionResponses.value;

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
            return Utils.CreateScriptableObjectAsset(assetType, name, new string[] {"Scenes", "Shared", "AppSettings"});
        }

    }
}