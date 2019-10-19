using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor.Timeline;
#endif

namespace AltSalt {

    [ExecuteInEditMode]
	public class PrepareScene : MonoBehaviour {

        [SerializeField]
        bool defaultX;
        [SerializeField]
        bool defaultY;
        [SerializeField]
        bool defaultZ;

        [SerializeField]
        bool invertYAxis;
        [SerializeField]
        bool invertXAxis;

        [SerializeField]
        Axis xSwipeAxis;
        [SerializeField]
        Axis ySwipeAxis;
        [SerializeField]
        Axis zSwipeAxis;

        [SerializeField]
        Axis xMomentumAxis;
        [SerializeField]
        Axis yMomentumAxis;
        [SerializeField]
        Axis zMomentumAxis;

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        BoolReference _invertYAxis;

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        BoolReference _invertXAxis;

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        BoolReference isReversing;

        [ValueDropdown(nameof(orientationValues))]
        [SerializeField]
        DimensionType orientation;

        [SerializeField]
        bool resetSequences;

//        [ShowIf(nameof(resetSequences))]
//        [SerializeField]
//        SequenceCollection _sequenceCollection;

        [SerializeField]
        bool delayStart;

        [SerializeField]
        [ShowIf(nameof(delayStart))]
        float delayAmount;

        [SerializeField]
        [Required]
        SimpleEventTrigger prepareSceneCompleted;

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        BoolReference removeOverlayImmediately;

        [SerializeField]
        [Required]
        SimpleEventTrigger fadeInTriggered;

        [ValidateInput(nameof(IsPopulated))]
        [SerializeField]
        FloatReference deviceAspectRatio;

        [ValidateInput(nameof(IsPopulated))]
        [SerializeField]
        FloatReference deviceWidth;

        [ValidateInput(nameof(IsPopulated))]
        [SerializeField]
        FloatReference deviceHeight;

        [ValidateInput(nameof(IsPopulated))]
        [SerializeField]
        FloatReference sceneWidth = new FloatReference();

        [ValidateInput(nameof(IsPopulated))]
        [SerializeField]
        FloatReference sceneHeight = new FloatReference();

        [ValidateInput(nameof(IsPopulated))]
        [SerializeField]
        FloatReference sceneAspectRatio = new FloatReference();

        [SerializeField]
        List<float> deviceBreakpoints = new List<float>();

        [SerializeField]
        List<SceneDimension> sceneDimensions = new List<SceneDimension>();

        List<IResponsive> priorityResponsiveElements = new List<IResponsive>();
        List<IResponsive> responsiveElements = new List<IResponsive>();

        private ValueDropdownList<DimensionType> orientationValues = new ValueDropdownList<DimensionType>(){
            {"Vertical", DimensionType.Vertical },
            {"Horizontal", DimensionType.Horizontal }
        };

        void Start() {
            ResetScene();
		}

        void OnDisable()
        {
            priorityResponsiveElements.Clear();
            responsiveElements.Clear();
        }

