using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine.Timeline;
using UnityEditor.Timeline;
using UnityEngine.Playables;

namespace AltSalt.Maestro
{
    public class EditRectTransformResponsivePosClip : ChildModuleWindow
    {
        static FloatField currentAspectRatio;

        protected override ChildModuleWindow Configure(ParentModuleWindow parentModuleWindow, string childRootUXMLName)
        {
            base.Configure(parentModuleWindow, childRootUXMLName);

            moduleChildUXML = parentModuleWindow.moduleWindowUXML.Query<Foldout>("EditRectTransformResponsivePosClip", EditorToolsCore.ToggleableGroup);

            var propertyFields = moduleChildUXML.Query<PropertyField>();
            propertyFields.ForEach(SetupPropertyField);

            var buttons = moduleChildUXML.Query<Button>();
            buttons.ForEach(SetupButton);

            currentAspectRatio = moduleChildUXML.Query<FloatField>("CurrentAspectRatio");
            ControlPanel.inspectorUpdateDelegate += UpdateBreakpoint;

            UpdateDisplay();
            ControlPanel.selectionChangedDelegate += UpdateDisplay;

            return this;
        }

        void OnDestroy()
        {
            ControlPanel.inspectorUpdateDelegate -= UpdateBreakpoint;
            ControlPanel.selectionChangedDelegate -= UpdateDisplay;
        }

        static VisualElementToggleData toggleData = new VisualElementToggleData();

        public Vector3 initialPos = new Vector3(0, 0, 0);
        public Vector3 transposeInitialPos = new Vector3(0, 0, 0);
        public Vector3 initialPosInterval = new Vector3(0, 0, 0);
        public bool setInitIntervalOnValueChange = true;

        public Vector3 targetPos = new Vector3(0, 0, 0);
        public Vector3 transposeTargetPos = new Vector3(0, 0, 0);
        public Vector3 targetPosInterval = new Vector3(0, 0, 0);
        public bool setTargetIntervalOnValueChange = true;

        static bool populateButtonPressed = false;

        enum PropertyFieldNames
        {
            InitialPos,
            TransposeInitialPos,
            InitialPosInterval,
            SetInitIntervalOnValueChange,
            TargetPos,
            TransposeTargetPos,
            TargetPosInterval,
            SetTargetIntervalOnValueChange
        }

        enum ButtonNames
        {
            PopulateInitialPosFromSelection,
            PopulateInitialPosFromTarget,
            SetInitialPos,
            SetInitPosUsingInterval,
            PopulateTargetPosFromSelection,
            PopulateTargetPosFromInit,
            SetTargetPos,
            SetTargetPosUsingInterval
        }

        enum EnableCondition
        {
            MultipleClipsSelected
        }

        void UpdateBreakpoint()
        {
            if(currentAspectRatio != null) {
                currentAspectRatio.value = ModuleUtils.sceneAspectRatio;
            }
        }

        void UpdateDisplay()
        {
            bool dependencySelected = false;

            for (int i = 0; i < TimelineEditor.selectedClips.Length; i++) {
                if (TimelineEditor.selectedClips[i].parentTrack is ResponsiveRectTransformPosTrack) {
                    dependencySelected = true;
                    break;
                }
            }

            moduleChildUXML.SetEnabled(dependencySelected);
            moduleChildUXML.value = dependencySelected;

            if (TimelineEditor.selectedClips.Length > 1) {
                EditorToolsCore.ToggleVisualElements(toggleData, EnableCondition.MultipleClipsSelected, true);
            } else {
                EditorToolsCore.ToggleVisualElements(toggleData, EnableCondition.MultipleClipsSelected, false);
            }
        }

