﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine.Timeline;
using UnityEditor.Timeline;

namespace AltSalt.Maestro
{
    public class EditRectTransformRotationClip : ChildUIElementsWindow
    {
        static PageBuilderWindow pageBuilderWindow;
        static Foldout elementUXML;

        public override ChildUIElementsWindow Init(EditorWindow parentWindow)
        {
            pageBuilderWindow = parentWindow as PageBuilderWindow;
            VisualElement parentVisualElement = parentWindow.rootVisualElement;

            elementUXML = parentVisualElement.Query<Foldout>("EditRectTransformRotationClip", EditorToolsCore.ToggleableGroup);

            var propertyFields = elementUXML.Query<PropertyField>();
            propertyFields.ForEach(SetupPropertyField);

            var buttons = elementUXML.Query<Button>();
            buttons.ForEach(SetupButton);

            UpdateDisplay();
            PageBuilderWindow.selectionChangedDelegate += UpdateDisplay;

            return this;
        }

        void OnDestroy()
        {
            PageBuilderWindow.selectionChangedDelegate -= UpdateDisplay;
        }

        public Vector3 initialValue = new Vector3(0, 0, 0);
        public Vector3 transposeInitialValue = new Vector3(0, 0, 0);

        public Vector3 targetValue = new Vector3(0, 0, 0);
        public Vector3 transposeTargetValue = new Vector3(0, 0, 0);

        static bool populateButtonPressed = false;

        enum PropertyFieldNames
        {
            InitialValue,
            TargetValue,
            TransposeInitialValue,
            TransposeTargetValue
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

        void UpdateDisplay()
        {
            bool dependencySelected = false;

            for (int i = 0; i < TimelineEditor.selectedClips.Length; i++) {
                if (TimelineEditor.selectedClips[i].asset is RectTransformRotationClip) {
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

                case nameof(PropertyFieldNames.TransposeInitialValue):
                    propertyField.RegisterCallback<ChangeEvent<Vector3>>((ChangeEvent<Vector3> evt) => {
                        TransposeInitialValue(TimelineEditor.selectedClips, transposeInitialValue);
                        transposeInitialValue = new Vector3(0, 0, 0);
                        TimelineUtilsCore.RefreshTimelineContentsModified();
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

                case nameof(PropertyFieldNames.TransposeTargetValue):
                    propertyField.RegisterCallback<ChangeEvent<Vector3>>((ChangeEvent<Vector3> evt) => {
                        TransposeTargetValue(TimelineEditor.selectedClips, transposeTargetValue);
                        transposeTargetValue = new Vector3(0, 0, 0);
                        TimelineUtilsCore.RefreshTimelineContentsModified();
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
                if (clipSelection[i].asset is RectTransformRotationClip) {
                    value = (clipSelection[i].asset as RectTransformRotationClip).template.initialRotation;
                    break;
                }
            }

            return value;
        }

        public static TimelineClip[] SetInitialValue(TimelineClip[] clipSelection, Vector3 targetValue)
        {
            List<TimelineClip> changedClips = new List<TimelineClip>();

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].asset is RectTransformRotationClip) {
                    TimelineClip clip = clipSelection[i];
                    changedClips.Add(clip);
                    RectTransformRotationClip clipAsset = clipSelection[i].asset as RectTransformRotationClip;
                    Undo.RecordObject(clipAsset, "set clip(s) initial rotation");
                    clipAsset.template.initialRotation = targetValue;
                }
            }

            return changedClips.ToArray();
        }

        public static Vector3 GetTargetValueFromSelection(TimelineClip[] clipSelection)
        {
            Vector3 value = new Vector3(0, 0, 0);
            Array.Sort(clipSelection, new Utils.ClipTimeSort());

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].asset is RectTransformRotationClip) {
                    value = (clipSelection[i].asset as RectTransformRotationClip).template.targetRotation;
                    break;
                }
            }

            return value;
        }

        public static TimelineClip[] SetTargetValue(TimelineClip[] clipSelection, Vector3 targetValue)
        {
            List<TimelineClip> changedClips = new List<TimelineClip>();

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].asset is RectTransformRotationClip) {
                    TimelineClip clip = clipSelection[i];
                    changedClips.Add(clip);
                    RectTransformRotationClip clipAsset = clipSelection[i].asset as RectTransformRotationClip;
                    Undo.RecordObject(clipAsset, "set clip(s) target rotation");
                    clipAsset.template.targetRotation = targetValue;
                }
            }

            return changedClips.ToArray();
        }


        public static TimelineClip[] TransposeInitialValue(TimelineClip[] clipSelection, Vector3 transposeValue)
        {
            List<TimelineClip> changedClips = new List<TimelineClip>();

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].asset is RectTransformRotationClip) {
                    changedClips.Add(clipSelection[i]);
                    RectTransformRotationClip clipAsset = clipSelection[i].asset as RectTransformRotationClip;
                    Undo.RecordObject(clipAsset, "transpose clip(s) initial rotation");
                    Vector3 originalValue = clipAsset.template.initialRotation;
                    clipAsset.template.initialRotation = new Vector3(originalValue.x + transposeValue.x, originalValue.y + transposeValue.y, originalValue.z + transposeValue.z);
                }
            }

            return changedClips.ToArray();
        }

        public static TimelineClip[] TransposeTargetValue(TimelineClip[] clipSelection, Vector3 transposeValue)
        {
            List<TimelineClip> changedClips = new List<TimelineClip>();

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].asset is RectTransformRotationClip) {
                    changedClips.Add(clipSelection[i]);
                    RectTransformRotationClip clipAsset = clipSelection[i].asset as RectTransformRotationClip;
                    Undo.RecordObject(clipAsset, "transpose clip(s) target rotation");
                    Vector3 originalValue = clipAsset.template.targetRotation;
                    clipAsset.template.targetRotation = new Vector3(originalValue.x + transposeValue.x, originalValue.y + transposeValue.y, originalValue.z + transposeValue.z);
                }
            }

            return changedClips.ToArray();
        }
    }
}