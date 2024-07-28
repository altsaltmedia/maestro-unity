using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.Timeline;
using UnityEditor.SceneManagement;
#endif

namespace AltSalt.Maestro.Layout {
    
	[ExecuteInEditMode]
    public class DynamicLayoutController : MonoBehaviour {

        [Required]
        [SerializeField]
        [ReadOnly]
        private AppSettingsReference _appSettings = new AppSettingsReference();

        private AppSettings appSettings => _appSettings.GetVariable() as AppSettings;

        [SerializeField]
        private bool _refreshLayoutOnStart;

        private bool refreshLayoutOnStart => _refreshLayoutOnStart;

        private float deviceAspectRatio
        {
            get => appSettings.GetDeviceAspectRatio(this);
            set => appSettings.SetDeviceAspectRatio(this.gameObject, value);
        }

        private float deviceWidth
        {
            get => appSettings.GetDeviceWidth(this);
            set => appSettings.SetDeviceWidth(this.gameObject, value);
        }

        private float deviceHeight
        {
            get => appSettings.GetDeviceHeight(this);
            set => appSettings.SetDeviceHeight(this.gameObject, value);
        }
        
        [FormerlySerializedAs("_sceneAspectRatio"),SerializeField]
        private FloatReference _targetAspectRatio = new FloatReference();

        private float targetAspectRatio
        {
            get => _targetAspectRatio.GetValue();
            set => _targetAspectRatio.SetValue(this.gameObject, value);
        }
        
        [FormerlySerializedAs("_sceneWidth"),SerializeField]
        private FloatReference _targetWidth = new FloatReference();

        private float sceneWidth
        {
            get => _targetWidth.GetValue();
            set => _targetWidth.SetValue(this.gameObject, value);
        }
        
        [FormerlySerializedAs("_sceneHeight"),SerializeField]
        private FloatReference _targetHeight = new FloatReference();

        private float sceneHeight
        {
            get => _targetHeight.GetValue();
            set => _targetHeight.SetValue(this.gameObject, value);
        }
        
        [SerializeField]
        private SimpleEventTrigger _callbackEvent;

        private SimpleEventTrigger callbackEvent => _callbackEvent;
        
        [SerializeField]
        private bool _setScreenOrientation;

        private bool setScreenOrientation => _setScreenOrientation;

        [SerializeField]
        [ShowIf(nameof(setScreenOrientation))]
        private DimensionType _primaryOrientation;

        private DimensionType primaryOrientation => _primaryOrientation;

        [SerializeField]
        private bool _hasBreakpoints;

        private bool hasBreakpoints => _hasBreakpoints;

        [SerializeField]
        [ShowIf(nameof(hasBreakpoints))]
        [InfoBox("You should always have x+1 breakpoint values; e.g., for 1 breakpoint at 1.34, you must specify 2 breakpoint values - one for aspect ratios wider than 1.34, and another for aspect ratios narrower than or equal to 1.34.")]
        [InfoBox("Breakpoint examples: To target devices narrow than iPad (aspect ratio 1.33), specify a breakpoint of 1.34; to target narrower than iPhone (1.77), specify a breakpoint of 1.78.")]
        private List<float> _deviceBreakpoints = new List<float>();

        private List<float> deviceBreakpoints => _deviceBreakpoints;

        [SerializeField]
        private List<SceneDimension> _sceneDimensions = new List<SceneDimension>();

        private List<SceneDimension> sceneDimensions => _sceneDimensions;

        [ShowInInspector]
        private List<IDynamicLayoutElement> _priorityDynamicElements = new List<IDynamicLayoutElement>();

        private List<IDynamicLayoutElement> priorityDynamicElements => _priorityDynamicElements;
        
        [ShowInInspector]
        private List<IDynamicLayoutElement> _dynamicElements = new List<IDynamicLayoutElement>();

        private List<IDynamicLayoutElement> dynamicElements => _dynamicElements;

        [SerializeField]
        private bool _delayStart;

        private bool delayStart => _delayStart;

        [SerializeField]
        [ShowIf(nameof(delayStart))]
        private float _delayAmount;

        private float delayAmount => _delayAmount;

        private enum ResizeType { PillarBox, LetterBox }

        private bool _mobilePlayerActive;

        public bool mobilePlayerActive
        {
            get => _mobilePlayerActive;
            set => _mobilePlayerActive = value;
        }