        PropertyField SetupPropertyField(PropertyField propertyField)
        {
            switch (propertyField.name) {

                case nameof(PropertyFieldNames.InitialPos):
                    propertyField.RegisterCallback<ChangeEvent<Vector3>>((ChangeEvent<Vector3> evt) => {
                        if (populateButtonPressed == false) {
                            SetInitialPosition(TimelineEditor.selectedClips, initialPos);
                            TimelineUtils.RefreshTimelineContentsModified();
                        }
                        populateButtonPressed = false;
                    });
                    break;

                case nameof(PropertyFieldNames.TransposeInitialPos):
                    propertyField.RegisterCallback<ChangeEvent<Vector3>>((ChangeEvent<Vector3> evt) => {
                        TransposeInitialPosition(TimelineEditor.selectedClips, transposeInitialPos);
                        transposeInitialPos = new Vector3(0, 0, 0);
                        TimelineUtils.RefreshTimelineContentsModified();
                    });
                    break;

                case nameof(PropertyFieldNames.SetInitIntervalOnValueChange):
                    EditorToolsCore.AddToVisualElementToggleData(toggleData, EnableCondition.MultipleClipsSelected, propertyField);
                    break;

                case nameof(PropertyFieldNames.InitialPosInterval):
                    propertyField.RegisterCallback<ChangeEvent<Vector3>>((ChangeEvent<Vector3> evt) => {
                        if (setInitIntervalOnValueChange == true) {
                            SetInitPosUsingInterval(TimelineEditor.selectedClips, initialPos, initialPosInterval);
                            TimelineUtils.RefreshTimelineContentsModified();
                        }
                    });
                    break;

                case nameof(PropertyFieldNames.TargetPos):
                    propertyField.RegisterCallback<ChangeEvent<Vector3>>((ChangeEvent<Vector3> evt) => {
                        if (populateButtonPressed == false) {
                            SetTargetPosition(TimelineEditor.selectedClips, targetPos);
                            TimelineUtils.RefreshTimelineContentsModified();
                        }
                        populateButtonPressed = false;
                    });
                    break;

                case nameof(PropertyFieldNames.TransposeTargetPos):
                    propertyField.RegisterCallback<ChangeEvent<Vector3>>((ChangeEvent<Vector3> evt) => {
                        TransposeTargetPosition(TimelineEditor.selectedClips, transposeTargetPos);
                        transposeTargetPos = new Vector3(0, 0, 0);
                        TimelineUtils.RefreshTimelineContentsModified();
                    });
                    break;

                case nameof(PropertyFieldNames.SetTargetIntervalOnValueChange):
                    EditorToolsCore.AddToVisualElementToggleData(toggleData, EnableCondition.MultipleClipsSelected, propertyField);
                    break;

                case nameof(PropertyFieldNames.TargetPosInterval):
                    propertyField.RegisterCallback<ChangeEvent<Vector3>>((ChangeEvent<Vector3> evt) => {
                        if (setTargetIntervalOnValueChange == true) {
                            SetTargetPosUsingInterval(TimelineEditor.selectedClips, targetPos, targetPosInterval);
                            TimelineUtils.RefreshTimelineContentsModified();
                        }
                    });
                    EditorToolsCore.AddToVisualElementToggleData(toggleData, EnableCondition.MultipleClipsSelected, propertyField);
                    break;
            }

            return propertyField;
        }

        Button SetupButton(Button button)
        {
            switch (button.name) {

                case nameof(ButtonNames.PopulateInitialPosFromSelection):
                    button.clickable.clicked += () => {
                        populateButtonPressed = true;
                        initialPos = GetInitialPosFromSelection(TimelineEditor.selectedClips);
                    };
                    break;

                case nameof(ButtonNames.PopulateInitialPosFromTarget):
                    button.clickable.clicked += () => {
                        populateButtonPressed = true;
                        initialPos = targetPos;
                    };
                    break;

                case nameof(ButtonNames.SetInitialPos):
                    button.clickable.clicked += () => {
                        SetInitialPosition(TimelineEditor.selectedClips, initialPos);
                        TimelineUtils.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.SetInitPosUsingInterval):
                    button.clickable.clicked += () => {
                        SetInitPosUsingInterval(TimelineEditor.selectedClips, initialPos, initialPosInterval);
                        TimelineUtils.RefreshTimelineContentsModified();
                    };
                    EditorToolsCore.AddToVisualElementToggleData(toggleData, EnableCondition.MultipleClipsSelected, button);
                    break;

                case nameof(ButtonNames.PopulateTargetPosFromSelection):
                    button.clickable.clicked += () => {
                        populateButtonPressed = true;
                        targetPos = GetTargetPosFromSelection(TimelineEditor.selectedClips);
                    };
                    break;

                case nameof(ButtonNames.PopulateTargetPosFromInit):
                    button.clickable.clicked += () => {
                        populateButtonPressed = true;
                        targetPos = initialPos;
                    };
                    break;

                case nameof(ButtonNames.SetTargetPos):
                    button.clickable.clicked += () => {
                        SetTargetPosition(TimelineEditor.selectedClips, targetPos);
                        TimelineUtils.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.SetTargetPosUsingInterval):
                    button.clickable.clicked += () => {
                        SetTargetPosUsingInterval(TimelineEditor.selectedClips, targetPos, targetPosInterval);
                        TimelineUtils.RefreshTimelineContentsModified();
                    };
                    EditorToolsCore.AddToVisualElementToggleData(toggleData, EnableCondition.MultipleClipsSelected, button);
                    break;
            }

            return button;
        }