        [HorizontalGroup("Split", 1f)]
        [InfoBox("Reset scene variables")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void ResetScene()
        {
            xSwipeAxis.active = defaultX;
            ySwipeAxis.active = defaultY;
            zSwipeAxis.active = defaultZ;

            xMomentumAxis.active = defaultX;
            yMomentumAxis.active = defaultY;
            zMomentumAxis.active = defaultZ;

            _invertYAxis.Variable.Value = invertYAxis;
            _invertXAxis.Variable.Value = invertXAxis;

            isReversing.Variable.Value = false;

            Time.timeScale = 1.0f;

            if(orientation == DimensionType.Vertical) {
                Screen.orientation = ScreenOrientation.Portrait;
            } else {
                Screen.orientation = ScreenOrientation.LandscapeLeft;
            }

            if(resetSequences == true) {
                TriggerResetSequences();
            }

            if (removeOverlayImmediately.Value == true) {
                fadeInTriggered.RaiseEvent(this.gameObject);
            }

            CallExecuteLayoutUpdate();

            if(delayStart == false) {
                prepareSceneCompleted.RaiseEvent(this.gameObject);
            } else {
                StartCoroutine(PrepareSceneCompleteTimedDelay());
            }
        }

        IEnumerator PrepareSceneCompleteTimedDelay()
        {
            yield return new WaitForSeconds(delayAmount);
            prepareSceneCompleted.RaiseEvent(this.gameObject);
        }

        void TriggerResetSequences()
        {
//            if (_sequenceCollection == null) {
//                Debug.LogWarning("No sequence list found on " + this.name + ", please check.", this);
//                return;
//            }
//
//            for (int i = 0; i < _sequenceCollection.sequences.Count; i++) {
//                _sequenceCollection.sequences[i].SetDefaults();
//            }
        }

        public void AddResponsiveElement(EventPayload eventPayload)
        {
            IResponsive element = eventPayload.objectDictionary[DataType.systemObjectType] as IResponsive;
            
            if(element.ParentScene == this.gameObject.scene &&
                priorityResponsiveElements.Contains(element) == false && responsiveElements.Contains(element) == false) {

                if(element.Priority > 0) {
                    priorityResponsiveElements.Add(element);
                } else {
                    responsiveElements.Add(element);
                }
            }
        }

        public void RemoveResponsiveElement(EventPayload eventPayload)
        {
            IResponsive element = eventPayload.objectDictionary[DataType.systemObjectType] as IResponsive;
            if(priorityResponsiveElements.Contains(element) == true) {
                priorityResponsiveElements.Remove(element);
            }
            if (responsiveElements.Contains(element) == true) {
                responsiveElements.Remove(element);
            }
        }

        public void CallExecuteLayoutUpdate()
        {
            PopulateSceneDimensions();

            // We track all priority responsive elements separately
            // because sorting is an expensive operation
            priorityResponsiveElements.Sort(new ResponsiveUtilsCore.ResponsiveElementSort());

            // Start with lowest priority and end with highest priority
            for(int i= priorityResponsiveElements.Count - 1; i >= 0; i--) {
                if(priorityResponsiveElements[i] != null && priorityResponsiveElements[i].Name.Length > 0) {
                    priorityResponsiveElements[i].CallExecuteLayoutUpdate(this.gameObject);
                } else {
                    priorityResponsiveElements.Remove(priorityResponsiveElements[i]);
                }
            }

            // All other elements can be executed in any order
            for (int i=0; i<responsiveElements.Count; i++) {
                if (responsiveElements[i] != null && responsiveElements[i].Name.Length > 0) {
                    responsiveElements[i].CallExecuteLayoutUpdate(this.gameObject);
                } else {
                    responsiveElements.Remove(responsiveElements[i]);
                }
            }
#if UNITY_EDITOR
            if(TimelineEditor.inspectedDirector != null) {
                TimelineEditor.Refresh(RefreshReason.ContentsModified);
            }
#endif
        }

        void PopulateSceneDimensions()
        {
            if(deviceBreakpoints.Count > 0) {
                int breakpointIndex = Utils.GetValueIndexInList(deviceAspectRatio.Value, deviceBreakpoints);
                SceneDimension sceneDimension = sceneDimensions[breakpointIndex];

                switch (sceneDimension.targetAspectRatio) {

                    case AspectRatioType.x9x16:
                        if(sceneDimension.resizeType == ResizeType.VerticalPillarBox || sceneDimension.resizeType == ResizeType.HorizontalLetterBox) {
                            sceneWidth.Variable.SetValue(deviceWidth.Value);
                            sceneHeight.Variable.SetValue((16f * sceneWidth.Value) / 9f);
                        } else {
                            sceneHeight.Variable.SetValue(deviceHeight.Value);
                            sceneWidth.Variable.SetValue((9f * sceneHeight.Value) / 16f);
                        }
                        break;

                    case AspectRatioType.x3x4:
                        if (sceneDimension.resizeType == ResizeType.VerticalPillarBox || sceneDimension.resizeType == ResizeType.HorizontalLetterBox) {
                            sceneWidth.Variable.SetValue(deviceWidth.Value);
                            sceneHeight.Variable.SetValue((4f * sceneWidth.Value) / 3f);
                        } else {
                            sceneHeight.Variable.SetValue(deviceHeight.Value);
                            sceneWidth.Variable.SetValue((3f * sceneHeight.Value) / 4f);
                        }
                        break;

                    case AspectRatioType.Dynamic:
                        sceneWidth.Variable.SetValue(deviceWidth.Value);
                        sceneHeight.Variable.SetValue(deviceHeight.Value);
                        break;

                    default:
                        Debug.Log("Specified aspect ratio " + nameof(sceneDimension.targetAspectRatio) + " is not supported.");
                        break;
                }

                sceneAspectRatio.Variable.SetValue(sceneHeight.Value / sceneWidth.Value);

            } else {

                sceneWidth.Variable.SetValue(deviceWidth.Value);
                sceneHeight.Variable.SetValue(deviceHeight.Value);
                sceneAspectRatio.Variable.SetValue(deviceHeight.Value / deviceWidth.Value);

            }
        }

        [Serializable]
        class SceneDimension
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

            bool TargetAspectRatioIsDynamic()
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
