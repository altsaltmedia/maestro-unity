﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine.Timeline;
using UnityEditor.Timeline;

namespace AltSalt.Maestro.Animation
{
    public class EditRectTransformPosClip : ChildModuleWindow
    {
        protected override ChildModuleWindow Configure(ParentModuleWindow parentModuleWindow, string childRootUXMLName)
        {
            base.Configure(parentModuleWindow, childRootUXMLName);
            
            moduleChildUXML = parentModuleWindow.moduleWindowUXML.Query<Foldout>("EditRectTransformPosClip", ModuleUtils.toggleableGroup);

            var propertyFields = moduleChildUXML.Query<PropertyField>();
            propertyFields.ForEach(SetupPropertyField);

            var buttons = moduleChildUXML.Query<Button>();
            buttons.ForEach(SetupButton);

            UpdateDisplay();
            ControlPanel.selectionChangedDelegate += UpdateDisplay;

            return this;
        }

        void OnDestroy()
        {
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

        void UpdateDisplay()
        {
            bool dependencySelected = false;

            for (int i = 0; i < TimelineEditor.selectedClips.Length; i++) {
                if (TimelineEditor.selectedClips[i].asset is RectTransformPosClip) {
                    dependencySelected = true;
                    break;
                }
            }

            moduleChildUXML.SetEnabled(dependencySelected);
            moduleChildUXML.value = dependencySelected;

            if(TimelineEditor.selectedClips.Length > 1) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.MultipleClipsSelected, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.MultipleClipsSelected, false);
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
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.MultipleClipsSelected, propertyField);
                    break;

                case nameof(PropertyFieldNames.InitialPosInterval):
                    propertyField.RegisterCallback<ChangeEvent<Vector3>>((ChangeEvent<Vector3> evt) => {
                        if(setInitIntervalOnValueChange == true) {
                            SetInitPosUsingInterval(TimelineEditor.selectedClips, initialPos, initialPosInterval);
                            TimelineUtils.RefreshTimelineContentsModified();
                        }
                    });
                    break;

                case nameof(PropertyFieldNames.TargetPos):
                    propertyField.RegisterCallback<ChangeEvent<Vector3>>((ChangeEvent<Vector3> evt) => {
                        if(populateButtonPressed == false) {
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
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.MultipleClipsSelected, propertyField);
                    break;

                case nameof(PropertyFieldNames.TargetPosInterval):
                    propertyField.RegisterCallback<ChangeEvent<Vector3>>((ChangeEvent<Vector3> evt) => {
                        if(setTargetIntervalOnValueChange == true) {
                            SetTargetPosUsingInterval(TimelineEditor.selectedClips, targetPos, targetPosInterval);
                            TimelineUtils.RefreshTimelineContentsModified();
                        }
                    });
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.MultipleClipsSelected, propertyField);
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
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.MultipleClipsSelected, button);
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
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.MultipleClipsSelected, button);
                    break;
            }

            return button;
        }

        public static Vector3 GetInitialPosFromSelection(TimelineClip[] clipSelection)
        {
            Vector3 value = new Vector3(0,0,0);
            Array.Sort(clipSelection, new Utils.ClipTimeSort());

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].asset is RectTransformPosClip) {
                    value = (clipSelection[i].asset as RectTransformPosClip).template.initialValue;
                    break;
                }
            }

            return value;
        }

        public static TimelineClip[] SetInitialPosition(TimelineClip[] clipSelection, Vector3 targetValue)
        {
            List<TimelineClip> changedClips = new List<TimelineClip>();

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].asset is RectTransformPosClip) {
                    TimelineClip clip = clipSelection[i];
                    changedClips.Add(clip);
                    RectTransformPosClip clipAsset = clipSelection[i].asset as RectTransformPosClip;
                    Undo.RecordObject(clipAsset, "set clip(s) initial position");
                    clipAsset.template.initialValue = targetValue;
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
                if (clipSelection[i].asset is RectTransformPosClip) {
                    TimelineClip clip = clipSelection[i];
                    changedClips.Add(clip);
                    RectTransformPosClip clipAsset = clipSelection[i].asset as RectTransformPosClip;
                    Undo.RecordObject(clipAsset, "set clip(s) initial interval position");
                    clipAsset.template.initialValue = sourceValue;
                    sourceValue += interval;
                }
            }

            return changedClips.ToArray();
        }

        public static TimelineClip[] TransposeInitialPosition(TimelineClip[] clipSelection, Vector3 transposeValue)
        {
            List<TimelineClip> changedClips = new List<TimelineClip>();

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].asset is RectTransformPosClip) {
                    changedClips.Add(clipSelection[i]);
                    RectTransformPosClip clipAsset = clipSelection[i].asset as RectTransformPosClip;
                    Undo.RecordObject(clipAsset, "transpose clip(s) initial position");
                    Vector3 originalPosition = clipAsset.template.initialValue;
                    clipAsset.template.initialValue = new Vector3(originalPosition.x + transposeValue.x, originalPosition.y + transposeValue.y, originalPosition.z + transposeValue.z);
                }
            }

            return changedClips.ToArray();
        }

        public static Vector3 GetTargetPosFromSelection(TimelineClip[] clipSelection)
        {
            Vector3 value = new Vector3(0, 0, 0);
            Array.Sort(clipSelection, new Utils.ClipTimeSort());

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].asset is RectTransformPosClip) {
                    value = (clipSelection[i].asset as RectTransformPosClip).template.targetValue;
                    break;
                }
            }

            return value;
        }

        public static TimelineClip[] SetTargetPosition(TimelineClip[] clipSelection, Vector3 targetValue)
        {
            List<TimelineClip> changedClips = new List<TimelineClip>();

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].asset is RectTransformPosClip) {
                    TimelineClip clip = clipSelection[i];
                    changedClips.Add(clip);
                    RectTransformPosClip clipAsset = clipSelection[i].asset as RectTransformPosClip;
                    Undo.RecordObject(clipAsset, "set clip(s) target position");
                    clipAsset.template.targetValue = targetValue;
                }
            }

            return changedClips.ToArray();
        }

        public static TimelineClip[] TransposeTargetPosition(TimelineClip[] clipSelection, Vector3 transposeValue)
        {
            List<TimelineClip> changedClips = new List<TimelineClip>();

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].asset is RectTransformPosClip) {
                    changedClips.Add(clipSelection[i]);
                    RectTransformPosClip clipAsset = clipSelection[i].asset as RectTransformPosClip;
                    Undo.RecordObject(clipAsset, "transpose clip(s) target position");
                    Vector3 originalPosition = clipAsset.template.targetValue;
                    clipAsset.template.targetValue = new Vector3(originalPosition.x + transposeValue.x, originalPosition.y + transposeValue.y, originalPosition.z + transposeValue.z);
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
                if (clipSelection[i].asset is RectTransformPosClip) {
                    TimelineClip clip = clipSelection[i];
                    changedClips.Add(clip);
                    RectTransformPosClip clipAsset = clipSelection[i].asset as RectTransformPosClip;
                    Undo.RecordObject(clipAsset, "set clip(s) target interval position");
                    clipAsset.template.targetValue = sourceValue;
                    sourceValue += interval;
                }
            }

            return changedClips.ToArray();
        }
    }
}
