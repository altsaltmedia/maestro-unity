using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine.Timeline;
using UnityEditor.Timeline;
using UnityEngine.Playables;

namespace AltSalt.Maestro.Animation
{
    public class EditResponsiveVector3Clip : ChildModuleWindow
    {
        static FloatField currentAspectRatio;

        protected override ChildModuleWindow Configure(ParentModuleWindow parentModuleWindow, string childRootUXMLName)
        {
            base.Configure(parentModuleWindow, childRootUXMLName);

            moduleChildUXML = parentModuleWindow.moduleWindowUXML.Query<Foldout>("EditResponsiveVector3Clip", ModuleUtils.toggleableGroup);

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

        public Vector3 initialValue = new Vector3(0, 0, 0);
        public Vector3 transposeInitialValue = new Vector3(0, 0, 0);
        public Vector3 initialValueInterval = new Vector3(0, 0, 0);
        public bool setInitIntervalOnValueChange = true;

        public Vector3 targetValue = new Vector3(0, 0, 0);
        public Vector3 transposeTargetValue = new Vector3(0, 0, 0);
        public Vector3 targetValueInterval = new Vector3(0, 0, 0);
        public bool setTargetIntervalOnValueChange = true;

        static bool populateButtonPressed = false;

        enum PropertyFieldNames
        {
            InitialValue,
            TransposeInitialValue,
            InitialValueInterval,
            SetInitIntervalOnValueChange,
            TargetValue,
            TransposeTargetValue,
            TargetValueInterval,
            SetTargetIntervalOnValueChange
        }

        enum ButtonNames
        {
            PopulateInitialValueFromSelection,
            PopulateInitialValueFromTarget,
            SetInitialValue,
            SetInitValueUsingInterval,
            PopulateTargetValueFromSelection,
            PopulateTargetValueFromInit,
            SetTargetValue,
            SetTargetValueUsingInterval
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
                if (TimelineEditor.selectedClips[i].asset is ResponsiveVector3Clip) {
                    dependencySelected = true;
                    break;
                }
            }

            moduleChildUXML.SetEnabled(dependencySelected);
            moduleChildUXML.value = dependencySelected;

            if (TimelineEditor.selectedClips.Length > 1) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.MultipleClipsSelected, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.MultipleClipsSelected, false);
            }
        }

        PropertyField SetupPropertyField(PropertyField propertyField)
        {
            switch (propertyField.name) {

                case nameof(PropertyFieldNames.InitialValue):
                    propertyField.RegisterCallback<ChangeEvent<Vector3>>((ChangeEvent<Vector3> evt) => {
                        if (populateButtonPressed == false) {
                            SetInitialValue(TimelineEditor.selectedClips, initialValue);
                            TimelineUtils.RefreshTimelineContentsModified();
                        }
                        populateButtonPressed = false;
                    });
                    break;

                case nameof(PropertyFieldNames.TransposeInitialValue):
                    propertyField.RegisterCallback<ChangeEvent<Vector3>>((ChangeEvent<Vector3> evt) => {
                        TransposeInitialValue(TimelineEditor.selectedClips, transposeInitialValue);
                        transposeInitialValue = new Vector3(0, 0, 0);
                        TimelineUtils.RefreshTimelineContentsModified();
                    });
                    break;

                case nameof(PropertyFieldNames.SetInitIntervalOnValueChange):
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.MultipleClipsSelected, propertyField);
                    break;

                case nameof(PropertyFieldNames.InitialValueInterval):
                    propertyField.RegisterCallback<ChangeEvent<Vector3>>((ChangeEvent<Vector3> evt) => {
                        if (setInitIntervalOnValueChange == true) {
                            SetInitValueUsingInterval(TimelineEditor.selectedClips, initialValue, initialValueInterval);
                            TimelineUtils.RefreshTimelineContentsModified();
                        }
                    });
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.MultipleClipsSelected, propertyField);
                    break;

                case nameof(PropertyFieldNames.TargetValue):
                    propertyField.RegisterCallback<ChangeEvent<Vector3>>((ChangeEvent<Vector3> evt) => {
                        if (populateButtonPressed == false) {
                            SetTargetValue(TimelineEditor.selectedClips, targetValue);
                            TimelineUtils.RefreshTimelineContentsModified();
                        }
                        populateButtonPressed = false;
                    });
                    break;

                case nameof(PropertyFieldNames.TransposeTargetValue):
                    propertyField.RegisterCallback<ChangeEvent<Vector3>>((ChangeEvent<Vector3> evt) => {
                        TransposeTargetValue(TimelineEditor.selectedClips, transposeTargetValue);
                        transposeTargetValue = new Vector3(0, 0, 0);
                        TimelineUtils.RefreshTimelineContentsModified();
                    });
                    break;

                case nameof(PropertyFieldNames.SetTargetIntervalOnValueChange):
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.MultipleClipsSelected, propertyField);
                    break;

                case nameof(PropertyFieldNames.TargetValueInterval):
                    propertyField.RegisterCallback<ChangeEvent<Vector3>>((ChangeEvent<Vector3> evt) => {
                        if (setTargetIntervalOnValueChange == true) {
                            SetTargetValueUsingInterval(TimelineEditor.selectedClips, targetValue, targetValueInterval);
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
                        TimelineUtils.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.SetInitValueUsingInterval):
                    button.clickable.clicked += () => {
                        SetInitValueUsingInterval(TimelineEditor.selectedClips, initialValue, initialValueInterval);
                        TimelineUtils.RefreshTimelineContentsModified();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.MultipleClipsSelected, button);
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
                        TimelineUtils.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.SetTargetValueUsingInterval):
                    button.clickable.clicked += () => {
                        SetTargetValueUsingInterval(TimelineEditor.selectedClips, targetValue, targetValueInterval);
                        TimelineUtils.RefreshTimelineContentsModified();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.MultipleClipsSelected, button);
                    break;
            }

            return button;
        }

        public static Vector3 GetInitialValueFromSelection(TimelineClip[] clipSelection)
        {
            Vector3 value = new Vector3(0, 0, 0);
            Array.Sort(clipSelection, new Utils.ClipTimeSort());

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].asset is ResponsiveVector3Clip) {
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
                if (clipSelection[i].asset is ResponsiveVector3Clip) {
                    TimelineClip clip = clipSelection[i];
                    changedClips.Add(clip);
                    ResponsiveVector3Clip clipAsset = clipSelection[i].asset as ResponsiveVector3Clip;
                    Undo.RecordObject(clipAsset, "set clip(s) initial position");
                    clipAsset.template.SaveNewInitialValue(targetValue);
                }
            }

            return changedClips.ToArray();
        }

        public static TimelineClip[] SetInitValueUsingInterval(TimelineClip[] clipSelection, Vector3 sourceValue, Vector3 interval)
        {
            List<TimelineClip> changedClips = new List<TimelineClip>();
            Array.Sort(clipSelection, new Utils.ClipTimeSort());
            Vector3 newValue = sourceValue;

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].asset is ResponsiveVector3Clip) {
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

        public static TimelineClip[] TransposeInitialValue(TimelineClip[] clipSelection, Vector3 transposeValue)
        {
            List<TimelineClip> changedClips = new List<TimelineClip>();

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].asset is ResponsiveVector3Clip) {
                    changedClips.Add(clipSelection[i]);
                    ResponsiveVector3Clip clipAsset = clipSelection[i].asset as ResponsiveVector3Clip;
                    Undo.RecordObject(clipAsset, "transpose clip(s) initial position");
                    Vector3 originalPosition = clipAsset.template.GetInitialValueAtBreakpoint(currentAspectRatio.value);
                    clipAsset.template.SaveNewInitialValue(new Vector3(originalPosition.x + transposeValue.x, originalPosition.y + transposeValue.y, originalPosition.z + transposeValue.z));
                }
            }

            return changedClips.ToArray();
        }

        public static Vector3 GetTargetValueFromSelection(TimelineClip[] clipSelection)
        {
            Vector3 value = new Vector3(0, 0, 0);
            Array.Sort(clipSelection, new Utils.ClipTimeSort());

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].asset is ResponsiveVector3Clip) {
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
                if (clipSelection[i].asset is ResponsiveVector3Clip) {
                    TimelineClip clip = clipSelection[i];
                    changedClips.Add(clip);
                    ResponsiveVector3Clip clipAsset = clipSelection[i].asset as ResponsiveVector3Clip;
                    Undo.RecordObject(clipAsset, "set clip(s) target position");
                    clipAsset.template.SaveNewTargetValue(targetValue);
                }
            }

            return changedClips.ToArray();
        }

        public static TimelineClip[] TransposeTargetValue(TimelineClip[] clipSelection, Vector3 transposeValue)
        {
            List<TimelineClip> changedClips = new List<TimelineClip>();

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].asset is ResponsiveVector3Clip) {
                    changedClips.Add(clipSelection[i]);
                    ResponsiveVector3Clip clipAsset = clipSelection[i].asset as ResponsiveVector3Clip;
                    Undo.RecordObject(clipAsset, "transpose clip(s) target position");
                    Vector3 originalPosition = clipAsset.template.GetTargetValueAtBreakpoint(currentAspectRatio.value);
                    clipAsset.template.SaveNewTargetValue(new Vector3(originalPosition.x + transposeValue.x, originalPosition.y + transposeValue.y, originalPosition.z + transposeValue.z));
                }
            }

            return changedClips.ToArray();
        }

        public static TimelineClip[] SetTargetValueUsingInterval(TimelineClip[] clipSelection, Vector3 sourceValue, Vector3 interval)
        {
            List<TimelineClip> changedClips = new List<TimelineClip>();
            Array.Sort(clipSelection, new Utils.ClipTimeSort());
            Vector3 newValue = sourceValue;

            for (int i = 0; i < clipSelection.Length; i++) {
                if (clipSelection[i].asset is ResponsiveVector3Clip) {
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
