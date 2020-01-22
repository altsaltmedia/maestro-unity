using System;
using System.Reflection;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
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
            get {
                
                if(_systemSettings == null) {
                   RefreshDependencies();
                }

                return _systemSettings;
            }
        }

        [SerializeField, Required]
        private UserData _userData;

        private UserData userData
        {
            get {
                
                if(_userData == null) {
                   RefreshDependencies();
                }

                return _userData;
            }
        }

        [SerializeField, Required]
        private InputData _inputData;

        private InputData inputData
        {
            get
            {
                if(_inputData == null) {
                   RefreshDependencies();
                }
                return _inputData;
            }
        }

        [SerializeField, Required]
        private DebugPreferences _debugPreferences;

        private DebugPreferences debugPreferences
        {
            get
            {
                if(_debugPreferences == null) {
                   RefreshDependencies();
                }
                return _debugPreferences;
            }
        }

        [SerializeField, Required]
        private InputGroupKeyReference _mainInput = new InputGroupKeyReference();

        public InputGroupKey mainInput {

            get
            {
                if(_mainInput == null) {
                    RefreshDependencies();             
                }
                return _mainInput.GetVariable(this);
            }
            
        }

        [SerializeField, Required]
        private UserDataKeyReference _user1 = new UserDataKeyReference();

        public UserDataKey user1 {

            get
            {
                if(_user1 == null) {
                    RefreshDependencies();
                }
                return _user1.GetVariable(this);                    
            }
            
        }


    #region System Settings

    
        // Has Been Opened
    
        public bool GetHasBeenOpened(Object callingObject)
        {
            return systemSettings.hasBeenOpened.GetValue(callingObject);
        }
        
        
        // Device Settings
        
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
        
        
        
        // Scene Settings
        
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
        
        
        // Audio

        public bool GetVolumeEnabled(Object callingObject)
        {
            return systemSettings.volumeEnabled.GetValue(callingObject);
        }
        
        public bool SetVolumeEnabled(GameObject callingObject, bool targetValue)
        {
            return systemSettings.volumeEnabled.SetValue(callingObject, targetValue);
        }

        public bool GetMusicEnabled(Object callingObject)
        {
            return systemSettings.musicEnabled.GetValue(callingObject);
        }

        public bool GetSoundEffectsEnabled(Object callingObject)
        {
            return systemSettings.soundEffectsEnabled.GetValue(callingObject);
        }
        
        
        // Paused
        
        public bool GetIsPaused(Object callingObject)
        {
            return systemSettings.paused.GetValue(callingObject);
        }
        
        
        // Timescale
        
        public float GetTimescale(Object callingObject)
        {
            return systemSettings.timescale.GetValue(callingObject);
        }

        
    #endregion
    

    #region User Data

    
        // Y Sensitivity
    
        public float GetYSensitivity(Object callingObject, UserDataKey userKey)
        {
            return userData.GetUserPreferences(userKey).ySensitivity.GetValue(callingObject);
        }
        
        
        // Invert Y Input
        
        public bool GetInvertYInput(Object callingObject, UserDataKey userKey)
        {
            return userData.GetUserPreferences(userKey).invertYInput.GetValue(callingObject);
        }
        
        
        // X Sensitivity

        public float GetXSensitivity(Object callingObject, UserDataKey userKey)
        {
            return userData.GetUserPreferences(userKey).xSensitivity.GetValue(callingObject);
        }
        
        
        // Invert X Input
        
        public bool GetInvertXInput(Object callingObject, UserDataKey userKey)
        {
            return userData.GetUserPreferences(userKey).invertXInput.GetValue(callingObject);
        }
        
        
        // Autoplay Enabled
        
        public bool GetUserAutoplayEnabled(Object callingObject, UserDataKey userKey)
        {
            return userData.GetUserPreferences(userKey).userAutoplayEnabled.GetValue(callingObject);
        }
        
        
        // Momentum Enabled

        public bool GetUserMomentumEnabled(Object callingObject, UserDataKey userKey)
        {
            return userData.GetUserPreferences(userKey).userMomentumEnabled.GetValue(callingObject);
        }

        
    #endregion


    #region Input Data

    
        // Is Swiping
        
        public bool GetIsSwiping(GameObject callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).isSwiping.GetValue(callingObject);
        }
        
        public BoolVariable SetIsSwiping(GameObject callingObject, InputGroupKey inputGroupKey, bool targetValue)
        {
            return inputData.GetInputGroup(inputGroupKey).isSwiping.SetValue(callingObject, targetValue);
        }
    
        
        // Swipe Force
    
        public Vector2 GetSwipeForce(GameObject callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).swipeForce.GetValue(callingObject);
        }
        
        public V2Variable SetSwipeForce(GameObject callingObject, InputGroupKey inputGroupKey, Vector2 targetValue)
        {
            return inputData.GetInputGroup(inputGroupKey).swipeForce.SetValue(callingObject, targetValue);
        }
        
        
        // Swipe Monitor Momentum
        
        public Vector2 GetSwipeMonitorMomentum(GameObject callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).swipeMonitorMomentum.GetValue(callingObject);
        }
        
        public V2Variable SetSwipeMonitorMomentum(GameObject callingObject, InputGroupKey inputGroupKey, Vector2 targetValue)
        {
            return inputData.GetInputGroup(inputGroupKey).swipeMonitorMomentum.SetValue(callingObject, targetValue);
        }

        
        // Swipe Monitor Momentum Cache
        
        public Vector2 GetSwipeMonitorMomentumCache(GameObject callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).swipeMonitorMomentumCache.GetValue(callingObject);
        }
        
        public V2Variable SetSwipeMonitorMomentumCache(GameObject callingObject, InputGroupKey inputGroupKey, Vector2 targetValue)
        {
            return inputData.GetInputGroup(inputGroupKey).swipeMonitorMomentumCache.SetValue(callingObject, targetValue);
        }
        
        
        // Axis Transition Active

        public bool GetAxisTransitionActive(GameObject callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).axisTransitionActive.GetValue(callingObject);
        }
        
        public BoolVariable SetAxisTransitionActive(GameObject callingObject, InputGroupKey inputGroupKey, bool targetValue)
        {
            return inputData.GetInputGroup(inputGroupKey).axisTransitionActive.SetValue(callingObject, targetValue);
        }
        
        
        // Fork Transition Active

        public bool GetForkTransitionActive(GameObject callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).forkTransitionActive.GetValue(callingObject);
        }
        
        public BoolVariable SetForkTransitionActive(GameObject callingObject, InputGroupKey inputGroupKey, bool targetValue)
        {
            return inputData.GetInputGroup(inputGroupKey).forkTransitionActive.SetValue(callingObject, targetValue);
        }
        
        
        // Scrubber Active

        public bool GetScrubberActive(GameObject callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).scrubberActive.GetValue(callingObject);
        }
        
        public BoolVariable SetScrubberActive(GameObject callingObject, InputGroupKey inputGroupKey, bool targetValue)
        {
            return inputData.GetInputGroup(inputGroupKey).scrubberActive.SetValue(callingObject, targetValue);
        }
        

        // Axis Transition Spread

        public float GetAxisTransitionSpread(GameObject callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).axisTransitionSpread.GetValue(callingObject);
        }
        
        
        // Fork Transition Spread

        public float GetForkTransitionSpread(GameObject callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).forkTransitionSpread.GetValue(callingObject);
        }


        // Swipe Min Max
        
        public float GetSwipeMinMax(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).swipeMinMax.GetValue(callingObject);
        }
        
        
        // Momentum Min Max
        
        public float GetMomentumMinMax(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).momentumMinMax.GetValue(callingObject);
        }
        
        
        // Momentum Decay
        
        public float GetMomentumDecay(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).momentumDecay.GetValue(callingObject);
        }
        
        
        // Momentum Sensitivity

        public float GetMomentumSensitivity(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).momentumSensitivity.GetValue(callingObject);
        }
        
        
        // Gesture Time Multiplier

        public float GetGestureTimeMultiplier(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).gestureTimeMultiplier.GetValue(callingObject);
        }

        
        // Cancel Momentum Time Threshold

        public float GetCancelMomentumTimeThreshold(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).cancelMomentumTimeThreshold.GetValue(callingObject);
        }
        
        
        // Pause Momentum Threshold

        public float GetPauseMomentumTimeThreshold(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).pauseMomentumThreshold.GetValue(callingObject);
        }
        
        
        // Cancel Momentum Time Threshold

        public float GetCancelMomentumMagnitudeThreshold(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).cancelMomentumMagnitudeThreshold.GetValue(callingObject);
        }
        
        
        // Flick Threshold
        
        public float GetFlickThreshold(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).flickThreshold.GetValue(callingObject);
        }
        
        
        // Is Flicked
        
        public BoolVariable SetIsFlicked(GameObject callingObject, InputGroupKey inputGroupKey, bool targetValue)
        {
            return inputData.GetInputGroup(inputGroupKey).isFlicked.SetValue(callingObject, targetValue);
        }
        
        
        // Touch Start Position
        
        public V2Variable SetTouchStartPosition(GameObject callingObject, InputGroupKey inputGroupKey, Vector2 targetValue)
        {
            return inputData.GetInputGroup(inputGroupKey).touchStartPosition.SetValue(callingObject, targetValue);
        }
        
        
        // Swipe Direction

        public string GetSwipeDirection(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).swipeDirection.GetValue(callingObject);
        }
        
        public StringVariable SetSwipeDirection(GameObject callingObject, InputGroupKey inputGroupKey, string targetValue)
        {
            return inputData.GetInputGroup(inputGroupKey).swipeDirection.SetValue(callingObject, targetValue);
        }
        
        
        // Input Events
        
        public SimpleEventTrigger GetOnTouchStart(GameObject callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).onTouchStart;
        }
        
        public SimpleEventTrigger GetOnLongTouch(GameObject callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).onLongTouch;
        }
        
        public SimpleEventTrigger GetOnSwipe(GameObject callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).onSwipe;
        }
        
        public SimpleEventTrigger GetOnSwipeEnd(GameObject callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).onSwipeEnd;
        }
        
        public SimpleEventTrigger GetMomentumUpdate(GameObject callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).momentumUpdate;
        }
        
        public SimpleEventTrigger GetMomentumAppliedToSequences(GameObject callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).momentumAppliedToSequences;
        }
        
        public SimpleEventTrigger GetMomentumDepleted(GameObject callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).momentumDepleted;
        }
        
        public SimpleEventTrigger GetBoundaryReached(GameObject callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).boundaryReached;
        }
        
        public ComplexEventManualTrigger GetSequenceModified(GameObject callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).sequenceModified;
        }


        // Y Axes

        public AxisReference GetYSwipeAxisReference(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).ySwipeAxis;
        }

        public AxisReference GetYMomentumAxisReference(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).yMomentumAxis;
        }
        
        
        // X Axes

        public AxisReference GetXSwipeAxisReference(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).xSwipeAxis;
        }

        public AxisReference GetXMomentumAxisReference(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).xMomentumAxis;
        }
        
        
        // Is Reversing
        
        public bool GetIsReversing(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).isReversing.GetValue(callingObject);
        }
        
        public BoolVariable SetIsReversing(GameObject callingObject, InputGroupKey inputGroupKey, bool targetValue)
        {
            return inputData.GetInputGroup(inputGroupKey).isReversing.SetValue(callingObject, targetValue);
        }
        
        
        // Swipe Modifier Output
        
        public float GetSwipeModifierOutput(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).swipeModifierOutput.GetValue(callingObject);
        }
        
        public FloatVariable SetSwipeModifierOutput(GameObject callingObject, InputGroupKey inputGroupKey, float targetValue)
        {
            return inputData.GetInputGroup(inputGroupKey).swipeModifierOutput.SetValue(callingObject, targetValue);
        }
        
        
        // Momentum Modifier Output
        
        public float GetMomentumModifierOutput(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).momentumModifierOutput.GetValue(callingObject);
        }
        
        public FloatVariable SetMomentumModifierOutput(GameObject callingObject, InputGroupKey inputGroupKey, float targetValue)
        {
            return inputData.GetInputGroup(inputGroupKey).momentumModifierOutput.SetValue(callingObject, targetValue);
        }

        
        // Frame Step Value
        
        public float GetFrameStepValue(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).frameStepValue.GetValue(callingObject);
        }
        
    #endregion


    #region Debug Settings

        public float timelineDebugTime
        {
            get => debugPreferences.timelineDebugTime;
            set => debugPreferences.timelineDebugTime = value;
        }

        public SimpleEventTrigger onEditorGraphStart => debugPreferences.onEditorGraphStart;

        public bool dynamicLayoutActive => debugPreferences.dynamicLayoutActive;

        public bool saveDataActive => debugPreferences.saveDataActive;

        public bool modifyTextActive => debugPreferences.modifyTextActive;

        public bool modifyLayoutActive => debugPreferences.modifyLayoutActive;

        public bool useAddressables => debugPreferences.useAddressables;

        public bool logEventCallersAndListeners => debugPreferences.logEventCallersAndListeners;

        public bool logGlobalResponsiveElementActions => debugPreferences.logResponsiveElementActions;

        public bool logConditionResponses => debugPreferences.logConditionResponses;

    #endregion
        
        
