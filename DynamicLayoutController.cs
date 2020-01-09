using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Events;

namespace AltSalt.Maestro
{
    public class DynamicLayoutController : MonoBehaviour, ISkipRegistration
    {
        public bool DoNotRecord => true;
        
        [Required]
        [SerializeField]
        [ReadOnly]
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

        [SerializeField]
        private DimensionType _orientation;

        private DimensionType orientation => _orientation;

        [SerializeField]
        private bool _delayStart;

        private bool delayStart => _delayStart;

        [SerializeField]
        [ShowIf(nameof(delayStart))]
        private float _delayAmount;

        private float delayAmount => _delayAmount;

        [SerializeField]
        [Required]
        private SimpleEventTrigger _fadeInTriggered;

        private SimpleEventTrigger fadeInTriggered => _fadeInTriggered;

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private FloatReference _deviceAspectRatio;

        private float deviceAspectRatio => _deviceAspectRatio.GetValue(this.gameObject);

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private FloatReference _deviceWidth;

        private float deviceWidth => _deviceWidth.GetValue(this.gameObject);

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private FloatReference _deviceHeight;

        private float deviceHeight => _deviceHeight.GetValue(this.gameObject);

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private FloatReference _sceneAspectRatio = new FloatReference();

        private float sceneAspectRatio
        {
            get => _sceneAspectRatio.GetValue(this.gameObject);
            set => _sceneAspectRatio.GetVariable(this.gameObject).SetValue(value);
        }

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private FloatReference _sceneWidth = new FloatReference();

        private float sceneWidth
        {
            get => _sceneWidth.GetValue(this.gameObject);
            set => _sceneWidth.GetVariable(this.gameObject).SetValue(value);
        }

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private FloatReference _sceneHeight = new FloatReference();

        private float sceneHeight
        {
            get => _sceneHeight.GetValue(this.gameObject);
            set => _sceneHeight.GetVariable(this.gameObject).SetValue(value);
        }

        [SerializeField]
        private List<float> _deviceBreakpoints = new List<float>();

        private List<float> deviceBreakpoints => _deviceBreakpoints;

        [SerializeField]
        private List<SceneDimension> _sceneDimensions = new List<SceneDimension>();

        private List<SceneDimension> sceneDimensions => _sceneDimensions;

        [SerializeField]
        private List<IDynamicLayoutElement> _priorityDynamicElements = new List<IDynamicLayoutElement>();

        private List<IDynamicLayoutElement> priorityDynamicElements => _priorityDynamicElements;

        [SerializeField]
        private List<IDynamicLayoutElement> _dynamicElements = new List<IDynamicLayoutElement>();

        private List<IDynamicLayoutElement> dynamicElements => _dynamicElements;

        [SerializeField]
        [Required]
        private SimpleEventTrigger _layoutRendered;

        private SimpleEventTrigger layoutRendered => _layoutRendered;
        
        [SerializeField]
        private UnityEvent _layoutRenderCompleteEvents;

        private UnityEvent layoutRenderCompleteEvents => _layoutRenderCompleteEvents;

        private void Start()
        {
            RefreshLayout();
        }

        private void RefreshLayout()
        {
            if(orientation == DimensionType.Vertical) {
                Screen.orientation = ScreenOrientation.Portrait;
            } else {
                Screen.orientation = ScreenOrientation.LandscapeLeft;
            }

            CallExecuteLayoutUpdate();

            if(delayStart == false) {
                layoutRendered.RaiseEvent(this.gameObject);
                layoutRenderCompleteEvents.Invoke();
            } else {
                StartCoroutine(LayoutRenderCompleteTimeDelay());
            }
        }
        
        void OnDisable()
        {
            priorityDynamicElements.Clear();
            dynamicElements.Clear();
        }


        private IEnumerator LayoutRenderCompleteTimeDelay()
        {
            yield return new WaitForSeconds(delayAmount);
            layoutRendered.RaiseEvent(this.gameObject);
            layoutRenderCompleteEvents.Invoke();
        }
        
        
        public void AddResponsiveElement(EventPayload eventPayload)
        {
            IDynamicLayoutElement element = eventPayload.objectDictionary[DataType.systemObjectType] as IDynamicLayoutElement;
            
            if(element.parentScene == this.gameObject.scene &&
               priorityDynamicElements.Contains(element) == false && dynamicElements.Contains(element) == false) {

                if(element.priority > 0) {
                    priorityDynamicElements.Add(element);
                } else {
                    dynamicElements.Add(element);
                }
            }
        }