        private bool _isFullscreen;

        private bool isFullscreen
        {
            get => _isFullscreen;
            set => _isFullscreen = value;
        }

        private void Start()
		{
#if !UNITY_EDITOR && UNITY_IOS || UNITY_ANDROID
            mobilePlayerActive = true;      
#endif
            if (Application.isPlaying == false || refreshLayoutOnStart == true) {
                RefreshLayout();
            }

            isFullscreen = Screen.fullScreen;
		}
        
        private void OnDisable()
        {
            priorityDynamicElements.Clear();
            dynamicElements.Clear();
        }

#if UNITY_EDITOR
        private void OnEnable()
        {
            _appSettings.PopulateVariable(this, nameof(_appSettings));
            _targetAspectRatio.PopulateVariable(this, nameof(_targetAspectRatio));
            _targetWidth.PopulateVariable(this, nameof(_targetWidth));
            _targetHeight.PopulateVariable(this, nameof(_targetHeight));
            _callbackEvent.PopulateVariable(this, nameof(_callbackEvent));

            EditorSceneManager.sceneSaved += scene =>
            {
                if (TimelineEditor.inspectedDirector != null) {
                    TimelineEditor.Refresh(RefreshReason.ContentsModified);
                    TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);
                    TimelineEditor.Refresh(RefreshReason.WindowNeedsRedraw);
                }
            };
        }
#endif

        private void Update()
        {
#if UNITY_EDITOR
            // Do not execute if we are in prefab editing mode
            if (PrefabStageUtility.GetCurrentPrefabStage() != null) {
                return;
            }
#endif
            if (ScreenResized() == true || isFullscreen != Screen.fullScreen) {
                RefreshLayout();
            }
        }

        private bool ScreenResized()
        {
            if (Mathf.Approximately(deviceWidth, Screen.width) == false ||
                Mathf.Approximately(deviceHeight, Screen.height) == false) {
                return true;
            }

            return false;
        }

        public void RegisterDynamicElement(ComplexPayload complexPayload)
        {
            IDynamicLayoutElement element = complexPayload.GetObjectValue() as IDynamicLayoutElement;
            
            if(element.parentScene == this.gameObject.scene &&
               priorityDynamicElements.Contains(element) == false && dynamicElements.Contains(element) == false) {

                if(element.priority != 0) {
                    priorityDynamicElements.Add(element);
                } else {
                    dynamicElements.Add(element);
                }
            }
        }

        public void DeregisterDynamicElement(ComplexPayload complexPayload)
        {
            IDynamicLayoutElement element = complexPayload.GetObjectValue() as IDynamicLayoutElement;
            if(priorityDynamicElements.Contains(element) == true) {
                priorityDynamicElements.Remove(element);
            }
            if (dynamicElements.Contains(element) == true) {
                dynamicElements.Remove(element);
            }
        }
        
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        [PropertyOrder(8)]
        public void CallRefreshLayout()
        {
            RefreshLayout();
        }

        private void RefreshLayout()
        {
            StartCoroutine(SaveScreenValues(() =>
            {
                PopulateSceneDimensions();
            
                if (appSettings.dynamicLayoutActive == false) {
                    return;
                }

                // We track all priority responsive elements separately
                // because sorting is an expensive operation
                priorityDynamicElements.Sort(new ResponsiveUtilsCore.DynamicElementSort());
                List<IDynamicLayoutElement> elementsToRemove = new List<IDynamicLayoutElement>();

                UpdateDynamicElements(priorityDynamicElements, this, 1);
                UpdateDynamicElements(dynamicElements, this, int.MinValue);
                UpdateDynamicElements(priorityDynamicElements, this, int.MinValue, 0);

                if (Application.isPlaying == true) {
                    if(delayStart == false) {
                        if (callbackEvent.GetVariable() != null) {
                            callbackEvent.RaiseEvent(this.gameObject);
                        }
                    } else {
                        StartCoroutine(LayoutRenderCompleteTimeDelay());
                    }
                }
            }));
        }