        public static Vector3 GetInitialPosFromSelection(TimelineClip[] clipSelection)
        {
            Vector3 value = new Vector3(0, 0, 0);
            Array.Sort(clipSelection, new Utils.ClipTimeSort());

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].parentTrack is ResponsiveRectTransformPosTrack) {
                    value = (clipSelection[i].asset as ResponsiveVector3Clip).template.GetInitialValueAtBreakpoint(currentAspectRatio.value);
                    break;
                }
            }

            return value;
        }

        public static TimelineClip[] SetInitialPosition(TimelineClip[] clipSelection, Vector3 targetValue)
        {
            List<TimelineClip> changedClips = new List<TimelineClip>();

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].parentTrack is ResponsiveRectTransformPosTrack) {
                    TimelineClip clip = clipSelection[i];
                    changedClips.Add(clip);
                    ResponsiveVector3Clip clipAsset = clipSelection[i].asset as ResponsiveVector3Clip;
                    Undo.RecordObject(clipAsset, "set clip(s) initial position");
                    clipAsset.template.SaveNewInitialValue(targetValue);
                }
            }

            return changedClips.ToArray();
        }

        public static TimelineClip[] SetInitPosUsingInterval(TimelineClip[] clipSelection, Vector3 sourceValue, Vector3 interval)
        {
            List<TimelineClip> changedClips = new List<TimelineClip>();
            Array.Sort(clipSelection, new Utils.ClipTimeSort());
            Vector3 newValue = sourceValue;

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].parentTrack is ResponsiveRectTransformPosTrack) {
                    TimelineClip clip = clipSelection[i];
                    changedClips.Add(clip);
                    ResponsiveVector3Clip clipAsset = clipSelection[i].asset as ResponsiveVector3Clip;
                    Undo.RecordObject(clipAsset, "set clip(s) initial interval position");
                    clipAsset.template.SaveNewInitialValue(sourceValue);
                    sourceValue += interval;
                }
            }

            return changedClips.ToArray();
        }

        public static TimelineClip[] TransposeInitialPosition(TimelineClip[] clipSelection, Vector3 transposeValue)
        {
            List<TimelineClip> changedClips = new List<TimelineClip>();

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].parentTrack is ResponsiveRectTransformPosTrack) {
                    changedClips.Add(clipSelection[i]);
                    ResponsiveVector3Clip clipAsset = clipSelection[i].asset as ResponsiveVector3Clip;
                    Undo.RecordObject(clipAsset, "transpose clip(s) initial position");
                    Vector3 originalPosition = clipAsset.template.GetInitialValueAtBreakpoint(currentAspectRatio.value);
                    clipAsset.template.SaveNewInitialValue(new Vector3(originalPosition.x + transposeValue.x, originalPosition.y + transposeValue.y, originalPosition.z + transposeValue.z));
                }
            }

            return changedClips.ToArray();
        }

        public static Vector3 GetTargetPosFromSelection(TimelineClip[] clipSelection)
        {
            Vector3 value = new Vector3(0, 0, 0);
            Array.Sort(clipSelection, new Utils.ClipTimeSort());

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].parentTrack is ResponsiveRectTransformPosTrack) {
                    value = (clipSelection[i].asset as ResponsiveVector3Clip).template.GetTargetValueAtBreakpoint(currentAspectRatio.value);
                    break;
                }
            }

            return value;
        }

        public static TimelineClip[] SetTargetPosition(TimelineClip[] clipSelection, Vector3 targetValue)
        {
            List<TimelineClip> changedClips = new List<TimelineClip>();

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].parentTrack is ResponsiveRectTransformPosTrack) {
                    TimelineClip clip = clipSelection[i];
                    changedClips.Add(clip);
                    ResponsiveVector3Clip clipAsset = clipSelection[i].asset as ResponsiveVector3Clip;
                    Undo.RecordObject(clipAsset, "set clip(s) target position");
                    clipAsset.template.SaveNewTargetValue(targetValue);
                }
            }

            return changedClips.ToArray();
        }

        public static TimelineClip[] TransposeTargetPosition(TimelineClip[] clipSelection, Vector3 transposeValue)
        {
            List<TimelineClip> changedClips = new List<TimelineClip>();

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].parentTrack is ResponsiveRectTransformPosTrack) {
                    changedClips.Add(clipSelection[i]);
                    ResponsiveVector3Clip clipAsset = clipSelection[i].asset as ResponsiveVector3Clip;
                    Undo.RecordObject(clipAsset, "transpose clip(s) target position");
                    Vector3 originalPosition = clipAsset.template.GetTargetValueAtBreakpoint(currentAspectRatio.value);
                    clipAsset.template.SaveNewTargetValue(new Vector3(originalPosition.x + transposeValue.x, originalPosition.y + transposeValue.y, originalPosition.z + transposeValue.z));
                }
            }

            return changedClips.ToArray();
        }

        public static TimelineClip[] SetTargetPosUsingInterval(TimelineClip[] clipSelection, Vector3 sourceValue, Vector3 interval)
        {
            List<TimelineClip> changedClips = new List<TimelineClip>();
            Array.Sort(clipSelection, new Utils.ClipTimeSort());
            Vector3 newValue = sourceValue;

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].parentTrack is ResponsiveRectTransformPosTrack) {
                    TimelineClip clip = clipSelection[i];
                    changedClips.Add(clip);
                    ResponsiveVector3Clip clipAsset = clipSelection[i].asset as ResponsiveVector3Clip;
                    Undo.RecordObject(clipAsset, "set clip(s) target interval position");
                    clipAsset.template.SaveNewTargetValue(sourceValue);
                    sourceValue += interval;
                }
            }

            return changedClips.ToArray();
        }
    }
}
