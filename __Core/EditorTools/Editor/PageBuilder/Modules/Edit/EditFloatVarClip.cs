using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine.Timeline;
using UnityEditor.Timeline;

namespace AltSalt
{
    public class EditFloatVarClip : ChildUIElementsWindow
    {
        static PageBuilderWindow pageBuilderWindow;
        static Foldout elementUXML;

        public override ChildUIElementsWindow Init(EditorWindow parentWindow)
        {
            pageBuilderWindow = parentWindow as PageBuilderWindow;
            VisualElement parentVisualElement = parentWindow.rootVisualElement;

            var propertyFields = parentVisualElement.Query<PropertyField>();
            propertyFields.ForEach(SetupPropertyField);

            var buttons = parentVisualElement.Query<Button>();
            buttons.ForEach(SetupButton);

            elementUXML = parentVisualElement.Query<Foldout>("EditFloatVarClip", EditorToolsCore.ToggleableGroup);

            UpdateDisplay();

            PageBuilderWindow.selectionChangedDelegate += UpdateDisplay;

            return this;
        }

        void OnDestroy()
        {
            PageBuilderWindow.selectionChangedDelegate -= UpdateDisplay;
        }

        public float initialValue;
        public float targetValue;

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
            PopulateTargetValueFromSelection,
            PopulateTargetValueFromInit,
            SetTargetValue
        }

        void UpdateDisplay()
        {
            bool dependencySelected = false;

            for (int i = 0; i < TimelineEditor.selectedClips.Length; i++) {
                if (TimelineEditor.selectedClips[i].asset is LerpFloatVarClip) {
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
                    propertyField.RegisterCallback<ChangeEvent<float>>((ChangeEvent<float> evt) => {
                        if (populateButtonPressed == false) {
                            SetInitialValue(TimelineEditor.selectedClips, initialValue);
                            TimelineUtilsCore.RefreshTimelineContentsModified();
                        }
                        populateButtonPressed = false;
                    });
                    break;

                case nameof(PropertyFieldNames.TargetValue):
                    propertyField.RegisterCallback<ChangeEvent<float>>((ChangeEvent<float> evt) => {
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

        public static float GetInitialValueFromSelection(TimelineClip[] clipSelection)
        {
            float value = float.NaN;
            Array.Sort(clipSelection, new Utils.ClipTimeSort());

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].asset is LerpFloatVarClip) {
                    value = (clipSelection[i].asset as LerpFloatVarClip).template.initialValue;
                    break;
                }
            }

            return value;
        }

        public static TimelineClip[] SetInitialValue(TimelineClip[] clipSelection, float targetValue)
        {
            List<TimelineClip> changedClips = new List<TimelineClip>();

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].asset is LerpFloatVarClip) {
                    TimelineClip clip = clipSelection[i];
                    changedClips.Add(clip);
                    LerpFloatVarClip clipAsset = clipSelection[i].asset as LerpFloatVarClip;
                    Undo.RecordObject(clipAsset, "set clip(s) initial value");
                    clipAsset.template.initialValue = targetValue;
                }
            }

            return changedClips.ToArray();
        }

        public static float GetTargetValueFromSelection(TimelineClip[] clipSelection)
        {
            float value = float.NaN;
            Array.Sort(clipSelection, new Utils.ClipTimeSort());

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].asset is LerpFloatVarClip) {
                    value = (clipSelection[i].asset as LerpFloatVarClip).template.targetValue;
                    break;
                }
            }

            return value;
        }

        public static TimelineClip[] SetTargetValue(TimelineClip[] clipSelection, float targetValue)
        {
            List<TimelineClip> changedClips = new List<TimelineClip>();

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].asset is LerpFloatVarClip) {
                    TimelineClip clip = clipSelection[i];
                    changedClips.Add(clip);
                    LerpFloatVarClip clipAsset = clipSelection[i].asset as LerpFloatVarClip;
                    Undo.RecordObject(clipAsset, "set clip(s) target value");
                    clipAsset.template.targetValue = targetValue;
                }
            }

            return changedClips.ToArray();
        }
    }
}
