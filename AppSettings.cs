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
        }

        // Production settings
        // In production, these values will vary depending on app state
        
        public bool hasBeenOpened => productionSettings.hasBeenOpened.value;

        public bool globalAutoplayEnabled => productionSettings.globalAutoplayEnabled.value;

        public bool globalMomentumEnabled => productionSettings.globalMomentumEnabled.value;

        public bool volumeEnabled => productionSettings.volumeEnabled.value;

        public bool musicEnabled => productionSettings.musicEnabled.value;

        public bool soundEffectsEnabled => productionSettings.soundEffectsEnabled.value;
        
        public float deviceAspectRatio
        {
            get => productionSettings.deviceAspectRatio.value;
            set => productionSettings.deviceAspectRatio.SetValue(value);
        }
        
        public float deviceWidth
        {
            get => productionSettings.deviceWidth.value;
            set => productionSettings.deviceWidth.SetValue(value);
        }
        
        public float deviceHeight
        {
            get => productionSettings.deviceHeight.value;
            set => productionSettings.deviceHeight.SetValue(value);
        }

        public float timescale => productionSettings.timescale.value;


        // Debug settings
        
        public bool dynamicLayoutActive
        {
            get => debugSettings.dynamicLayoutActive;
            set => debugSettings.dynamicLayoutActive = value;
        }

        public bool saveDataActive
        {
            get => debugSettings.saveDataActive;
            set => debugSettings.saveDataActive = value;
        }

        public bool modifyTextActive
        {
            get => debugSettings.modifyTextActive;
            set => debugSettings.modifyTextActive = value;
        }

        public bool modifyLayoutActive
        {
            get => debugSettings.modifyLayoutActive;
            set => debugSettings.modifyLayoutActive = value;
        }

        public bool useAddressables
        {
            get => debugSettings.useAddressables;
            set => debugSettings.useAddressables = value;
        }

        public bool logEventCallersAndListeners
        {
            get => debugSettings.logEventCallersAndListeners;
            set => debugSettings.logEventCallersAndListeners = value;
        }

        public bool logResponsiveElementActions
        {
            get => debugSettings.logResponsiveElementActions;
            set => debugSettings.logResponsiveElementActions = value;
        }

        public bool logConditionResponses
        {
            get => debugSettings.logConditionResponses;
            set => debugSettings.logConditionResponses = value;
        }

        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void SetDefaults()
        {
            debugSettings.SetDefaults();
            productionSettings.SetDefaults();
        }

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