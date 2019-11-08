﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using TMPro;
using UnityEngine.Timeline;
using UnityEditor.Timeline;

namespace AltSalt.Maestro
{
    public class EditColorVarClip : ChildUIElementsWindow
    {
        static PageBuilderWindow pageBuilderWindow;
        static Foldout elementUXML;

        public override ChildUIElementsWindow Init(EditorWindow parentWindow)
        {
            pageBuilderWindow = parentWindow as PageBuilderWindow;
            VisualElement parentVisualElement = parentWindow.rootVisualElement;

            elementUXML = parentVisualElement.Query<Foldout>("EditColorVarClip", EditorToolsCore.ToggleableGroup);

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

        public Color initialValue = new Color(0, 0, 0, 1);
        public Color targetValue = new Color(0, 0, 0, 1);

        static bool populateButtonPressed = false;

        enum PropertyFieldNames
        {
            InitialValue,
            TargetValue
        }

        enum ButtonNames
        {
            PopulateInitialValueFromSelection,
            PopulateInitialValueFromTarget,
            SetInitialValue,
            SetInitialTransparent,
            SetInitialOpaque,
            PopulateTargetValueFromSelection,
            PopulateTargetValueFromInit,
            SetTargetValue,
            SetTargetTransparent,
            SetTargetOpaque
        }

        void UpdateDisplay()
        {
            bool dependencySelected = false;

            for (int i = 0; i < TimelineEditor.selectedClips.Length; i++) {
                if (TimelineEditor.selectedClips[i].asset is LerpColorVarClip) {
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
                    propertyField.RegisterCallback<ChangeEvent<Color>>((ChangeEvent<Color> evt) => {
                        if(populateButtonPressed == false) {
                            SetInitialValue(TimelineEditor.selectedClips, initialValue);
                            TimelineUtilsCore.RefreshTimelineContentsModified();
                            TimelineUtilsCore.RefreshTimelineSceneRedraw();
                        }
                        populateButtonPressed = false;
                    });
                    break;

                case nameof(PropertyFieldNames.TargetValue):
                    propertyField.RegisterCallback<ChangeEvent<Color>>((ChangeEvent<Color> evt) => {
                        if(populateButtonPressed == false) {
                            SetTargetValue(TimelineEditor.selectedClips, targetValue);
                            TimelineUtilsCore.RefreshTimelineContentsModified();
                            TimelineUtilsCore.RefreshTimelineSceneRedraw();
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

                case nameof(ButtonNames.SetInitialTransparent):
                    button.clickable.clicked += () => {
                        SetInitialAlpha(TimelineEditor.selectedClips, 0);
                        TimelineUtilsCore.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.SetInitialOpaque):
                    button.clickable.clicked += () => {
                        SetInitialAlpha(TimelineEditor.selectedClips, 1);
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

                case nameof(ButtonNames.SetTargetTransparent):
                    button.clickable.clicked += () => {
                        SetTargetAlpha(TimelineEditor.selectedClips, 0);
                        TimelineUtilsCore.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.SetTargetOpaque):
                    button.clickable.clicked += () => {
                        SetTargetAlpha(TimelineEditor.selectedClips, 1);
                        TimelineUtilsCore.RefreshTimelineContentsModified();
                    };
                    break;
            }

            return button;
        }

        public static Color GetInitialValueFromSelection(TimelineClip[] clipSelection)
        {
            Color value = new Color(0, 0, 0, 0);
            Array.Sort(clipSelection, new Utils.ClipTimeSort());

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].asset is LerpColorVarClip) {
                    value = (clipSelection[i].asset as LerpColorVarClip).template.initialColor;
                    break;
                }
            }

            return value;
        }

        public static TimelineClip[] SetInitialValue(TimelineClip[] clipSelection, Color targetValue)
        {
            List<TimelineClip> changedClips = new List<TimelineClip>();

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].asset is LerpColorVarClip) {
                    TimelineClip clip = clipSelection[i];
                    changedClips.Add(clip);
                    LerpColorVarClip clipAsset = clipSelection[i].asset as LerpColorVarClip;
                    Undo.RecordObject(clipAsset, "set clip(s) initial color");
                    clipAsset.template.initialColor = targetValue;
                }
            }

            return changedClips.ToArray();
        }

        public static TimelineClip[] SetInitialAlpha(TimelineClip[] clipSelection, float targetValue)
        {
            List<TimelineClip> changedClips = new List<TimelineClip>();

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].asset is LerpColorVarClip) {
                    TimelineClip clip = clipSelection[i];
                    changedClips.Add(clip);
                    LerpColorVarClip clipAsset = clipSelection[i].asset as LerpColorVarClip;
                    Undo.RecordObject(clipAsset, "set clip(s) initial color");
                    Color originalColor = clipAsset.template.initialColor;
                    clipAsset.template.initialColor = new Color(originalColor.r, originalColor.g, originalColor.b, targetValue);
                }
            }

            return changedClips.ToArray();
        }

        public static Color GetTargetValueFromSelection(TimelineClip[] clipSelection)
        {
            Color value = new Color(0, 0, 0, 0);
            Array.Sort(clipSelection, new Utils.ClipTimeSort());

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].asset is LerpColorVarClip) {
                    value = (clipSelection[i].asset as LerpColorVarClip).template.targetColor;
                    break;
                }
            }

            return value;
        }

        public static TimelineClip[] SetTargetValue(TimelineClip[] clipSelection, Color targetValue)
        {
            List<TimelineClip> changedClips = new List<TimelineClip>();

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].asset is LerpColorVarClip) {
                    TimelineClip clip = clipSelection[i];
                    changedClips.Add(clip);
                    LerpColorVarClip clipAsset = clipSelection[i].asset as LerpColorVarClip;
                    Undo.RecordObject(clipAsset, "set clip(s) target color");
                    clipAsset.template.targetColor = targetValue;
                }
            }

            return changedClips.ToArray();
        }

        public static TimelineClip[] SetTargetAlpha(TimelineClip[] clipSelection, float targetValue)
        {
            List<TimelineClip> changedClips = new List<TimelineClip>();

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].asset is LerpColorVarClip) {
                    TimelineClip clip = clipSelection[i];
                    changedClips.Add(clip);
                    LerpColorVarClip clipAsset = clipSelection[i].asset as LerpColorVarClip;
                    Undo.RecordObject(clipAsset, "set clip(s) target color");
                    Color originalColor = clipAsset.template.targetColor;
                    clipAsset.template.targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, targetValue);
                }
            }

            return changedClips.ToArray();
        }
    }
}