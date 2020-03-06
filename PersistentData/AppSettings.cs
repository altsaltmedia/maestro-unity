using System;
using System.Reflection;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro
{
    [CreateAssetMenu(menuName = "Maestro/Settings/App Settings")]
    public class AppSettings : ScriptableObject
    {
        [SerializeField, Required]
        private SystemSettings _systemSettings;

        private SystemSettings systemSettings
        {
            get
            {
#if UNITY_EDITOR
                if(_systemSettings == null) {
                   RefreshDependencies();
                }
#endif
                return _systemSettings;
            }
        }

        [SerializeField, Required]
        private UserData _userData;

        private UserData userData
        {
            get
            {
#if UNITY_EDITOR                
                if(_userData == null) {
                   RefreshDependencies();
                }
#endif                

                return _userData;
            }
        }

        [SerializeField, Required]
        private InputData _inputData;

        private InputData inputData
        {
            get
            {
#if UNITY_EDITOR                
                if(_inputData == null) {
                   RefreshDependencies();
                }
#endif                
                return _inputData;
            }
        }

        [SerializeField, Required]
        private DebugPreferences _debugPreferences;

        private DebugPreferences debugPreferences
        {
            get
            {
#if UNITY_EDITOR                
                if(_debugPreferences == null) {
                   RefreshDependencies();
                }
#endif                
                return _debugPreferences;
            }
        }

        [SerializeField, Required]
        private InputGroupKeyReference _mainInput = new InputGroupKeyReference();

        public InputGroupKey mainInput {

            get
            {
#if UNITY_EDITOR                
                if(_mainInput == null) {
                    RefreshDependencies();             
                }
#endif                
                return _mainInput.GetVariable() as InputGroupKey;
            }
            
        }

        [SerializeField, Required]
        private UserDataKeyReference _user1 = new UserDataKeyReference();

        public UserDataKey user1 {

            get
            {
#if UNITY_EDITOR                
                if(_user1 == null) {
                    RefreshDependencies();
                }
#endif                
                return _user1.GetVariable() as UserDataKey;                    
            }
            
        }

        public static void ResetGameState(GameObject callingObject)
        {
            var variableSearch = Resources.FindObjectsOfTypeAll(typeof(ModifiableEditorVariable));
            ModifiableEditorVariable[] modifiableEditorVariables =
                Array.ConvertAll(variableSearch, x => (ModifiableEditorVariable) x); 
            for (int i = 0; i < modifiableEditorVariables.Length; i++) {
                if (modifiableEditorVariables[i].hasDefault == true
                    && modifiableEditorVariables[i].resetOnGameRefresh == true) {
                    modifiableEditorVariables[i].StoreCaller(callingObject);
                    modifiableEditorVariables[i].SetToDefaultValue();
                }
            }
        }


    #region System Settings

    
        // Has Been Opened
    
        public bool GetHasBeenOpened(Object callingObject)
        {
            return systemSettings.hasBeenOpened.GetValue();
        }
        
        
        // Device Settings
        
        public float GetDeviceAspectRatio(Object callingObject)
        {
            return systemSettings.deviceAspectRatio.GetValue();
        }
        
        public FloatVariable SetDeviceAspectRatio(GameObject callingObject, float value)
        {
            return systemSettings.deviceAspectRatio.SetValue(callingObject, value);
        }
        
        public float GetDeviceWidth(Object callingObject)
        {
            return systemSettings.deviceWidth.GetValue();
        }
        
        public FloatVariable SetDeviceWidth(GameObject callingObject, float value)
        {
            return systemSettings.deviceWidth.SetValue(callingObject, value);
        }
        
        public float GetDeviceHeight(Object callingObject)
        {
            return systemSettings.deviceHeight.GetValue();
        }

        public FloatVariable SetDeviceHeight(GameObject callingObject, float value)
        {
            return systemSettings.deviceHeight.SetValue(callingObject, value);
        }
        
        
        
        // Scene Settings

        public string GetActiveScene(Object callingObject)
        {
            return systemSettings.activeScene.GetValue();
        }
        
        public StringVariable SetActiveScene(GameObject callingObject, string targetValue)
        {
            return systemSettings.activeScene.SetValue(callingObject, targetValue);
        }
        
        public float GetCurrentSceneAspectRatio(Object callingObject)
        {
            return systemSettings.currentSceneAspectRatio.GetValue();
        }
        
        public FloatVariable SetCurrentSceneAspectRatio(GameObject callingObject, float value)
        {
            return systemSettings.currentSceneAspectRatio.SetValue(callingObject, value);
        }
        
        public float GetCurrentSceneWidth(Object callingObject)
        {
            return systemSettings.currentSceneWidth.GetValue();
        }
        
        public FloatVariable SetCurrentSceneWidth(GameObject callingObject, float value)
        {
            return systemSettings.currentSceneWidth.SetValue(callingObject, value);
        }
        
        public float GetCurrentSceneHeight(Object callingObject)
        {
            return systemSettings.currentSceneHeight.GetValue();
        }

        public FloatVariable SetCurrentSceneHeight(GameObject callingObject, float value)
        {
            return systemSettings.currentSceneHeight.SetValue(callingObject, value);
        }
        
        
        // Audio

        public bool GetVolumeEnabled(Object callingObject)
        {
            return systemSettings.volumeEnabled.GetValue();
        }
        
        public BoolVariable SetVolumeEnabled(GameObject callingObject, bool targetValue)
        {
            return systemSettings.volumeEnabled.SetValue(callingObject, targetValue);
        }

        public bool GetMusicEnabled(Object callingObject)
        {
            return systemSettings.musicEnabled.GetValue();
        }

        public bool GetSoundEffectsEnabled(Object callingObject)
        {
            return systemSettings.soundEffectsEnabled.GetValue();
        }
        
        
        // Progress Bar
        
        public bool GetProgressBarVisible(Object callingObject)
        {
            return systemSettings.progressBarVisible.GetValue();
        }
        
        public bool SetProgressBarVisible(GameObject callingObject, bool targetValue)
        {
            return systemSettings.progressBarVisible.SetValue(callingObject, targetValue);
        }
        
        
        // Paused
        
        public bool GetIsPaused(Object callingObject)
        {
            return systemSettings.paused.GetValue();
        }
        
        
        // Frames Per Second
        
        public int GetFramesPerSecond(Object callingObject)
        {
            return systemSettings.framesPerSecond.GetValue();
        }
        
        public IntVariable SetFramesPerSecond(GameObject callingObject, int targetValue)
        {
            return systemSettings.framesPerSecond.SetValue(callingObject, targetValue);
        }
        
        
        // Timescale
        
        public float GetTimescale(Object callingObject)
        {
            return systemSettings.timescale.GetValue();
        }
        

        // Has Bookmark
        
        public BoolReference GetHasBookmarkReference(Object callingObject)
        {
            return systemSettings.hasBookmark;
        }
        
        
        // Last Opened Scene
        
        public StringReference GetLastOpenedSceneReference(Object callingObject)
        {
            return systemSettings.lastOpenedScene;
        }
        
        
        // Last Loaded Sequence
        
        public StringReference GetLastLoadedSequenceReference(Object callingObject)
        {
            return systemSettings.lastLoadedSequence;
        }

        
        // Last Loaded Sequence Time
        
        public FloatReference GetLastLoadedSequenceTimeReference(Object callingObject)
        {
            return systemSettings.lastLoadedSequenceTime;
        }
        
        
        // Scene Loading Progress
        
        public float GetSceneLoadingProgress(Object callingObject)
        {
            return systemSettings.sceneLoadingProgress.GetValue();
        }
        
        public FloatVariable SetSceneLoadingProgress(GameObject callingObject, float targetValue)
        {
            return systemSettings.sceneLoadingProgress.SetValue(callingObject, targetValue);
        }

        
    #endregion
    

    #region User Data

    
        // Y Sensitivity
    
        public float GetYSensitivity(Object callingObject, UserDataKey userKey)
        {
            return userData.GetUserPreferences(userKey).ySensitivity.GetValue();
        }
        
        
        // Invert Y Input
        
        public bool GetInvertYInput(Object callingObject, UserDataKey userKey)
        {
            return userData.GetUserPreferences(userKey).invertYInput.GetValue();
        }
        
        
        // X Sensitivity

        public float GetXSensitivity(Object callingObject, UserDataKey userKey)
        {
            return userData.GetUserPreferences(userKey).xSensitivity.GetValue();
        }
        
        
        // Invert X Input
        
        public bool GetInvertXInput(Object callingObject, UserDataKey userKey)
        {
            return userData.GetUserPreferences(userKey).invertXInput.GetValue();
        }
        
        
        // Autoplay Enabled
        
        public bool GetUserAutoplayEnabled(Object callingObject, UserDataKey userKey)
        {
            return userData.GetUserPreferences(userKey).userAutoplayEnabled.GetValue();
        }
        
        
        // Momentum Enabled

        public bool GetUserMomentumEnabled(Object callingObject, UserDataKey userKey)
        {
            return userData.GetUserPreferences(userKey).userMomentumEnabled.GetValue();
        }

        
    #endregion


    #region Input Data

    
        // Is Swiping
        
        public bool GetIsSwiping(GameObject callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).isSwiping.GetValue();
        }
        
        public BoolVariable SetIsSwiping(GameObject callingObject, InputGroupKey inputGroupKey, bool targetValue)
        {
            return inputData.GetInputGroup(inputGroupKey).isSwiping.SetValue(callingObject, targetValue);
        }
    
        
        // Swipe Force
    
        public Vector2 GetSwipeForce(GameObject callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).swipeForce.GetValue();
        }
        
        public V2Variable SetSwipeForce(GameObject callingObject, InputGroupKey inputGroupKey, Vector2 targetValue)
        {
            return inputData.GetInputGroup(inputGroupKey).swipeForce.SetValue(callingObject, targetValue);
        }
        
        
        // Swipe Monitor Momentum
        
        public Vector2 GetSwipeMonitorMomentum(GameObject callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).swipeMonitorMomentum.GetValue();
        }
        
        public V2Variable SetSwipeMonitorMomentum(GameObject callingObject, InputGroupKey inputGroupKey, Vector2 targetValue)
        {
            return inputData.GetInputGroup(inputGroupKey).swipeMonitorMomentum.SetValue(callingObject, targetValue);
        }

        
        // Swipe Monitor Momentum Cache
        
        public Vector2 GetSwipeMonitorMomentumCache(GameObject callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).swipeMonitorMomentumCache.GetValue();
        }
        
        public V2Variable SetSwipeMonitorMomentumCache(GameObject callingObject, InputGroupKey inputGroupKey, Vector2 targetValue)
        {
            return inputData.GetInputGroup(inputGroupKey).swipeMonitorMomentumCache.SetValue(callingObject, targetValue);
        }
        
        
        // Axis Transition Active

        public bool GetAxisTransitionActive(GameObject callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).axisTransitionActive.GetValue();
        }
        
        public BoolVariable SetAxisTransitionActive(GameObject callingObject, InputGroupKey inputGroupKey, bool targetValue)
        {
            return inputData.GetInputGroup(inputGroupKey).axisTransitionActive.SetValue(callingObject, targetValue);
        }
        
        
        // Fork Transition Active

        public bool GetForkTransitionActive(GameObject callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).forkTransitionActive.GetValue();
        }
        
        public BoolVariable SetForkTransitionActive(GameObject callingObject, InputGroupKey inputGroupKey, bool targetValue)
        {
            return inputData.GetInputGroup(inputGroupKey).forkTransitionActive.SetValue(callingObject, targetValue);
        }


        // Axis Transition Spread

        public float GetAxisTransitionSpread(GameObject callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).axisTransitionSpread.GetValue();
        }
        
        
        // Fork Transition Spread

        public float GetForkTransitionSpread(GameObject callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).forkTransitionSpread.GetValue();
        }


        // Swipe Min Max
        
        public float GetSwipeMinMax(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).swipeMinMax.GetValue();
        }
        
        
        // Momentum Min Max
        
        public float GetMomentumMinMax(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).momentumMinMax.GetValue();
        }
        
        
        // Momentum Decay
        
        public float GetMomentumDecay(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).momentumDecay.GetValue();
        }
        
        
        // Momentum Sensitivity

        public float GetMomentumSensitivity(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).momentumSensitivity.GetValue();
        }
        
        
        // Gesture Time Multiplier

        public float GetGestureTimeMultiplier(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).gestureTimeMultiplier.GetValue();
        }

        
        // Cancel Momentum Time Threshold

        public float GetCancelMomentumTimeThreshold(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).cancelMomentumTimeThreshold.GetValue();
        }
        
        
        // Pause Momentum Threshold

        public float GetPauseMomentumTimeThreshold(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).pauseMomentumThreshold.GetValue();
        }
        
        
        // Cancel Momentum Time Threshold

        public float GetCancelMomentumMagnitudeThreshold(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).cancelMomentumMagnitudeThreshold.GetValue();
        }
        
        
        // Flick Threshold
        
        public float GetFlickThreshold(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).flickThreshold.GetValue();
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
            return inputData.GetInputGroup(inputGroupKey).swipeDirection.GetValue();
        }
        
        public StringVariable SetSwipeDirection(GameObject callingObject, InputGroupKey inputGroupKey, string targetValue)
        {
            return inputData.GetInputGroup(inputGroupKey).swipeDirection.SetValue(callingObject, targetValue);
        }
        
        
        // Gesture Action Time

        public float GetGestureActionTime(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).gestureActionTime.GetValue();
        }
        
        public FloatVariable SetGestureActionTime(GameObject callingObject, InputGroupKey inputGroupKey, float targetValue)
        {
            return inputData.GetInputGroup(inputGroupKey).gestureActionTime.SetValue(callingObject, targetValue);
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

        public SimpleEventTrigger GetSequenceModified(GameObject callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).sequenceModified;
        }
        
        public ComplexEventManualTrigger GetInputActionComplete(GameObject callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).inputActionComplete;
        }

        public SimpleEventTrigger GetAutoplayActivate(GameObject callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).autoplayActivate;
        }
        
        public ComplexEventManualTrigger GetUpdateFork(GameObject callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).updateFork;
        }
        
        
        // Refresh App Utils
        
        public ComplexEventManualTrigger GetRefreshAppUtils(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).refreshAppUtils;
        }
        
        
        // Is Scrubbing
        
        public bool GetIsScrubbing(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).isScrubbing.GetValue();
        }

        public BoolVariable SetIsScrubbing(GameObject callingObject, InputGroupKey inputGroupKey, bool targetValue)
        {
            return inputData.GetInputGroup(inputGroupKey).isScrubbing.SetValue(callingObject, targetValue);
        }
        
        
        // On Scrub
        
        public SimpleEventTrigger GetOnScrub(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).onScrub;
        }
        
        
        // App Utils Requested

        public bool GetAppUtilsRequested(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).appUtilsRequested.GetValue();
        }
        
        public BoolVariable SetAppUtilsRequested(GameObject callingObject, InputGroupKey inputGroupKey, bool targetValue)
        {
            return inputData.GetInputGroup(inputGroupKey).appUtilsRequested.SetValue(callingObject, targetValue);
        }
        
        
        // Configuration Completed
        
        public SimpleEventTrigger GetConfigurationCompleted(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).configurationCompleted;
        }
        
        
        // Bookmark Loading Completed
        
        public bool GetBookmarkLoadingCompleted(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).bookmarkLoadingCompleted.GetValue();
        }
        
        public bool SetBookmarkLoadingCompleted(GameObject callingObject, InputGroupKey inputGroupKey, bool targetValue)
        {
            return inputData.GetInputGroup(inputGroupKey).bookmarkLoadingCompleted.SetValue(callingObject, targetValue);
        }
        

        // Is Reversing
        
        public bool GetIsReversing(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).isReversing.GetValue();
        }
        
        public BoolReference GetIsReversingReference(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).isReversing;
        }
        
        public BoolVariable SetIsReversing(GameObject callingObject, InputGroupKey inputGroupKey, bool targetValue)
        {
            return inputData.GetInputGroup(inputGroupKey).isReversing.SetValue(callingObject, targetValue);
        }
        
        
        // Swipe Modifier Output
        
        public float GetSwipeModifierOutput(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).swipeModifierOutput.GetValue();
        }
        
        public FloatVariable SetSwipeModifierOutput(GameObject callingObject, InputGroupKey inputGroupKey, float targetValue)
        {
            return inputData.GetInputGroup(inputGroupKey).swipeModifierOutput.SetValue(callingObject, targetValue);
        }
        
        
        // Momentum Modifier Output
        
        public float GetMomentumModifierOutput(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).momentumModifierOutput.GetValue();
        }
        
        public FloatVariable SetMomentumModifierOutput(GameObject callingObject, InputGroupKey inputGroupKey, float targetValue)
        {
            return inputData.GetInputGroup(inputGroupKey).momentumModifierOutput.SetValue(callingObject, targetValue);
        }

        
        // Lerp Speed
        
        public float GetLerpSpeed(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).lerpSpeed.GetValue();
        }
        
        
        // Frame Step Value
        
        public float GetFrameStepValue(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).frameStepValue.GetValue();
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
        
        
        // Scrubber Enabled
        
        public bool GetScrubberEnabled(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).scrubberEnabled.GetValue();
        }
        
        
        // Arrow Indicator Enabled

        public bool GetArrowIndicatorEnabled(Object callingObject, InputGroupKey inputGroupKey)
        {
            return inputData.GetInputGroup(inputGroupKey).arrowIndicatorEnabled.GetValue();
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
        
        public bool bookmarkingEnabled => debugPreferences.bookmarkingEnabled;

        public static bool logEventCallersAndListeners => DebugPreferences.logEventCallersAndListeners;

        public static bool logGlobalResponsiveElementActions => DebugPreferences.logResponsiveElementActions;

        public static bool logConditionResponses => DebugPreferences.logConditionResponses;

    #endregion
        
        
#if UNITY_EDITOR

        private void OnEnable()
        {
            _mainInput.PopulateVariable(this, nameof(_mainInput));
            _user1.PopulateVariable(this, nameof(_user1));
        }

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

            _mainInput.isSystemReference = true;
            if (_mainInput.GetVariable() == null) {
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

            _user1.isSystemReference = true;
            if (_user1.GetVariable() == null) {
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

            systemSettings.RefreshDependencies();
            userData.RefreshDependencies();
            inputData.RefreshDependencies();
            debugPreferences.RefreshDependencies(); 
        }
        
        private static dynamic CreateAppSetting(Type assetType, string name)
        {
            return Utils.ForceCreateScriptableObjectAsset(assetType, name, Utils.settingsPath);
        }

#endif

    }
}