        private IEnumerator SaveScreenValues(Action callback)
        {
            // Sometimes these values are erroneous, so make sure
            // we don't use them when they are
            if (Screen.width <= 0 || Screen.height <= 0) yield break;
            
            // It takes a few moments for the screen orientation to
            // update on device, so yield until the values have been updated
            if (mobilePlayerActive == true && setScreenOrientation == true) {
                if (primaryOrientation == DimensionType.Vertical) {
                    Screen.autorotateToPortrait = true;
                    Screen.autorotateToPortraitUpsideDown = false;
                    Screen.autorotateToLandscapeLeft = false;
                    Screen.autorotateToLandscapeRight = false;
                    Screen.orientation = ScreenOrientation.Portrait;

                    while (Screen.height < Screen.width) {
                        yield return null;
                    }
                    
                }
                else {
                    Screen.autorotateToPortrait = false;
                    Screen.autorotateToPortraitUpsideDown = false;
                    Screen.autorotateToLandscapeLeft = true;
                    Screen.autorotateToLandscapeRight = true;
                    Screen.orientation = ScreenOrientation.AutoRotation;
                    
                    while (Screen.width < Screen.height) {
                        yield return null;
                    }
                }
            }
            
            deviceWidth = Screen.width;
            deviceHeight = Screen.height;

            // Aspect ratio is calculated by dividing device height
            // by device width. In the case of horizontal orientation
            // in the editor, however, the screen values will be reversed,
            // so we need to account for that in order to get the
            // correct aspect ratio value, i.e. (4:3 = 1.333, 16:9 = 1.777)
#if UNITY_IOS || UNITY_ANDROID
                    if (Screen.height > Screen.width) {
                         deviceAspectRatio = deviceHeight / deviceWidth;
                    }
                    else {
                         deviceAspectRatio = deviceWidth / deviceHeight;
                    }
#endif
#if PLATFORM_STANDALONE_OSX
            deviceWidth = Screen.safeArea.width;
            deviceHeight = Screen.safeArea.height;
            deviceAspectRatio = deviceHeight / deviceWidth;
#endif
            callback();
        }

        private static List<IDynamicLayoutElement> UpdateDynamicElements(List<IDynamicLayoutElement> dynamicElementsList,
            DynamicLayoutController dynamicLayoutController, int minPriority, int maxPriority = int.MaxValue)
        {
            // Sanitize our list in case any elements have been removed
            for (int i = 0; i < dynamicElementsList.Count; i++) {
                if (dynamicElementsList[i] == null || dynamicElementsList[i].Equals(null)) {
                    dynamicElementsList.Remove(dynamicElementsList[i]);
                }
            }

            for (int i = 0; i < dynamicElementsList.Count; i++) {
                if (dynamicElementsList[i].priority >= minPriority && dynamicElementsList[i].priority <= maxPriority) {
                    if (dynamicElementsList[i] is ISceneDimensionListener sceneDimensionListener) {
                        sceneDimensionListener.sceneAspectRatio = dynamicLayoutController.targetAspectRatio;
                        sceneDimensionListener.sceneWidth = dynamicLayoutController.sceneWidth;
                        sceneDimensionListener.sceneHeight = dynamicLayoutController.sceneHeight;
                    }
                    dynamicElementsList[i].CallExecuteLayoutUpdate(dynamicLayoutController.gameObject);
                }
            }

            return dynamicElementsList;
        }

