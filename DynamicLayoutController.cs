using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor.Timeline;
using UnityEngine.Events;

namespace AltSalt.Maestro.Layout {
    
	#if UNITY_EDITOR
	[ExecuteInEditMode]
	#endif
    public class DynamicLayoutController : MonoBehaviour {

        [Required]
        [SerializeField]
        private AppSettings _appSettings;

        private AppSettings appSettings
        {
            get
            {
                if (_appSettings == null) {
                    _appSettings = Utils.GetAppSettings();
                }
                return _appSettings;
            }
            set => _appSettings = value;
        }
        
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
        
        [SerializeField]
        private FloatReference _sceneAspectRatio = new FloatReference();

        private float sceneAspectRatio
        {
            get => _sceneAspectRatio.GetValue();
            set => _sceneAspectRatio.SetValue(this.gameObject, value);
        }
        
        [SerializeField]
        private FloatReference _sceneWidth = new FloatReference();

        private float sceneWidth
        {
            get => _sceneWidth.GetValue();
            set => _sceneWidth.SetValue(this.gameObject, value);
        }
        
        [SerializeField]
        private FloatReference _sceneHeight = new FloatReference();

        private float sceneHeight
        {
            get => _sceneHeight.GetValue();
            set => _sceneHeight.SetValue(this.gameObject, value);
        }

        [SerializeField]
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

        [SerializeField]
        private UnityEvent _layoutRenderCompleteEvents;

        private UnityEvent layoutRenderCompleteEvents => _layoutRenderCompleteEvents;

        private enum ResizeType { VerticalPillarBox, HorizontalLetterBox, VerticalLetterBox, HorizontalPillarBox }
        
        private void Start()
		{
            SaveScreenValues();
            RefreshLayout();   
		}
        
        private void OnDisable()
        {
            priorityDynamicElements.Clear();
            dynamicElements.Clear();
        }

#if UNITY_EDITOR
        private void OnEnable()
        {
            _sceneAspectRatio.PopulateVariable(this, nameof(_sceneAspectRatio));
            _sceneWidth.PopulateVariable(this, nameof(_sceneWidth));
            _sceneHeight.PopulateVariable(this, nameof(_sceneHeight));
            
            if (appSettings == null) {
                appSettings = Utils.GetAppSettings();
            }
        }

