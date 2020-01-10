using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Events;

namespace AltSalt.Maestro
{
    public class DynamicLayoutControllerBackup : MonoBehaviour, ISkipRegistration
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

        private float deviceAspectRatio => appSettings.deviceAspectRatio;
        
        private float deviceWidth => appSettings.deviceWidth;

        private float deviceHeight => appSettings.deviceHeight;

        [field: ShowInInspector, ReadOnly]
        private float sceneAspectRatio { get; set; }

        [field: ShowInInspector, ReadOnly]
        private float sceneWidth { get; set; }

        [field: ShowInInspector, ReadOnly]
        private float sceneHeight { get; set; }

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
        private UnityEvent _layoutRenderCompleteEvents;

        private UnityEvent layoutRenderCompleteEvents => _layoutRenderCompleteEvents;

        private void Start()
        {
            RefreshLayout();
        }

        private void RefreshLayout()
        {
            CallExecuteLayoutUpdate();

            if(delayStart == false) {
                layoutRendered.RaiseEvent(this.gameObject);
                layoutRenderCompleteEvents.Invoke();
            } else {
                StartCoroutine(LayoutRenderCompleteTimeDelay());
            }
        }

        private void OnDisable()
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

            UpdateDynamicElements(priorityDynamicElements, this, 1);
            UpdateDynamicElements(dynamicElements, this, int.MinValue);
            UpdateDynamicElements(priorityDynamicElements, this, int.MinValue, 0);

#if UNITY_EDITOR
            if(TimelineEditor.inspectedDirector != null) {
                TimelineEditor.Refresh(RefreshReason.ContentsModified);
            }
#endif
        }

        private static List<IDynamicLayoutElement> UpdateDynamicElements(List<IDynamicLayoutElement> dynamicLayoutElements,
            DynamicLayoutControllerBackup dynamicLayoutController, int minPriority, int maxPriority = int.MaxValue)
        {
            // Used to remove any elements that might be null
            List<IDynamicLayoutElement> elementsToRemove = new List<IDynamicLayoutElement>();
            
            for (int i=0; i<dynamicLayoutElements.Count; i++) {
                
                if (dynamicLayoutElements[i] == null || dynamicLayoutElements[i].elementName.Length < 0) {
                    elementsToRemove.Add(dynamicLayoutElements[i]);
                    continue;
                }

                if (dynamicLayoutElements[i].priority >= minPriority && dynamicLayoutElements[i].priority <= maxPriority) {
                    if (dynamicLayoutElements[i] is IResponsiveBreakpoints responsiveBreakpoints) {
                        responsiveBreakpoints.sceneAspectRatio = dynamicLayoutController.sceneAspectRatio;
                        responsiveBreakpoints.sceneWidth = dynamicLayoutController.sceneWidth;
                        responsiveBreakpoints.sceneHeight = dynamicLayoutController.sceneHeight;
                    }
                    dynamicLayoutElements[i].CallExecuteLayoutUpdate(dynamicLayoutController.gameObject);
                }
            }
            
            // Sanitize the list after we have iterated through it
            // so we don't interfere with the loop during iteration
            for (int i = 0; i < elementsToRemove.Count; i++) {
                dynamicLayoutElements.Remove(elementsToRemove[i]);
            }

            return dynamicLayoutElements;
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
            [HideIf(nameof(AspectRatioIsDynamic))]
            public ResizeType resizeType;

            private ValueDropdownList<ResizeType> resizeTypeValues = new ValueDropdownList<ResizeType>(){
                {"Vertical : Crop or box top and bottom", ResizeType.VerticalPillarBox  },
                {"Vertical : Crop or box sides", ResizeType.VerticalLetterBox },
                {"Horizontal: Crop or box top and bottom", ResizeType.HorizontalPillarBox},
                {"Horizontal : Crop or box sides", ResizeType.HorizontalLetterBox  }
            };

            private bool AspectRatioIsDynamic()
            {
                if(targetAspectRatio == AspectRatioType.Dynamic) {
                    return true;
                }
                return false;
            }
        }

        public enum ResizeType { VerticalPillarBox, HorizontalLetterBox, VerticalLetterBox, HorizontalPillarBox }

    }
}