        private void PopulateSceneDimensions()
        {
            if(sceneDimensions.Count > 0) {
                
                int breakpointIndex = 0;

                if (hasBreakpoints == true) {
                    breakpointIndex = Utils.GetValueIndexInList(deviceAspectRatio, deviceBreakpoints);
                }
                
                SceneDimension sceneDimension = sceneDimensions[breakpointIndex];

                switch (sceneDimension.targetAspectRatio) {

                    case AspectRatioType.x9x16:
                        if (deviceHeight > deviceWidth) {
                            if (deviceAspectRatio > 1.77) {
                                sceneWidth = deviceWidth;
                                sceneHeight = (16f * sceneWidth) / 9f;
                            }
                            else {
                                sceneHeight = deviceHeight;
                                sceneWidth = (9f * sceneHeight) / 16f;
                            }
                        }
                        else {
                            sceneHeight = deviceHeight;
                            sceneWidth = (9f * sceneHeight) / 16f;
                        }
                        break;
                    
                    case AspectRatioType.x16x9:
                        if (deviceWidth > deviceHeight) {
                            if (deviceAspectRatio > 1.77) {
                                sceneHeight = deviceHeight;
                                sceneWidth = (16f * sceneHeight) / 9f;
                            }
                            else {
                                sceneWidth = deviceWidth;
                                sceneHeight = (9f * sceneWidth) / 16f;
                            }
                        }
                        else {
                            sceneWidth = deviceWidth;
                            sceneHeight = (9f * sceneWidth) / 16f;
                        }
                        break;

                    case AspectRatioType.x3x4:
                        if (deviceHeight > deviceWidth) {
                            if (deviceAspectRatio > 1.33) {
                                sceneWidth = deviceWidth;
                                sceneHeight = (4f * sceneWidth) / 3f;
                            } else {
                                sceneHeight = deviceHeight;
                                sceneWidth = (3f * sceneHeight) / 4f;
                            }
                        }
                        else {
                            sceneHeight = deviceHeight;
                            sceneWidth = (3f * sceneHeight) / 4f;
                        }
                        break;
                    
                    case AspectRatioType.x4x3:
                        if (deviceWidth > deviceHeight) {
                            if (deviceAspectRatio > 1.33) {
                                sceneHeight = deviceHeight;
                                sceneWidth = (4f * sceneHeight) / 3f;
                            } else {
                                sceneWidth = deviceWidth;
                                sceneHeight = (3f * sceneWidth) / 4f;
                            }
                        }
                        else {
                            sceneWidth = deviceWidth;
                            sceneHeight = (3f * sceneWidth) / 4f;
                        }
                        break;

                    case AspectRatioType.x1x1:
                        if (deviceHeight > deviceWidth) {
                            sceneWidth = deviceWidth;
                            sceneHeight = deviceWidth;
                        }
                        else {
                            sceneHeight = deviceHeight;
                            sceneWidth = deviceHeight;
                        }
                        break;

                    case AspectRatioType.Dynamic:
                        sceneWidth = deviceWidth;
                        sceneHeight = deviceHeight;
                        break;

                    default:
                        Debug.Log("Specified aspect ratio " + nameof(sceneDimension.targetAspectRatio) + " is not supported.");
                        break;
                }
                
            } else {
                sceneWidth = deviceWidth;
                sceneHeight = deviceHeight;
            }
            
            // Aspect ratio is calculated by dividing device height
            // by device width. In the case of a horizontal orientation, however,
            // then screen values will be reversed, so we need to account for
            // that in order to get the correct aspect ratio value, i.e.
            // (4:3 = 1.333, 16:9 = 1.777)
            if (sceneHeight > sceneWidth) {
                targetAspectRatio = sceneHeight / sceneWidth;
            }
            else {
                targetAspectRatio = sceneWidth / sceneHeight;
            }
        }
        
        private IEnumerator LayoutRenderCompleteTimeDelay()
        {
            yield return new WaitForSeconds(delayAmount);
            if (callbackEvent.GetVariable() != null) {
                callbackEvent.RaiseEvent(this.gameObject);
            }
        }
        
        [Serializable]
        private class SceneDimension
        {
            [ValueDropdown(nameof(aspectRatioValues))]
            [SerializeField]
            public AspectRatioType targetAspectRatio = AspectRatioType.x9x16;

            private ValueDropdownList<AspectRatioType> aspectRatioValues = new ValueDropdownList<AspectRatioType>(){
                {"9 x 16", AspectRatioType.x9x16  },
                {"16 x 9", AspectRatioType.x16x9  },
                {"3 x 4", AspectRatioType.x3x4 },
                {"4 x 3", AspectRatioType.x4x3 },
                {"1 x 1 | Box", AspectRatioType.x1x1 },
                {"None | Resize", AspectRatioType.Dynamic }
            };

            [ValueDropdown(nameof(resizeTypeValues))]
            [SerializeField]
            [ShowIf(nameof(CanChooseCrop))]
            [HideInInspector]
            public ResizeType resizeType = ResizeType.LetterBox;

            private ValueDropdownList<ResizeType> resizeTypeValues = new ValueDropdownList<ResizeType>(){
                {"Pillarbox : Crop or box top and bottom", ResizeType.PillarBox  },
                {"Letterbox : Crop or box sides", ResizeType.LetterBox }
            };

            private bool CanChooseCrop()
            {
                if(targetAspectRatio == AspectRatioType.Dynamic || targetAspectRatio == AspectRatioType.x1x1) {
                    return false;
                }
                return true;
            }
        }

        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
	}
	
}