        private void Update()
        {
            if (ScreenResized() == true) {
                SaveScreenValues();
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
#endif
        
        private void SaveScreenValues()
        {
            // Sometimes these values are erroneous, so make sure
            // we don't use them when they do
            if (Screen.width <= 0 || Screen.height <= 0) return;
            
            deviceWidth = Screen.width;
            deviceHeight = Screen.height;
            deviceAspectRatio = (float)Screen.height / Screen.width;
        }

        private void RefreshLayout()
        {
            CallExecuteLayoutUpdate();

            if (Application.isPlaying == true) {
                if(delayStart == false) {
                    layoutRenderCompleteEvents.Invoke();
                } else {
                    StartCoroutine(LayoutRenderCompleteTimeDelay());
                }
            }
        }
        
        private IEnumerator LayoutRenderCompleteTimeDelay()
        {
            yield return new WaitForSeconds(delayAmount);
            layoutRenderCompleteEvents.Invoke();
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
        public void CallExecuteLayoutUpdate()
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

#if UNITY_EDITOR
            if(TimelineEditor.inspectedDirector != null) {
                TimelineEditor.Refresh(RefreshReason.ContentsModified);
            }
#endif
        }

        private static List<IDynamicLayoutElement> UpdateDynamicElements(List<IDynamicLayoutElement> dynamicElementsList,
            DynamicLayoutController dynamicLayoutController, int minPriority, int maxPriority = int.MaxValue)
        {
            // Used to remove any elements that might be null
            List<IDynamicLayoutElement> elementsToRemove = new List<IDynamicLayoutElement>();
            
            for (int i=0; i<dynamicElementsList.Count; i++) {
                
                if (dynamicElementsList[i] == null || dynamicElementsList[i].elementName.Length < 0) {
                    elementsToRemove.Add(dynamicElementsList[i]);
                    continue;
                }

                if (dynamicElementsList[i].priority >= minPriority && dynamicElementsList[i].priority <= maxPriority) {
                    if (dynamicElementsList[i] is ISceneDimensionListener sceneDimensionListener) {
                        sceneDimensionListener.sceneAspectRatio = dynamicLayoutController.sceneAspectRatio;
                        sceneDimensionListener.sceneWidth = dynamicLayoutController.sceneWidth;
                        sceneDimensionListener.sceneHeight = dynamicLayoutController.sceneHeight;
                    }
                    dynamicElementsList[i].CallExecuteLayoutUpdate(dynamicLayoutController.gameObject);
                }
            }
            
            // Sanitize the list after we have iterated through it
            // so we don't interfere with the loop during iteration
            for (int i = 0; i < elementsToRemove.Count; i++) {
                dynamicElementsList.Remove(elementsToRemove[i]);
            }

            return dynamicElementsList;
        }

        private void PopulateSceneDimensions()
        {
            if(deviceBreakpoints.Count > 0) {
                int breakpointIndex = Utils.GetValueIndexInList(deviceAspectRatio, deviceBreakpoints);
                SceneDimension sceneDimension = sceneDimensions[breakpointIndex];
                
                switch (sceneDimension.targetAspectRatio) {

                    case AspectRatioType.x9x16:
                        if(sceneDimension.resizeType == ResizeType.VerticalPillarBox || sceneDimension.resizeType == ResizeType.HorizontalLetterBox) {
                            sceneWidth = deviceWidth;
                            sceneHeight = (16f * sceneWidth) / 9f;
                        } else {
                            sceneHeight = deviceHeight;
                            sceneWidth = (9f * sceneHeight) / 16f;
                        }
                        break;

                    case AspectRatioType.x3x4:
                        if (sceneDimension.resizeType == ResizeType.VerticalPillarBox || sceneDimension.resizeType == ResizeType.HorizontalLetterBox) {
                            sceneWidth = deviceWidth;
                            sceneHeight = (4f * sceneWidth) / 3f;
                        } else {
                            sceneHeight = deviceHeight;
                            sceneWidth = (3f * sceneHeight) / 4f;
                        }
                        break;
                    case AspectRatioType.x1x1:
                        sceneWidth = deviceWidth;
                        sceneHeight = deviceWidth;
                        break;

                    case AspectRatioType.Dynamic:
                        sceneWidth = deviceWidth;
                        sceneHeight = deviceHeight;
                        break;

                    default:
                        Debug.Log("Specified aspect ratio " + nameof(sceneDimension.targetAspectRatio) + " is not supported.");
                        break;
                }

                sceneAspectRatio = sceneHeight / sceneWidth;

            } else {

                sceneWidth = deviceWidth;
                sceneHeight = deviceHeight;
                sceneAspectRatio = deviceHeight / deviceWidth;

            }
        }
        
        [Serializable]
        private class SceneDimension
        {
            [ValueDropdown(nameof(aspectRatioValues))]
            [SerializeField]
            public AspectRatioType targetAspectRatio = AspectRatioType.x9x16;

            private ValueDropdownList<AspectRatioType> aspectRatioValues = new ValueDropdownList<AspectRatioType>(){
                {"9 x 16 | 16 x 9", AspectRatioType.x9x16  },
                {"3 x 4 | 4 x 3", AspectRatioType.x3x4 },
                {"1 x 1 | Box", AspectRatioType.x1x1 },
                {"None | Resize", AspectRatioType.Dynamic }
            };

            [ValueDropdown(nameof(resizeTypeValues))]
            [SerializeField]
            [ShowIf(nameof(CanChooseCrop))]
            public ResizeType resizeType = ResizeType.VerticalLetterBox;

            private ValueDropdownList<ResizeType> resizeTypeValues = new ValueDropdownList<ResizeType>(){
                {"Vertical : Crop or box top and bottom", ResizeType.VerticalPillarBox  },
                {"Vertical : Crop or box sides", ResizeType.VerticalLetterBox },
                {"Horizontal: Crop or box top and bottom", ResizeType.HorizontalPillarBox},
                {"Horizontal : Crop or box sides", ResizeType.HorizontalLetterBox  }
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