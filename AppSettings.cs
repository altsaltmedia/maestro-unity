using System;
using System.Reflection;
using UnityEngine;
using Sirenix.OdinInspector;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro
{
    [CreateAssetMenu(menuName = "AltSalt/App Settings")]
    public class AppSettings : ScriptableObject
    {
       
        [SerializeField, Required]
        private SystemSettings _systemSettings;

        private SystemSettings systemSettings
        {
            get 
            {
                UpdateDependencies();
                return _systemSettings;
            }
        }
        
        [SerializeField, Required]
        private UserData _userData;

        private UserData userData
        {
            get 
            {
                UpdateDependencies();
                return _userData;
            }
        }
        
        [SerializeField, Required]
        private InputData _inputData;

        private InputData inputData
        {
            get 
            {
                UpdateDependencies();
                return _inputData;
            }
        }
        
        [SerializeField, Required]
        private DebugPreferences _debugPreferences;

        private DebugPreferences debugPreferences
        {
            get 
            {
                UpdateDependencies();
                return _debugPreferences;
            }
        }

        
        
        // System settings

        public bool GetHasBeenOpened(Object callingObject)
        {
            return systemSettings.hasBeenOpened.GetValue(callingObject);
        }
        
        public float GetDeviceAspectRatio(Object callingObject)
        {
            return systemSettings.deviceAspectRatio.GetValue(callingObject);
        }
        
        public FloatVariable SetDeviceAspectRatio(GameObject callingObject, float value)
        {
            return systemSettings.deviceAspectRatio.SetValue(callingObject, value);
        }
        
        public float GetDeviceWidth(Object callingObject)
        {
            return systemSettings.deviceWidth.GetValue(callingObject);
        }
        
        public FloatVariable SetDeviceWidth(GameObject callingObject, float value)
        {
            return systemSettings.deviceWidth.SetValue(callingObject, value);
        }
        
        public float GetDeviceHeight(Object callingObject)
        {
            return systemSettings.deviceHeight.GetValue(callingObject);
        }

        public FloatVariable SetDeviceHeight(GameObject callingObject, float value)
        {
            return systemSettings.deviceHeight.SetValue(callingObject, value);
        }
        
        public float GetCurrentSceneAspectRatio(Object callingObject)
        {
            return systemSettings.currentSceneAspectRatio.GetValue(callingObject);
        }
        
        public FloatVariable SetCurrentSceneAspectRatio(GameObject callingObject, float value)
        {
            return systemSettings.currentSceneAspectRatio.SetValue(callingObject, value);
        }
        
        public float GetCurrentSceneWidth(Object callingObject)
        {
            return systemSettings.currentSceneWidth.GetValue(callingObject);
        }
        
        public FloatVariable SetCurrentSceneWidth(GameObject callingObject, float value)
        {
            return systemSettings.currentSceneWidth.SetValue(callingObject, value);
        }
        
        public float GetCurrentSceneHeight(Object callingObject)
        {
            return systemSettings.currentSceneHeight.GetValue(callingObject);
        }

        public FloatVariable SetCurrentSceneHeight(GameObject callingObject, float value)
        {
            return systemSettings.currentSceneHeight.SetValue(callingObject, value);
        }

        public bool GetVolumeEnabled(Object callingObject)
        {
            return systemSettings.volumeEnabled.GetValue(callingObject);
        }

        public bool GetMusicEnabled(Object callingObject)
        {
            return systemSettings.musicEnabled.GetValue(callingObject);
        }

        public bool GetSoundEffectsEnabled(Object callingObject)
        {
            return systemSettings.soundEffectsEnabled.GetValue(callingObject);
        }
        
        public bool GetIsPaused(Object callingObject)
        {
            return systemSettings.paused.GetValue(callingObject);
        }
        
        public float GetTimescale(Object callingObject)
        {
            return systemSettings.timescale.GetValue(callingObject);
        }
        
        
        // Profile Settings

        public float GetYSensitivity(Object callingObject, CustomKey userKey)
        {
            return userData.GetUserPreferences(userKey).ySensitivity.GetValue(callingObject);
        }
        
        public bool GetInvertYInput(Object callingObject, CustomKey userKey)
        {
            return userData.GetUserPreferences(userKey).invertYInput.GetValue(callingObject);
        }

        public float GetXSensitivity(Object callingObject, CustomKey userKey)
        {
            return userData.GetUserPreferences(userKey).xSensitivity.GetValue(callingObject);
        }
        
        public bool GetInvertXInput(Object callingObject, CustomKey userKey)
        {
            return userData.GetUserPreferences(userKey).invertXInput.GetValue(callingObject);
        }
        
        public bool GetUserAutoplayEnabled(Object callingObject, CustomKey userKey)
        {
            return userData.GetUserPreferences(userKey).userAutoplayEnabled.GetValue(callingObject);
        }

        public bool GetUserMomentumEnabled(Object callingObject, CustomKey userKey)
        {
            return userData.GetUserPreferences(userKey).userMomentumEnabled.GetValue(callingObject);
        }


        // Input Data

        public V2Variable SetSwipeForce(GameObject callingObject, CustomKey inputGroupKey, Vector2 targetValue)
        {
            return inputData.GetInputGroup(inputGroupKey).swipeForce.SetValue(callingObject, targetValue);
        }
        
        public SimpleEventTrigger GetOnTouchStart(GameObject callingObject, CustomKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).onTouchStart;
        }
        
        public SimpleEventTrigger GetOnLongTouch(GameObject callingObject, CustomKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).onLongTouch;
        }
        
        public SimpleEventTrigger GetOnSwipe(GameObject callingObject, CustomKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).onSwipe;
        }
        
        public SimpleEventTrigger GetOnSwipeEnd(GameObject callingObject, CustomKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).onSwipeEnd;
        }


        // ========== //
        // SEQUENCING
        // ========== //
        
        public AxisReference GetYSwipeAxis(Object callingObject, CustomKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).ySwipeAxis;
        }

        public AxisReference GetYMomentumAxis(Object callingObject, CustomKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).yMomentumAxis;
        }

        public AxisReference GetXSwipeAxis(Object callingObject, CustomKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).xSwipeAxis;
        }

        public AxisReference GetXMomentumAxis(Object callingObject, CustomKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).xMomentumAxis;
        }
        
        public BoolReference GetIsReversing(Object callingObject, CustomKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).isReversing;
        }
        
        public FloatReference GetSwipeModifierOutput(Object callingObject, CustomKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).swipeModifierOutput;
        }
        
        public FloatReference GetMomentumModifierOutput(Object callingObject, CustomKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).swipeModifierOutput;
        }
        
        public StringReference GetSwipeDirection(Object callingObject, CustomKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).swipeDirection;
        }


    #region Debug Settings

        public bool dynamicLayoutActive
        {
            get => debugPreferences.dynamicLayoutActive;
            set => debugPreferences.dynamicLayoutActive = value;
        }

        public bool saveDataActive
        {
            get => debugPreferences.saveDataActive;
            set => debugPreferences.saveDataActive = value;
        }

        public bool modifyTextActive
        {
            get => debugPreferences.modifyTextActive;
            set => debugPreferences.modifyTextActive = value;
        }

        public bool modifyLayoutActive
        {
            get => debugPreferences.modifyLayoutActive;
            set => debugPreferences.modifyLayoutActive = value;
        }

        public bool useAddressables
        {
            get => debugPreferences.useAddressables;
            set => debugPreferences.useAddressables = value;
        }

        public bool logEventCallersAndListeners
        {
            get => debugPreferences.logEventCallersAndListeners;
            set => debugPreferences.logEventCallersAndListeners = value;
        }

        public bool logResponsiveElementActions
        {
            get => debugPreferences.logResponsiveElementActions;
            set => debugPreferences.logResponsiveElementActions = value;
        }

        public bool logConditionResponses
        {
            get => debugPreferences.logConditionResponses;
            set => debugPreferences.logConditionResponses = value;
        }

    #endregion


        private void OnEnable()
        {
            UpdateDependencies();
        }

        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void SetDefaults()
        {
            debugPreferences.SetDefaults();
            systemSettings.SetDefaults();
        }
      
        [InfoBox("Finds and / or creates settings.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void UpdateDependencies()
        {
#if UNITY_EDITOR
            FieldInfo[] fields = typeof(AppSettings).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

            for (int i = 0; i < fields.Length; i++) {
                var variable = fields[i].GetValue(this) as ScriptableObject;
                if (variable == null) {
                    string variableName = fields[i].Name.Replace("_", "").Capitalize();
                    variable = Utils.GetScriptableObject(variableName);
                    if (variable == null) {
                        Type type = fields[i].FieldType;
                        fields[i].SetValue(this, CreateAppSetting(type, variableName));
                    }
                }
            }
            
//            if (_debugSettings == null) {
//                var variable = Utils.GetScriptableObject(nameof(DebugSettings));
//                if (variable != null) {
//                    _debugSettings = variable;
//                }
//                else {
//                    _debugSettings = CreateAppSetting(typeof(DebugSettings), nameof(DebugSettings));
//                }
//            }
//            
//            if (_gameplaySettings == null) {
//                var variable = Utils.GetScriptableObject(nameof(GameplaySettings)) as GameplaySettings;
//                if (variable != null) {
//                    _gameplaySettings = variable;
//                }
//                else {
//                    _gameplaySettings = CreateAppSetting(typeof(GameplaySettings), nameof(GameplaySettings)) as GameplaySettings;;
//                }
//            }
//
//            if (_inputSettings == null) {
//                var variable = Utils.GetScriptableObject(nameof(InputSettings)) as InputSettings;
//                if (variable != null) {
//                    _inputSettings = variable;
//                }
//                else {
//                    _inputSettings = CreateAppSetting(typeof(InputSettings), nameof(InputSettings)) as InputSettings;;
//                }
//            }

        }
        private static dynamic CreateAppSetting(Type assetType, string name)
        {
            return Utils.CreateScriptableObjectAsset(assetType, name, Utils.settingsPath);
        }
#endif

    }
}