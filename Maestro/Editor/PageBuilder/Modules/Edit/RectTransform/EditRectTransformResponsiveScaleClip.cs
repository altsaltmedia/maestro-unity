﻿using System;
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
    public class EditRectTransformResponsiveScaleClip : ChildUIElementsWindow
    {
        static PageBuilderWindow pageBuilderWindow;
        static Foldout elementUXML;
        static FloatField currentAspectRatio;

        public override ChildUIElementsWindow Init(EditorWindow parentWindow)
        {
            pageBuilderWindow = parentWindow as PageBuilderWindow;
            VisualElement parentVisualElement = parentWindow.rootVisualElement;

            elementUXML = parentVisualElement.Query<Foldout>("EditRectTransformResponsiveScaleClip", EditorToolsCore.ToggleableGroup);

            var propertyFields = elementUXML.Query<PropertyField>();
            propertyFields.ForEach(SetupPropertyField);

            var buttons = elementUXML.Query<Button>();
            buttons.ForEach(SetupButton);

            currentAspectRatio = elementUXML.Query<FloatField>("CurrentAspectRatio");
            PageBuilderWindow.inspectorUpdateDelegate += UpdateBreakpoint;

            UpdateDisplay();
            PageBuilderWindow.selectionChangedDelegate += UpdateDisplay;

            return this;
        }

        void OnDestroy()
        {
            PageBuilderWindow.inspectorUpdateDelegate -= UpdateBreakpoint;
            PageBuilderWindow.selectionChangedDelegate -= UpdateDisplay;
        }

        public Vector3 initialValue = new Vector3(1, 1, 1);
        public Vector3 targetValue = new Vector3(1, 1, 1);

        static bool populateButtonPressed = false;

        enum PropertyFieldNames
        {
            InitialValue,
            TargetValue,
        }

        enum ButtonNames
        {
            PopulateInitialValueFromSelection,
            PopulateInitialValueFromTarget,
            SetInitialValue,
            PopulateTargetValueFromSelection,
            PopulateTargetValueFromInit,
            SetTargetValue
        }

        void UpdateBreakpoint()
        {
            if (currentAspectRatio != null) {
                currentAspectRatio.value = PageBuilderCore.SceneAspectRatio;
            }
        }

        void UpdateDisplay()
        {
            bool dependencySelected = false;

            for (int i = 0; i < TimelineEditor.selectedClips.Length; i++) {
                if (TimelineEditor.selectedClips[i].parentTrack is ResponsiveRectTransformScaleTrack) {
                    dependencySelected = true;
                    break;
                }
            }

            elementUXML.SetEnabled(dependencySelected);
            elementUXML.value = dependencySelected;
        }

        PropertyField SetupPropertyField(PropertyField propertyField)
        {
            switch (propertyField.name) {

                case nameof(PropertyFieldNames.InitialValue):
                    propertyField.RegisterCallback<ChangeEvent<Vector3>>((ChangeEvent<Vector3> evt) => {
                        if (populateButtonPressed == false) {
                            SetInitialValue(TimelineEditor.selectedClips, initialValue);
                            TimelineUtilsCore.RefreshTimelineContentsModified();
                        }
                        populateButtonPressed = false;
                    });
                    break;

                case nameof(PropertyFieldNames.TargetValue):
                    propertyField.RegisterCallback<ChangeEvent<Vector3>>((ChangeEvent<Vector3> evt) => {
                        if (populateButtonPressed == false) {
                            SetTargetValue(TimelineEditor.selectedClips, targetValue);
                            TimelineUtilsCore.RefreshTimelineContentsModified();
                        }
                        populateButtonPressed = false;
                    });
                    break;

            }

            return propertyField;
        }

        Button SetupButton(Button button)
        {
            switch (button.name) {

                case nameof(ButtonNames.PopulateInitialValueFromSelection):
                    button.clickable.clicked += () => {
                        populateButtonPressed = true;
                        initialValue = GetInitialValueFromSelection(TimelineEditor.selectedClips);
                    };
                    break;

                case nameof(ButtonNames.PopulateInitialValueFromTarget):
                    button.clickable.clicked += () => {
                        populateButtonPressed = true;
                        initialValue = targetValue;
                    };
                    break;

                case nameof(ButtonNames.SetInitialValue):
                    button.clickable.clicked += () => {
                        SetInitialValue(TimelineEditor.selectedClips, initialValue);
                        TimelineUtilsCore.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.PopulateTargetValueFromSelection):
                    button.clickable.clicked += () => {
                        populateButtonPressed = true;
                        targetValue = GetTargetValueFromSelection(TimelineEditor.selectedClips);
                    };
                    break;

                case nameof(ButtonNames.PopulateTargetValueFromInit):
                    button.clickable.clicked += () => {
                        populateButtonPressed = true;
                        targetValue = initialValue;
                    };
                    break;

                case nameof(ButtonNames.SetTargetValue):
                    button.clickable.clicked += () => {
                        SetTargetValue(TimelineEditor.selectedClips, targetValue);
                        TimelineUtilsCore.RefreshTimelineContentsModified();
                    };
                    break;
            }

            return button;
        }

        public static Vector3 GetInitialValueFromSelection(TimelineClip[] clipSelection)
        {
            Vector3 value = new Vector3(0, 0, 0);
            Array.Sort(clipSelection, new Utils.ClipTimeSort());

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].parentTrack is ResponsiveRectTransformScaleTrack) {
                    value = (clipSelection[i].asset as ResponsiveVector3Clip).template.GetInitialValueAtBreakpoint(currentAspectRatio.value);
                    break;
                }
            }

            return value;
        }

        public static TimelineClip[] SetInitialValue(TimelineClip[] clipSelection, Vector3 targetValue)
        {
            List<TimelineClip> changedClips = new List<TimelineClip>();

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].parentTrack is ResponsiveRectTransformScaleTrack) {
                    TimelineClip clip = clipSelection[i];
                    changedClips.Add(clip);
                    ResponsiveVector3Clip clipAsset = clipSelection[i].asset as ResponsiveVector3Clip;
                    Undo.RecordObject(clipAsset, "set clip(s) initial scale");
                    clipAsset.template.SaveNewInitialValue(targetValue);
                }
            }

            return changedClips.ToArray();
        }

        public static Vector3 GetTargetValueFromSelection(TimelineClip[] clipSelection)
        {
            Vector3 value = new Vector3(0, 0, 0);
            Array.Sort(clipSelection, new Utils.ClipTimeSort());

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].parentTrack is ResponsiveRectTransformScaleTrack) {
                    value = (clipSelection[i].asset as ResponsiveVector3Clip).template.GetTargetValueAtBreakpoint(currentAspectRatio.value);
                    break;
                }
            }

            return value;
        }

        public static TimelineClip[] SetTargetValue(TimelineClip[] clipSelection, Vector3 targetValue)
        {
            List<TimelineClip> changedClips = new List<TimelineClip>();

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].parentTrack is ResponsiveRectTransformScaleTrack) {
                    TimelineClip clip = clipSelection[i];
                    changedClips.Add(clip);
                    ResponsiveVector3Clip clipAsset = clipSelection[i].asset as ResponsiveVector3Clip;
                    Undo.RecordObject(clipAsset, "set clip(s) target scale");
                    clipAsset.template.SaveNewTargetValue(targetValue);
                }
            }

            return changedClips.ToArray();
        }

    }
}