#if UNITY_EDITOR

        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void SetDefaults()
        {
            userData.GetUserPreferences(user1);
            inputData.GetInputGroup(mainInput);
            
            systemSettings.SetDefaults();
            userData.SetDefaults();
            inputData.SetDefaults();
            debugPreferences.SetDefaults();
        }
      
        [InfoBox("Finds and / or creates settings.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void RefreshDependencies()
        {
            if (_systemSettings == null) {
                string variableName = nameof(systemSettings).Capitalize();
                var assetSearch = Utils.GetScriptableObject(nameof(systemSettings).Capitalize()) as SystemSettings;
                if (assetSearch == null) {
                    var newVariable = CreateAppSetting(typeof(SystemSettings), variableName);
                    _systemSettings = newVariable;
                }
                else {
                    _systemSettings = assetSearch;
                }
            }
            
            if (_userData == null) {
                string variableName = nameof(userData).Capitalize();
                var assetSearch = Utils.GetScriptableObject(nameof(userData).Capitalize()) as UserData;
                if (assetSearch == null) {
                    var newVariable = CreateAppSetting(typeof(UserData), variableName);
                    _userData = newVariable;
                }
                else {
                    _userData = assetSearch;
                }
            }
            
            if (_inputData == null) {
                string variableName = nameof(inputData).Capitalize();
                var assetSearch = Utils.GetScriptableObject(nameof(inputData).Capitalize()) as InputData;
                if (assetSearch == null) {
                    var newVariable = CreateAppSetting(typeof(InputData), variableName);
                    _inputData = newVariable;
                }
                else {
                    _inputData = assetSearch;
                }
            }
            
            if (_debugPreferences == null) {
                string variableName = nameof(debugPreferences).Capitalize();
                var assetSearch = Utils.GetScriptableObject(nameof(debugPreferences).Capitalize()) as DebugPreferences;
                if (assetSearch == null) {
                    var newVariable = CreateAppSetting(typeof(DebugPreferences), variableName);
                    _debugPreferences = newVariable;
                }
                else {
                    _debugPreferences = assetSearch;
                }
            }
            
            if (_mainInput.GetVariable(this) == null) {
                string variableName = nameof(mainInput).Capitalize();
                var assetSearch = Utils.GetScriptableObject(nameof(mainInput).Capitalize()) as InputGroupKey;
                if (assetSearch == null) {
                    var newVariable = CreateAppSetting(typeof(InputGroupKey), variableName);
                    _mainInput.SetVariable(newVariable);
                }
                else {
                    _mainInput.SetVariable(assetSearch);
                }
            }
            
            if (_user1.GetVariable(this) == null) {
                string variableName = nameof(user1).Capitalize();
                var assetSearch = Utils.GetScriptableObject(nameof(user1).Capitalize()) as UserDataKey;
                if (assetSearch == null) {
                    var newVariable = CreateAppSetting(typeof(UserDataKey), variableName);
                    _user1.SetVariable(newVariable);
                }
                else {
                    _user1.SetVariable(assetSearch);
                }
            }
            
//            FieldInfo[] referenceFields = typeof(AppSettings).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

//            for (int i = 0; i < referenceFields.Length; i++) {
//
//                if (referenceFields[i].FieldType.IsSubclassOf(typeof(ScriptableObject))) {
//                    var scriptableObjectValue = referenceFields[i].GetValue(this) as ScriptableObject;
//                    if (scriptableObjectValue == null) {
//                        PopulateScriptableObjectField(this, referenceFields[i]);
//                    }
//                }
//                else {
//                    FieldInfo variableField = Utils.GetVariableFieldFromReference(referenceFields[i], this, out var referenceValue);
//                    var variableValue = variableField.GetValue(referenceValue) as ScriptableObject;
//                    if (variableValue == null) {
//                        PopulateReferenceField(referenceValue, referenceFields[i], variableField);
//                    }
//                }
//            }
            systemSettings.RefreshDependencies();
            userData.RefreshDependencies();
            inputData.RefreshDependencies();
            debugPreferences.RefreshDependencies(); 
#endif
        }

        private static object PopulateScriptableObjectField(object parentObject, FieldInfo scriptableObjectField)
        {
            string variableName = scriptableObjectField.Name.Replace("_", "").Capitalize();
            var variableSearch = Utils.GetScriptableObject(variableName) as ScriptableObject;
            if (variableSearch == null) {
                var newVariable = CreateAppSetting(scriptableObjectField.FieldType, variableName);
                scriptableObjectField.SetValue(parentObject, newVariable);
                return newVariable;
            }

            scriptableObjectField.SetValue(parentObject, variableSearch);
            return variableSearch;
        }
        
        private static object PopulateReferenceField(object referenceObject, FieldInfo referenceField, FieldInfo variableField)
        {
            string variableName = referenceField.Name.Replace("_", "").Capitalize();
            var variableSearch = Utils.GetCustomKey(variableName) as CustomKey;
            if (variableSearch == null) {
                Type variableType = variableField.FieldType;
                var newVariable = CreateAppSetting(variableType, variableName);
                variableField.SetValue(referenceObject, newVariable);
                return newVariable;
            }

            variableField.SetValue(referenceObject, variableSearch);
            return variableSearch;
        }
        
#if UNITY_EDITOR
        private static dynamic CreateAppSetting(Type assetType, string name)
        {
            return Utils.CreateScriptableObjectAsset(assetType, name, Utils.settingsPath);
        }
#endif

    }
}