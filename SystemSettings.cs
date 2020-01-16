using System;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AltSalt.Maestro
{
    [CreateAssetMenu(menuName = "AltSalt/Production Settings")]
    public class SystemSettings : ScriptableObject
    {
        [SerializeField, Required]
        private BoolReference _hasBeenOpened;

        public BoolReference hasBeenOpened
        {
            get
            {
                UpdateDependencies();
                return _hasBeenOpened;
            }
        }

        [SerializeField, Required]
        public FloatReference _deviceAspectRatio;
        
        public FloatReference deviceAspectRatio
        {
            get
            {
                UpdateDependencies();
                return _deviceAspectRatio;
            }
        }
        
        [SerializeField, Required]
        public FloatReference _deviceHeight;
        
        public FloatReference deviceHeight
        {
            get
            {
                UpdateDependencies();
                return _deviceHeight;
            }
        }
        
        [SerializeField, Required]
        public FloatReference _deviceWidth;
        
        public FloatReference deviceWidth
        {
            get
            {
                UpdateDependencies();
                return _deviceWidth;
            }
        }
        
        [SerializeField, Required]
        public FloatReference _currentSceneAspectRatio;
        
        public FloatReference currentSceneAspectRatio
        {
            get
            {
                UpdateDependencies();
                return _currentSceneAspectRatio;
            }
        }
        
        [SerializeField, Required]
        public FloatReference _currentSceneHeight;
        
        public FloatReference currentSceneHeight
        {
            get
            {
                UpdateDependencies();
                return _currentSceneHeight;
            }
        }
        
        [SerializeField, Required]
        public FloatReference _currentSceneWidth;
        
        public FloatReference currentSceneWidth
        {
            get
            {
                UpdateDependencies();
                return _currentSceneWidth;
            }
        }

        [SerializeField, Required]
        private BoolReference _volumeEnabled;

        public BoolReference volumeEnabled
        {
            get
            {
                UpdateDependencies();
                return _volumeEnabled;
            }
        }
        
        [SerializeField, Required]
        private BoolReference _musicEnabled;

        public BoolReference musicEnabled
        {
            get
            {
                UpdateDependencies();
                return _musicEnabled;
            }
        }
        
        [SerializeField, Required]
        private BoolReference _soundEffectsEnabled;

        public BoolReference soundEffectsEnabled
        {
            get
            {
                UpdateDependencies();
                return _soundEffectsEnabled;
            }
        }

        [SerializeField, Required]
        private BoolReference _paused;

        public BoolReference paused {
            get
            {
                UpdateDependencies();
                return _paused;
            }
        }
        
        
        [SerializeField, Required]
        public FloatReference _timescale;
        
        public FloatReference timescale
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
            FieldInfo[] fieldInfo = typeof(SystemSettings).GetFields();
            for (int i = 0; i < fieldInfo.Length; i++) {
                var gameplaySetting = fieldInfo[i].GetValue(this);
                MethodInfo methodInfo = gameplaySetting.GetType().GetMethod(nameof(ModifiableEditorVariable.SetToDefaultValue));
                methodInfo.Invoke(gameplaySetting, BindingFlags.Public, null, null, null);
            }

//            hasBeenOpened.SetToDefaultValue();
//            globalAutoplayEnabled.SetToDefaultValue();
//            globalMomentumEnabled.SetToDefaultValue();
//            volumeEnabled.SetToDefaultValue();
//            musicEnabled.SetToDefaultValue();
//            soundEffectsEnabled.SetToDefaultValue();
//            paused.SetToDefaultValue();
//            timescale.SetToDefaultValue();
//            frameStepValue.SetToDefaultValue();
        }

        private void OnEnable()
        {
            UpdateDependencies();
        }

        [InfoBox("Finds and / or creates production settings.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void UpdateDependencies()
        {
            
            
#if UNITY_EDITOR
            FieldInfo[] fields = typeof(SystemSettings).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            for (int i = 0; i < fields.Length; i++) {
                
                var varReference = fields[i].GetValue(this);
                string name = fields[i].Name.Replace("_", "").Capitalize();
                var variableField = varReference.GetType().GetField("_variable", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

                var variable = variableField.GetValue(varReference) as ScriptableObject;

                if (variable == null) {
                    variableField.SetValue(varReference, CreateProductionSetting(variableField.FieldType, $"{name}"));
                }
            }
            
//            if (_hasBeenOpened == null) {
//                var variable = Utils.GetBoolVariable(nameof(hasBeenOpened).Capitalize());
//                if (variable != null) {
//                    _hasBeenOpened = variable;
//                }
//                else {
//                    _hasBeenOpened = CreateProductionSetting(typeof(BoolVariable), nameof(hasBeenOpened).Capitalize()) as BoolVariable;
//                    _hasBeenOpened.SetValue(false);
//                    _hasBeenOpened.hasDefault = true;
//                    _hasBeenOpened.defaultValue = false;
//                }
//            }
//            
//            if (_globaleAutoplayEnabled == null) {
//                var variable = Utils.GetBoolVariable(nameof(globalAutoplayEnabled).Capitalize());
//                if (variable != null) {
//                    _globaleAutoplayEnabled = variable;
//                }
//                else {
//                    _globaleAutoplayEnabled = CreateProductionSetting(typeof(BoolVariable), nameof(globalAutoplayEnabled).Capitalize()) as BoolVariable;
//                    _globaleAutoplayEnabled.SetValue(true);
//                    _globaleAutoplayEnabled.hasDefault = true;
//                    _globaleAutoplayEnabled.defaultValue = true;
//                }
//            }
//            
//            if (_globalMomentumEnabled == null) {
//                var variable = Utils.GetBoolVariable(nameof(globalMomentumEnabled).Capitalize());
//                if (variable != null) {
//                    _globalMomentumEnabled = variable;
//                }
//                else {
//                    _globalMomentumEnabled = CreateProductionSetting(typeof(BoolVariable), nameof(globalMomentumEnabled).Capitalize()) as BoolVariable;
//                    _globalMomentumEnabled.SetValue(true);
//                    _globalMomentumEnabled.hasDefault = true;
//                    _globalMomentumEnabled.defaultValue = true;
//                }
//            }
//            
//            if (_musicEnabled == null) {
//                var variable = Utils.GetBoolVariable(nameof(musicEnabled).Capitalize());
//                if (variable != null) {
//                    _musicEnabled = variable;
//                }
//                else {
//                    _musicEnabled = CreateProductionSetting(typeof(BoolVariable), nameof(musicEnabled).Capitalize()) as BoolVariable;
//                    _musicEnabled.SetValue(true);
//                    _musicEnabled.hasDefault = true;
//                    _musicEnabled.defaultValue = true;
//                }
//            }
//            
//            if (_soundEffectsEnabled == null) {
//                var variable = Utils.GetBoolVariable(nameof(soundEffectsEnabled).Capitalize());
//                if (variable != null) {
//                    _soundEffectsEnabled = variable;
//                }
//                else {
//                    _soundEffectsEnabled = CreateProductionSetting(typeof(BoolVariable), nameof(soundEffectsEnabled).Capitalize()) as BoolVariable;
//                    _soundEffectsEnabled.SetValue(true);
//                    _soundEffectsEnabled.hasDefault = true;
//                    _soundEffectsEnabled.defaultValue = true;
//                }
//            }
//            
//            if (_paused == null) {
//                var variable = Utils.GetBoolVariable(nameof(paused).Capitalize());
//                if (variable != null) {
//                    _paused = variable;
//                }
//                else {
//                    _paused = CreateProductionSetting(typeof(BoolVariable), nameof(paused).Capitalize()) as BoolVariable;
//                    _paused.SetValue(false);
//                    _paused.hasDefault = true;
//                    _paused.defaultValue = false;
//                }
//            }
//            
//            if (_deviceAspectRatio == null) {
//                var variable = Utils.GetFloatVariable(nameof(deviceAspectRatio).Capitalize());
//                if (variable != null) {
//                    _deviceAspectRatio = variable;
//                }
//                else {
//                    _deviceAspectRatio = CreateProductionSetting(typeof(FloatVariable), nameof(deviceAspectRatio).Capitalize()) as FloatVariable;
//                }
//            }
//            
//            if (_deviceHeight == null) {
//                var variable = Utils.GetFloatVariable(nameof(deviceHeight).Capitalize());
//                if (variable != null) {
//                    _deviceHeight = variable;
//                }
//                else {
//                    _deviceHeight = CreateProductionSetting(typeof(FloatVariable), nameof(deviceHeight).Capitalize()) as FloatVariable;
//                }
//            }
//            
//            if (_deviceWidth == null) {
//                var variable = Utils.GetFloatVariable(nameof(deviceWidth).Capitalize());
//                if (variable != null) {
//                    _deviceWidth = variable;
//                }
//                else {
//                    _deviceWidth = CreateProductionSetting(typeof(FloatVariable), nameof(deviceWidth).Capitalize()) as FloatVariable;
//                }
//            }
//            
//            if (_sceneAspectRatio == null) {
//                var variable = Utils.GetFloatVariable(nameof(sceneAspectRatio).Capitalize());
//                if (variable != null) {
//                    _sceneAspectRatio = variable;
//                }
//                else {
//                    _sceneAspectRatio = CreateProductionSetting(typeof(FloatVariable), nameof(sceneAspectRatio).Capitalize()) as FloatVariable;
//                }
//            }
//            
//            if (_sceneHeight == null) {
//                var variable = Utils.GetFloatVariable(nameof(sceneHeight).Capitalize());
//                if (variable != null) {
//                    _sceneHeight = variable;
//                }
//                else {
//                    _sceneHeight = CreateProductionSetting(typeof(FloatVariable), nameof(sceneHeight).Capitalize()) as FloatVariable;
//                }
//            }
//            
//            if (_sceneWidth == null) {
//                var variable = Utils.GetFloatVariable(nameof(sceneWidth).Capitalize());
//                if (variable != null) {
//                    _sceneWidth = variable;
//                }
//                else {
//                    _sceneWidth = CreateProductionSetting(typeof(FloatVariable), nameof(sceneWidth).Capitalize()) as FloatVariable;
//                }
//            }
//            
//            if (_timescale == null) {
//                var variable = Utils.GetFloatVariable(nameof(timescale).Capitalize());
//                if (variable != null) {
//                    _timescale = variable;
//                }
//                else {
//                    _timescale = CreateProductionSetting(typeof(FloatVariable), nameof(timescale).Capitalize()) as FloatVariable;
//                    _timescale.SetValue(1f);
//                    _timescale.hasDefault = true;
//                    _timescale.defaultValue = 1f;
//                }
//            }
//            
//            if (_frameStepValue == null) {
//                var variable = Utils.GetFloatVariable(nameof(frameStepValue).Capitalize());
//                if (variable != null) {
//                    _frameStepValue = variable;
//                }
//                else {
//                    _frameStepValue = CreateProductionSetting(typeof(FloatVariable), nameof(frameStepValue).Capitalize()) as FloatVariable;
//                    _frameStepValue.SetValue(.02f);
//                    _frameStepValue.hasDefault = true;
//                    _frameStepValue.defaultValue = .02f;
//                }
//            }
#endif
        }

        private static ScriptableObject CreateProductionSetting(Type assetType, string name)
        {
            return Utils.CreateScriptableObjectAsset(assetType, name, Utils.settingsPath + "/ProductionSettings");
        }
  
    }
}