using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AltSalt.Maestro
{
    [CreateAssetMenu(menuName = "AltSalt/Production Settings")]
    public class ProductionSettings : ScriptableObject
    {
        [SerializeField, Required]
        private BoolVariable _hasBeenOpened;

        public BoolVariable hasBeenOpened
        {
            get
            {
                UpdateDependencies();
                return _hasBeenOpened;
            }
            private set => hasBeenOpened = value;
        }
        
        [SerializeField, Required]
        private BoolVariable _autoplayActive;

        public BoolVariable autoplayActive
        {
            get
            {
                UpdateDependencies();
                return _autoplayActive;
            }
            private set => autoplayActive = value;
        }
        
        [SerializeField, Required]
        private BoolVariable _momentumActive;

        public BoolVariable momentumActive
        {
            get
            {
                UpdateDependencies();
                return _momentumActive;
            }
            private set => momentumActive = value;
        }
        
        [SerializeField, Required]
        private BoolVariable _musicActive;

        public BoolVariable musicActive
        {
            get
            {
                UpdateDependencies();
                return _musicActive;
            }
            private set => musicActive = value;
        }
        
        [SerializeField, Required]
        private BoolVariable _soundEffectsActive;

        public BoolVariable soundEffectsActive
        {
            get
            {
                UpdateDependencies();
                return _soundEffectsActive;
            }
            private set => soundEffectsActive = value;
        }

        [SerializeField, Required]
        private BoolVariable _paused;

        public BoolVariable paused {
            get
            {
                UpdateDependencies();
                return _paused;
            }
            private set => _paused = value;
        }
        
        [SerializeField, Required]
        public FloatVariable _timescale;
        
        public FloatVariable timescale
        {
            get
            {
                UpdateDependencies();
                return _timescale;
            }
            private set => _timescale = value;
        }

        [InfoBox("Finds and / or creates production settings.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void UpdateDependencies()
        {
#if UNITY_EDITOR
            if (_hasBeenOpened == null) {
                var variable = Utils.GetBoolVariable(nameof(hasBeenOpened).Capitalize());
                if (variable != null) {
                    _hasBeenOpened = variable;
                }
                else {
                    _hasBeenOpened = CreateProductionSetting(typeof(BoolVariable), nameof(hasBeenOpened).Capitalize()) as BoolVariable;
                    _hasBeenOpened.SetValue(false);
                    _hasBeenOpened.hasDefault = true;
                    _hasBeenOpened.defaultValue = false;
                }
            }
            
            if (_autoplayActive == null) {
                var variable = Utils.GetBoolVariable(nameof(autoplayActive).Capitalize());
                if (variable != null) {
                    _autoplayActive = variable;
                }
                else {
                    _autoplayActive = CreateProductionSetting(typeof(BoolVariable), nameof(autoplayActive).Capitalize()) as BoolVariable;
                    _autoplayActive.SetValue(true);
                    _autoplayActive.hasDefault = true;
                    _autoplayActive.defaultValue = true;
                }
            }
            
            if (_momentumActive == null) {
                var variable = Utils.GetBoolVariable(nameof(momentumActive).Capitalize());
                if (variable != null) {
                    _momentumActive = variable;
                }
                else {
                    _momentumActive = CreateProductionSetting(typeof(BoolVariable), nameof(momentumActive).Capitalize()) as BoolVariable;
                    _momentumActive.SetValue(true);
                    _momentumActive.hasDefault = true;
                    _momentumActive.defaultValue = true;
                }
            }
            
            if (_musicActive == null) {
                var variable = Utils.GetBoolVariable(nameof(musicActive).Capitalize());
                if (variable != null) {
                    _musicActive = variable;
                }
                else {
                    _musicActive = CreateProductionSetting(typeof(BoolVariable), nameof(musicActive).Capitalize()) as BoolVariable;
                    _musicActive.SetValue(true);
                    _musicActive.hasDefault = true;
                    _musicActive.defaultValue = true;
                }
            }
            
            if (_soundEffectsActive == null) {
                var variable = Utils.GetBoolVariable(nameof(soundEffectsActive).Capitalize());
                if (variable != null) {
                    _soundEffectsActive = variable;
                }
                else {
                    _soundEffectsActive = CreateProductionSetting(typeof(BoolVariable), nameof(soundEffectsActive).Capitalize()) as BoolVariable;
                    _soundEffectsActive.SetValue(true);
                    _soundEffectsActive.hasDefault = true;
                    _soundEffectsActive.defaultValue = true;
                }
            }
            
            if (_paused == null) {
                var variable = Utils.GetBoolVariable(nameof(paused).Capitalize());
                if (variable != null) {
                    _paused = variable;
                }
                else {
                    _paused = CreateProductionSetting(typeof(BoolVariable), nameof(paused).Capitalize()) as BoolVariable;
                    _paused.SetValue(false);
                    _paused.hasDefault = true;
                    _paused.defaultValue = false;
                }
            }
            
            if (_timescale == null) {
                var variable = Utils.GetFloatVariable(nameof(timescale).Capitalize());
                if (variable != null) {
                    _timescale = variable;
                }
                else {
                    _timescale = CreateProductionSetting(typeof(FloatVariable), nameof(timescale).Capitalize()) as FloatVariable;
                    _timescale.SetValue(1f);
                    _timescale.hasDefault = true;
                    _timescale.defaultValue = 1f;
                }
            }
#endif
        }

        private static ScriptableObject CreateProductionSetting(Type assetType, string name)
        {
            return Utils.CreateScriptableObjectAsset(assetType, name, new string[] {"Scenes", "Shared", "AppSettings", "ProductionSettings"});
        }
  
    }
}