        public void RemoveResponsiveElement(EventPayload eventPayload)
        {
            IDynamicLayoutElement element = eventPayload.objectDictionary[DataType.systemObjectType] as IDynamicLayoutElement;
            if(priorityDynamicElements.Contains(element) == true) {
                priorityDynamicElements.Remove(element);
            }
            if (dynamicElements.Contains(element) == true) {
                dynamicElements.Remove(element);
            }
        }
        
        
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

            // Start with priorities greater than 0
            for (int i=0; i<priorityDynamicElements.Count; i++) {
                
                if (priorityDynamicElements[i] == null || priorityDynamicElements[i].elementName.Length < 0) {
                    elementsToRemove.Add(priorityDynamicElements[i]);
                    continue;
                }

                if (priorityDynamicElements[i].priority > 0) {
                    if (priorityDynamicElements[i] is IResponsiveBreakpoints responsiveBreakpoints) {
                        responsiveBreakpoints.sceneAspectRatio = sceneAspectRatio;
                        responsiveBreakpoints.sceneHeight = sceneHeight;
                        responsiveBreakpoints.sceneWidth = sceneWidth;
                    }
                    priorityDynamicElements[i].CallExecuteLayoutUpdate(this.gameObject);
                }
            }

            for (int i = 0; i < elementsToRemove.Count; i++) {
                priorityDynamicElements.Remove(elementsToRemove[i]);
            }
            elementsToRemove.Clear();

            // All other elements with standard priority of 0, executed in any order
            for (int i=0; i<dynamicElements.Count; i++) {
                
                if (dynamicElements[i] == null || dynamicElements[i].elementName.Length > 0) {
                    elementsToRemove.Add(dynamicElements[i]);
                    continue;
                }

                if (dynamicElements[i] is IResponsiveBreakpoints responsiveBreakpoints) {
                        responsiveBreakpoints.sceneAspectRatio = sceneAspectRatio;
                        responsiveBreakpoints.sceneHeight = sceneHeight;
                        responsiveBreakpoints.sceneWidth = sceneWidth;
                }
                dynamicElements[i].CallExecuteLayoutUpdate(this.gameObject);
            }
            
            for (int i = 0; i < elementsToRemove.Count; i++) {
                dynamicElements.Remove(elementsToRemove[i]);
            }
            elementsToRemove.Clear();
            
            
            // Lastly, execute priority elements with priorities less than 0
            for (int i=0; i<priorityDynamicElements.Count; i++) {
                
                if (priorityDynamicElements[i] == null || priorityDynamicElements[i].elementName.Length < 0) {
                    elementsToRemove.Add(priorityDynamicElements[i]);
                    continue;
                }

                if (priorityDynamicElements[i].priority <= 0) {
                    if (priorityDynamicElements[i] is IResponsiveBreakpoints responsiveBreakpoints) {
                        responsiveBreakpoints.sceneAspectRatio = sceneAspectRatio;
                        responsiveBreakpoints.sceneHeight = sceneHeight;
                        responsiveBreakpoints.sceneWidth = sceneWidth;
                    }
                    priorityDynamicElements[i].CallExecuteLayoutUpdate(this.gameObject);
                }
            }
            
            for (int i = 0; i < elementsToRemove.Count; i++) {
                dynamicElements.Remove(elementsToRemove[i]);
            }
            elementsToRemove.Clear();
#if UNITY_EDITOR
            if(TimelineEditor.inspectedDirector != null) {
                TimelineEditor.Refresh(RefreshReason.ContentsModified);
            }
#endif
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
            public AspectRatioType targetAspectRatio;

            private ValueDropdownList<AspectRatioType> aspectRatioValues = new ValueDropdownList<AspectRatioType>(){
                {"9 x 16 | 16 x 9", AspectRatioType.x9x16  },
                {"3 x 4 | 4 x 3", AspectRatioType.x3x4 },
                {"None | Resize", AspectRatioType.Dynamic }
            };

            [ValueDropdown(nameof(resizeTypeValues))]
            [SerializeField]
            [HideIf(nameof(TargetAspectRatioIsDynamic))]
            public ResizeType resizeType;

            private ValueDropdownList<ResizeType> resizeTypeValues = new ValueDropdownList<ResizeType>(){
                {"Vertical : Crop or box top and bottom", ResizeType.VerticalPillarBox  },
                {"Vertical : Crop or box sides", ResizeType.VerticalLetterBox },
                {"Horizontal: Crop or box top and bottom", ResizeType.HorizontalPillarBox},
                {"Horizontal : Crop or box sides", ResizeType.HorizontalLetterBox  }
            };

            private bool TargetAspectRatioIsDynamic()
            {
                if(targetAspectRatio == AspectRatioType.Dynamic) {
                    return true;
                }
                return false;
            }
        }

        public enum ResizeType { VerticalPillarBox, HorizontalLetterBox, VerticalLetterBox, HorizontalPillarBox }


        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

        private static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }
}