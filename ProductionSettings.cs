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
        }
        
        [SerializeField, Required]
        private BoolVariable _globaleAutoplayEnabled;

        public BoolVariable globalAutoplayEnabled
        {
            get
            {
                UpdateDependencies();
                return _globaleAutoplayEnabled;
            }
        }
        
        [SerializeField, Required]
        private BoolVariable _globalMomentumEnabled;

        public BoolVariable globalMomentumEnabled
        {
            get
            {
                UpdateDependencies();
                return _globalMomentumEnabled;
            }
        }
        
        [SerializeField, Required]
        private BoolVariable _volumeEnabled;

        public BoolVariable volumeEnabled
        {
            get
            {
                UpdateDependencies();
                return _volumeEnabled;
            }
        }
        
        [SerializeField, Required]
        private BoolVariable _musicEnabled;

        public BoolVariable musicEnabled
        {
            get
            {
                UpdateDependencies();
                return _musicEnabled;
            }
        }
        
        [SerializeField, Required]
        private BoolVariable _soundEffectsEnabled;

        public BoolVariable soundEffectsEnabled
        {
            get
            {
                UpdateDependencies();
                return _soundEffectsEnabled;
            }
        }

        [SerializeField, Required]
        private BoolVariable _paused;

        public BoolVariable paused {
            get
            {
                UpdateDependencies();
                return _paused;
            }
        }
        
        [SerializeField, Required]
        public FloatVariable _deviceAspectRatio;
        
        public FloatVariable deviceAspectRatio
        {
            get
            {
                UpdateDependencies();
                return _deviceAspectRatio;
            }
        }
        
        [SerializeField, Required]
        public FloatVariable _deviceHeight;
        
        public FloatVariable deviceHeight
        {
            get
            {
                UpdateDependencies();
                return _deviceHeight;
            }
        }
        
        [SerializeField, Required]
        public FloatVariable _deviceWidth;
        
        public FloatVariable deviceWidth
        {
            get
            {
                UpdateDependencies();
                return _deviceWidth;
            }
        }
        
        [SerializeField, Required]
        public FloatVariable _sceneAspectRatio;
        
        public FloatVariable sceneAspectRatio
        {
            get
            {
                UpdateDependencies();
                return _sceneAspectRatio;
            }
        }
        
        [SerializeField, Required]
        public FloatVariable _sceneHeight;
        
        public FloatVariable sceneHeight
        {
            get
            {
                UpdateDependencies();
                return _sceneHeight;
            }
        }
        
        [SerializeField, Required]
        public FloatVariable _sceneWidth;
        
        public FloatVariable sceneWidth
        {
            get
            {
                UpdateDependencies();
                return _sceneWidth;
            }
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
        }

        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void SetDefaults()
        {
            hasBeenOpened.SetValue(false);
            globalAutoplayEnabled.SetValue(true);
            globalMomentumEnabled.SetValue(true);
            volumeEnabled.SetValue(true);
            musicEnabled.SetValue(true);
            soundEffectsEnabled.SetValue(true);
            paused.SetValue(false);
            timescale.SetValue(1f);
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
            
            if (_globaleAutoplayEnabled == null) {
                var variable = Utils.GetBoolVariable(nameof(globalAutoplayEnabled).Capitalize());
                if (variable != null) {
                    _globaleAutoplayEnabled = variable;
                }
                else {
                    _globaleAutoplayEnabled = CreateProductionSetting(typeof(BoolVariable), nameof(globalAutoplayEnabled).Capitalize()) as BoolVariable;
                    _globaleAutoplayEnabled.SetValue(true);
                    _globaleAutoplayEnabled.hasDefault = true;
                    _globaleAutoplayEnabled.defaultValue = true;
                }
            }
            
            if (_globalMomentumEnabled == null) {
                var variable = Utils.GetBoolVariable(nameof(globalMomentumEnabled).Capitalize());
                if (variable != null) {
                    _globalMomentumEnabled = variable;
                }
                else {
                    _globalMomentumEnabled = CreateProductionSetting(typeof(BoolVariable), nameof(globalMomentumEnabled).Capitalize()) as BoolVariable;
                    _globalMomentumEnabled.SetValue(true);
                    _globalMomentumEnabled.hasDefault = true;
                    _globalMomentumEnabled.defaultValue = true;
                }
            }
            
            if (_musicEnabled == null) {
                var variable = Utils.GetBoolVariable(nameof(musicEnabled).Capitalize());
                if (variable != null) {
                    _musicEnabled = variable;
                }
                else {
                    _musicEnabled = CreateProductionSetting(typeof(BoolVariable), nameof(musicEnabled).Capitalize()) as BoolVariable;
                    _musicEnabled.SetValue(true);
                    _musicEnabled.hasDefault = true;
                    _musicEnabled.defaultValue = true;
                }
            }
            
            if (_soundEffectsEnabled == null) {
                var variable = Utils.GetBoolVariable(nameof(soundEffectsEnabled).Capitalize());
                if (variable != null) {
                    _soundEffectsEnabled = variable;
                }
                else {
                    _soundEffectsEnabled = CreateProductionSetting(typeof(BoolVariable), nameof(soundEffectsEnabled).Capitalize()) as BoolVariable;
                    _soundEffectsEnabled.SetValue(true);
                    _soundEffectsEnabled.hasDefault = true;
                    _soundEffectsEnabled.defaultValue = true;
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
            
            if (_deviceAspectRatio == null) {
                var variable = Utils.GetFloatVariable(nameof(deviceAspectRatio).Capitalize());
                if (variable != null) {
                    _deviceAspectRatio = variable;
                }
                else {
                    _deviceAspectRatio = CreateProductionSetting(typeof(FloatVariable), nameof(deviceAspectRatio).Capitalize()) as FloatVariable;
                }
            }
            
            if (_deviceHeight == null) {
                var variable = Utils.GetFloatVariable(nameof(deviceHeight).Capitalize());
                if (variable != null) {
                    _deviceHeight = variable;
                }
                else {
                    _deviceHeight = CreateProductionSetting(typeof(FloatVariable), nameof(deviceHeight).Capitalize()) as FloatVariable;
                }
            }
            
            if (_deviceWidth == null) {
                var variable = Utils.GetFloatVariable(nameof(deviceWidth).Capitalize());
                if (variable != null) {
                    _deviceWidth = variable;
                }
                else {
                    _deviceWidth = CreateProductionSetting(typeof(FloatVariable), nameof(deviceWidth).Capitalize()) as FloatVariable;
                }
            }
            
            if (_sceneAspectRatio == null) {
                var variable = Utils.GetFloatVariable(nameof(sceneAspectRatio).Capitalize());
                if (variable != null) {
                    _sceneAspectRatio = variable;
                }
                else {
                    _sceneAspectRatio = CreateProductionSetting(typeof(FloatVariable), nameof(sceneAspectRatio).Capitalize()) as FloatVariable;
                }
            }
            
            if (_sceneHeight == null) {
                var variable = Utils.GetFloatVariable(nameof(sceneHeight).Capitalize());
                if (variable != null) {
                    _sceneHeight = variable;
                }
                else {
                    _sceneHeight = CreateProductionSetting(typeof(FloatVariable), nameof(sceneHeight).Capitalize()) as FloatVariable;
                }
            }
            
            if (_sceneWidth == null) {
                var variable = Utils.GetFloatVariable(nameof(sceneWidth).Capitalize());
                if (variable != null) {
                    _sceneWidth = variable;
                }
                else {
                    _sceneWidth = CreateProductionSetting(typeof(FloatVariable), nameof(sceneWidth).Capitalize()) as FloatVariable;
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
            return Utils.CreateScriptableObjectAsset(assetType, name, Utils.settingsPath + "/ProductionSettings");
        }
  
    }
}